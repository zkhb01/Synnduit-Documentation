namespace SynnduitTutor.Services;

/// <summary>A training course exposed under the "Training" menu: a stable id (used in the URL),
/// a display name, and the curriculum it loads.</summary>
public sealed record CourseDefinition(string Id, string Name, string? Description, string CurriculumPath);

/// <summary>
/// The set of training courses the tutor hosts. Each course owns its own <see cref="CurriculumStore"/>
/// (concept-graph + items + lessons) loaded from a curriculum folder. Courses are configured via the
/// "Courses" section in appsettings; with none configured the catalog falls back to a single
/// <c>synnduit</c> course resolved by walking up for a <c>Curriculum/</c> folder (keeps dev zero-config).
/// </summary>
public sealed class CourseCatalog
{
    private readonly Dictionary<string, CurriculumStore> _stores = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<CourseDefinition> _courses = new();

    public CourseCatalog(IConfiguration config, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        var log = loggerFactory.CreateLogger<CourseCatalog>();
        var configured = config.GetSection("Courses").Get<List<CourseConfig>>() ?? new List<CourseConfig>();

        if (configured.Count == 0)
        {
            // Dev / back-compat default: one course resolved the old way (walk up for Curriculum/).
            var root = CurriculumStore.ResolveCurriculumRoot(config["Curriculum:Path"], env.ContentRootPath)
                       ?? throw new DirectoryNotFoundException(
                           "Could not locate a Curriculum/ folder (looked for concept-graph.json). " +
                           "Set Curriculum:Path, or configure the Courses section in appsettings.json.");
            configured.Add(new CourseConfig
            {
                Id = "synnduit",
                Name = "Synnduit Tutorial",
                Description = "Learn how Synnduit works, one concept at a time.",
                Path = root,
            });
        }

        foreach (var c in configured)
        {
            if (string.IsNullOrWhiteSpace(c.Id))
            {
                log.LogError("Skipping a course with a missing Id.");
                continue;
            }
            // Empty Path → resolve the default way (walk up for a Curriculum/ folder); set it for a real
            // course in production. This lets the appsettings example carry an empty Path in dev.
            var configuredPath = string.IsNullOrWhiteSpace(c.Path) ? null : c.Path;
            var resolved = CurriculumStore.ResolveCurriculumRoot(configuredPath, env.ContentRootPath);
            if (resolved is null)
            {
                log.LogError("Course '{Id}': curriculum path '{Path}' not found; skipping.", c.Id, c.Path);
                continue;
            }
            if (_stores.ContainsKey(c.Id))
            {
                log.LogError("Duplicate course id '{Id}'; skipping the later one.", c.Id);
                continue;
            }
            _stores[c.Id] = new CurriculumStore(resolved, loggerFactory.CreateLogger<CurriculumStore>());
            _courses.Add(new CourseDefinition(
                c.Id, string.IsNullOrWhiteSpace(c.Name) ? c.Id : c.Name, c.Description, resolved));
        }

        if (_courses.Count == 0)
            throw new InvalidOperationException(
                "No training courses could be loaded. Check the Courses configuration / Curriculum paths.");

        log.LogInformation("Loaded {Count} training course(s): {Ids}",
            _courses.Count, string.Join(", ", _courses.Select(c => c.Id)));
    }

    private CourseCatalog(IEnumerable<(CourseDefinition def, CurriculumStore store)> courses)
    {
        foreach (var (def, store) in courses)
        {
            _stores[def.Id] = store;
            _courses.Add(def);
        }
    }

    /// <summary>Test/host helper: build a catalog from a single already-constructed store.</summary>
    public static CourseCatalog Single(string id, string name, CurriculumStore store) =>
        new(new[] { (new CourseDefinition(id, name, null, store.Root), store) });

    public IReadOnlyList<CourseDefinition> Courses => _courses;

    public CourseDefinition? GetCourse(string? courseId) =>
        courseId is null ? null : _courses.FirstOrDefault(c => string.Equals(c.Id, courseId, StringComparison.OrdinalIgnoreCase));

    public CurriculumStore? GetStore(string? courseId) =>
        courseId is not null && _stores.TryGetValue(courseId, out var s) ? s : null;

    private sealed class CourseConfig
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string Path { get; set; } = "";
    }
}
