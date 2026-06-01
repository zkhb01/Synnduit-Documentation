using Microsoft.EntityFrameworkCore;

namespace SynnduitTutor.Data;

public sealed class TutorDbContext(DbContextOptions<TutorDbContext> options) : DbContext(options)
{
    public DbSet<Learner> Learners => Set<Learner>();
    public DbSet<ConceptMastery> ConceptMastery => Set<ConceptMastery>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Learner>(e =>
        {
            e.HasIndex(x => x.ExternalId).IsUnique();
            e.HasMany(x => x.Mastery).WithOne(x => x.Learner!).HasForeignKey(x => x.LearnerId);
        });

        b.Entity<ConceptMastery>(e =>
        {
            e.HasIndex(x => new { x.LearnerId, x.ConceptId }).IsUnique();
        });
    }
}
