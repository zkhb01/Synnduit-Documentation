using Microsoft.EntityFrameworkCore;
using SynnduitTutor.Data;

namespace SynnduitTutor.Services;

public sealed record DonutBalance(int Earned, int Available, int Redeemed);

/// <summary>Reads donut balances and redeems unredeemed donuts onto a printable <see cref="Voucher"/>.</summary>
public sealed class DonutService(IDbContextFactory<TutorDbContext> dbFactory, DonutOptions options)
{
    public DonutOptions Options => options;
    public int Threshold => options.RedeemThresholdPoints;

    public bool CanRedeem(DonutBalance b) => b.Available >= options.RedeemThresholdPoints;

    public async Task<DonutBalance> GetBalanceAsync(int learnerId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var awards = await db.DonutAwards.Where(a => a.LearnerId == learnerId).ToListAsync();
        var earned = awards.Sum(a => a.Points);
        var redeemed = awards.Where(a => a.VoucherId != null).Sum(a => a.Points);
        return new DonutBalance(earned, earned - redeemed, redeemed);
    }

    /// <summary>
    /// Redeems all currently-unredeemed donuts onto a new voucher. Requires the configured minimum
    /// (default 6). Throws <see cref="InvalidOperationException"/> if below threshold.
    /// </summary>
    public async Task<Voucher> RedeemAsync(int learnerId, string milestone)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var unredeemed = await db.DonutAwards.Where(a => a.LearnerId == learnerId && a.VoucherId == null).ToListAsync();
        var points = unredeemed.Sum(a => a.Points);
        if (points < options.RedeemThresholdPoints)
            throw new InvalidOperationException(
                $"Need at least {options.RedeemThresholdPoints} donuts to redeem a voucher; you have {points}.");

        var voucher = new Voucher
        {
            LearnerId = learnerId,
            Code = NewCode(),
            Points = points,
            Milestone = string.IsNullOrWhiteSpace(milestone) ? "Milestone reached" : milestone,
            ManagerName = options.ManagerName,
            IssuedUtc = DateTime.UtcNow
        };
        db.Vouchers.Add(voucher);
        await db.SaveChangesAsync();              // assigns voucher.Id

        foreach (var a in unredeemed) a.VoucherId = voucher.Id;
        await db.SaveChangesAsync();

        return voucher;
    }

    public async Task<Voucher?> GetVoucherAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Vouchers.FindAsync(id);
    }

    public async Task<List<Voucher>> GetVouchersAsync(int learnerId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Vouchers.Where(v => v.LearnerId == learnerId)
            .OrderByDescending(v => v.IssuedUtc).ToListAsync();
    }

    private static string NewCode() => "DNT-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
}
