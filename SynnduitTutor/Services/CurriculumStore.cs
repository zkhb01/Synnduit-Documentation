using System.Text;
using System.Text.Json;
using Markdig;
using SynnduitTutor.Models;

namespace SynnduitTutor.Services;

/// <summary>
/// Loads and caches the authored curriculum (concept-graph.json + items/*.json + lessons/*.md)
/// from the repo's Curriculum/ folder. Read-only; the gating/mastery state lives in the database.
/// </summary>
public sealed class CurriculumStore
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };
    private static readonly MarkdownPipeline MdPipeline =
        new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    private readonly string _root;
    private readonly ILogger<CurriculumStore> _log;

    private ConceptGraph? _graph;
    private readonly Dictionary<string, ItemBank> _banks = new();
    private readonly Dictionary<string, Lesson> _lessons = new();
    private readonly object _gate = new();

    public CurriculumStore(IConfiguration config, IWebHostEnvironment env, ILogger<CurriculumStore> log)
    {
        _log = log;
        _root = ResolveCurriculumRoot(config["Curriculum:Path"], env.ContentRootPath)
                ?? throw new DirectoryNotFoundException(
                    "Could not locate the Curriculum/ folder (looked for concept-graph.json). " +
                    "Set Curriculum:Path in appsettings.json.");
        _log.LogInformation("Curriculum root: {Root}", _root);
    }

    /// <summary>Test-friendly constructor: point directly at a Curriculum folder.</summary>
    public CurriculumStore(string curriculumRoot, ILogger<CurriculumStore> log)
    {
        _log = log;
        _root = Directory.Exists(curriculumRoot)
            ? Path.GetFullPath(curriculumRoot)
            : throw new DirectoryNotFoundException(curriculumRoot);
    }

    public string Root => _root;

    public ConceptGraph Graph
    {
        get
        {
            EnsureLoaded();
            return _graph!;
        }
    }

    public ItemBank? GetBank(string poolId)
    {
        EnsureLoaded();
        return _banks.GetValueOrDefault(poolId);
    }

    public Lesson? GetLesson(string conceptId)
    {
        EnsureLoaded();
        return _lessons.GetValueOrDefault(conceptId);
    }

    /// <summary>
    /// Pick up to <paramref name="max"/> items for an attempt, preferring items the learner has not
    /// seen (so re-assessment uses fresh questions); tops up with seen items if needed.
    /// </summary>
    public List<Item> SelectItems(string poolId, ISet<string> seenIds, int max)
    {
        var bank = GetBank(poolId);
        if (bank is null) return new List<Item>();

        var unseen = bank.Items.Where(i => !seenIds.Contains(i.Id)).ToList();
        var seen = bank.Items.Where(i => seenIds.Contains(i.Id)).ToList();
        return unseen.Concat(seen).Take(max).ToList();
    }

    private void EnsureLoaded()
    {
        if (_graph is not null) return;
        lock (_gate)
        {
            if (_graph is not null) return;

            var graphPath = Path.Combine(_root, "concept-graph.json");
            _graph = JsonSerializer.Deserialize<ConceptGraph>(File.ReadAllText(graphPath), JsonOpts)
                     ?? throw new InvalidDataException($"Failed to parse {graphPath}.");

            var itemsDir = Path.Combine(_root, "items");
            if (Directory.Exists(itemsDir))
            {
                foreach (var file in Directory.EnumerateFiles(itemsDir, "*.items.json"))
                {
                    try
                    {
                        var bank = JsonSerializer.Deserialize<ItemBank>(File.ReadAllText(file), JsonOpts);
                        if (bank is not null && !string.IsNullOrEmpty(bank.PoolId))
                            _banks[bank.PoolId] = bank;
                    }
                    catch (Exception ex) { _log.LogWarning(ex, "Skipping unparseable item bank {File}", file); }
                }
            }

            var lessonsDir = Path.Combine(_root, "lessons");
            if (Directory.Exists(lessonsDir))
            {
                foreach (var file in Directory.EnumerateFiles(lessonsDir, "*.md"))
                {
                    var conceptId = Path.GetFileNameWithoutExtension(file);
                    _lessons[conceptId] = ParseLesson(conceptId, File.ReadAllText(file));
                }
            }

            _log.LogInformation("Loaded {Concepts} concepts, {Banks} item banks, {Lessons} lessons.",
                _graph.AllConcepts.Count(), _banks.Count, _lessons.Count);
        }
    }

    private static Lesson ParseLesson(string conceptId, string raw)
    {
        var lesson = new Lesson { ConceptId = conceptId };
        var text = raw.Replace("\r\n", "\n");
        var body = text;

        if (text.StartsWith("---\n"))
        {
            var end = text.IndexOf("\n---", 4, StringComparison.Ordinal);
            if (end > 0)
            {
                var fm = text.Substring(4, end - 4);
                body = text[(end + 4)..].TrimStart('\n');
                foreach (var line in fm.Split('\n'))
                {
                    var idx = line.IndexOf(':');
                    if (idx <= 0) continue;
                    var key = line[..idx].Trim();
                    var val = line[(idx + 1)..].Trim().Trim('"');
                    if (key.Length > 0) lesson.FrontMatter[key] = val;
                }
            }
        }

        lesson.MarkdownBody = body;
        lesson.HtmlBody = Markdown.ToHtml(body, MdPipeline);
        return lesson;
    }

    /// <summary>Walk up from the content root looking for a Curriculum/ folder with the graph file.</summary>
    private static string? ResolveCurriculumRoot(string? configured, string contentRoot)
    {
        if (!string.IsNullOrWhiteSpace(configured))
            return Directory.Exists(configured) ? Path.GetFullPath(configured) : null;

        var dir = new DirectoryInfo(contentRoot);
        for (var i = 0; i < 8 && dir is not null; i++, dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "Curriculum");
            if (File.Exists(Path.Combine(candidate, "concept-graph.json")))
                return candidate;
        }
        return null;
    }
}
