using System.Collections.Generic;

namespace LexicastUi.Models;

public sealed record Language(string Code, string Label)
{
    public override string ToString() => Label;

    public static readonly IReadOnlyList<Language> All = new List<Language>
    {
        new("es", "Spanish"),
        new("fr", "French"),
        new("de", "German"),
        new("pt", "Portuguese"),
        new("ja", "Japanese"),
        new("zh", "Chinese (Simplified)"),
        new("it", "Italian"),
        new("ko", "Korean"),
    };
}
