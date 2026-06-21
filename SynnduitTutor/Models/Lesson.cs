namespace SynnduitTutor.Models;

/// <summary>A parsed lessons/&lt;conceptId&gt;.md: YAML-ish front-matter + rendered HTML body.</summary>
public sealed class Lesson
{
    public string ConceptId { get; set; } = "";
    public Dictionary<string, string> FrontMatter { get; set; } = new();
    public string MarkdownBody { get; set; } = "";
    public string HtmlBody { get; set; } = "";

    public string? Get(string key) => FrontMatter.TryGetValue(key, out var v) ? v : null;
}
