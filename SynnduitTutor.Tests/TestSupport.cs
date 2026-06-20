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

    /// <summary>The course id used throughout the tests.</summary>
    public const string CourseId = "synnduit";

    /// <summary>A single-course catalog wrapping the real curriculum store.</summary>
    public static CourseCatalog Catalog(CurriculumStore store) =>
        CourseCatalog.Single(CourseId, "Synnduit Tutorial", store);

    /// <summary>Default donut config (all bonuses = 1, level bonus = 3, threshold = 6).</summary>
    public static DonutOptions DonutOpts() => new();

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
