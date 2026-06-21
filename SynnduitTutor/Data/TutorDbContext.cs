using Microsoft.EntityFrameworkCore;

namespace SynnduitTutor.Data;

public sealed class TutorDbContext(DbContextOptions<TutorDbContext> options) : DbContext(options)
{
    public DbSet<Learner> Learners => Set<Learner>();
    public DbSet<ConceptMastery> ConceptMastery => Set<ConceptMastery>();
    public DbSet<DonutAward> DonutAwards => Set<DonutAward>();
    public DbSet<Voucher> Vouchers => Set<Voucher>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Learner>(e =>
        {
            e.HasIndex(x => x.ExternalId).IsUnique();
            e.HasMany(x => x.Mastery).WithOne(x => x.Learner!).HasForeignKey(x => x.LearnerId);
        });

        b.Entity<ConceptMastery>(e =>
        {
            e.HasIndex(x => new { x.LearnerId, x.CourseId, x.ConceptId }).IsUnique();
        });

        b.Entity<DonutAward>(e =>
        {
            e.HasIndex(x => new { x.LearnerId, x.CourseId, x.VoucherId });
            e.HasIndex(x => new { x.LearnerId, x.CourseId, x.LevelId, x.Reason });
        });

        b.Entity<Voucher>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
        });
    }
}
