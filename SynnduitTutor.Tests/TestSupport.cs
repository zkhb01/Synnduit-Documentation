using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SynnduitTutor.Data;
using SynnduitTutor.Services;

namespace SynnduitTutor.Tests;

internal static class TestSupport
{
    /// <summary>Walk up from the test bin folder to the repo's Curriculum/ directory.</summary>
    public static string FindCurriculumRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        for (var i = 0; i < 10 && dir is not null; i++, dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "Curriculum");
            if (File.Exists(Path.Combine(candidate, "concept-graph.json")))
                return candidate;
        }
        throw new DirectoryNotFoundException($"Curriculum/ not found above {AppContext.BaseDirectory}");
    }

    public static CurriculumStore RealStore() =>
        new(FindCurriculumRoot(), NullLogger<CurriculumStore>.Instance);

    /// <summary>A throwaway file-based SQLite database for one test.</summary>
    public sealed class TempDbFactory : IDbContextFactory<TutorDbContext>, IDisposable
    {
        private readonly string _path;
        private readonly DbContextOptions<TutorDbContext> _opts;

        public TempDbFactory()
        {
            _path = Path.Combine(Path.GetTempPath(), $"tutor-test-{Guid.NewGuid():N}.db");
            _opts = new DbContextOptionsBuilder<TutorDbContext>().UseSqlite($"Data Source={_path}").Options;
            using var db = CreateDbContext();
            db.Database.EnsureCreated();
        }

        public TutorDbContext CreateDbContext() => new(_opts);

        public void Dispose()
        {
            try { if (File.Exists(_path)) File.Delete(_path); } catch { /* best effort */ }
        }
    }
}
