using System.Text;
using Microsoft.AspNetCore.Components;

namespace SynnduitTutor.Services;

/// <summary>
/// Minimal, dependency-free C# syntax highlighter for rendering code sample answers as a
/// VS Code "Dark+" editor pane (line numbers + token colors). Self-contained on purpose — no JS
/// library or CDN, so it works offline and in locked-down browsers. Single-pass per line, and every
/// token's text is HTML-encoded, so the output is safe to render as a raw MarkupString.
/// </summary>
public static class CodeHighlighter
{
    private static readonly HashSet<string> Keywords = new(StringComparer.Ordinal)
    {
        "public", "private", "protected", "internal", "static", "readonly", "abstract", "sealed",
        "class", "interface", "struct", "enum", "void", "var", "new", "return", "using", "typeof",
        "namespace", "this", "base", "get", "set", "string", "int", "long", "bool", "double",
        "decimal", "object", "null", "true", "false", "async", "await", "in", "out", "is", "as",
        "if", "else", "foreach", "for", "while", "switch", "case", "default", "throw", "try", "catch",
    };

    /// <summary>
    /// True when a sample answer is multi-line C#-ish code (worth rendering as an editor pane) rather
    /// than a prose sentence. Prose answers — even multi-line ones — lack these C# signals.
    /// </summary>
    public static bool LooksLikeCode(string? s) =>
        !string.IsNullOrEmpty(s) && s.Contains('\n') &&
        (s.Contains(';') || s.Contains("public ") || s.Contains("=>"));

    /// <summary>Renders code as line-numbered, token-colored HTML for a &lt;pre class="vscode"&gt; block.</summary>
    public static MarkupString Highlight(string code)
    {
        var sb = new StringBuilder();
        var lines = code.Replace("\r\n", "\n").Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            sb.Append("<span class=\"cl\"><span class=\"ln\">").Append(i + 1).Append("</span><span class=\"cc\">");
            HighlightLine(lines[i], sb);
            sb.Append("</span></span>");
        }
        return new MarkupString(sb.ToString());
    }

    private static void HighlightLine(string line, StringBuilder sb)
    {
        int i = 0, n = line.Length;
        while (i < n)
        {
            var c = line[i];

            // // line comment — rest of the line
            if (c == '/' && i + 1 < n && line[i + 1] == '/')
            {
                Emit("c", line[i..], sb);
                return;
            }

            // "string literal"
            if (c == '"')
            {
                var j = i + 1;
                while (j < n && !(line[j] == '"' && line[j - 1] != '\\')) j++;
                if (j < n) j++;   // include the closing quote
                Emit("s", line[i..j], sb);
                i = j;
                continue;
            }

            // [Attribute(...)] — only when the bracket starts the line (after indentation)
            if (c == '[' && IsAttributeStart(line, i))
            {
                var j = line.IndexOf(']', i);
                if (j < 0) j = n - 1;
                Emit("a", line[i..(j + 1)], sb);
                i = j + 1;
                continue;
            }

            // identifier / keyword
            if (char.IsLetter(c) || c == '_')
            {
                var j = i;
                while (j < n && (char.IsLetterOrDigit(line[j]) || line[j] == '_')) j++;
                var word = line[i..j];
                Emit(Keywords.Contains(word) ? "k" : null, word, sb);
                i = j;
                continue;
            }

            // number
            if (char.IsDigit(c))
            {
                var j = i;
                while (j < n && (char.IsLetterOrDigit(line[j]) || line[j] == '.')) j++;
                Emit("nm", line[i..j], sb);
                i = j;
                continue;
            }

            Emit(null, c.ToString(), sb);
            i++;
        }
    }

    // An attribute opens the line if '[' is the first non-whitespace char and a letter follows.
    private static bool IsAttributeStart(string line, int i)
    {
        var k = i - 1;
        while (k >= 0 && char.IsWhiteSpace(line[k])) k--;
        return k < 0 && i + 1 < line.Length && char.IsLetter(line[i + 1]);
    }

    private static void Emit(string? cls, string text, StringBuilder sb)
    {
        var enc = System.Net.WebUtility.HtmlEncode(text);
        if (cls is null) sb.Append(enc);
        else sb.Append("<span class=\"").Append(cls).Append("\">").Append(enc).Append("</span>");
    }
}
