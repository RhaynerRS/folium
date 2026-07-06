using System.Collections.Generic;

namespace LexicastUi.Models;

public sealed record SubmitKindOption(SubmitKind Kind, string Title, string Description)
{
    public static readonly IReadOnlyList<SubmitKindOption> All = new List<SubmitKindOption>
    {
        new(SubmitKind.REPLACE, "REPLACE", "Replace the original text entirely with the translation."),
        new(SubmitKind.APPEND_TEXT, "APPEND_TEXT", "Insert translated text inline after each paragraph."),
        new(SubmitKind.APPEND_BLOCK, "APPEND_BLOCK", "Append translated content as a new block after each section."),
    };
}
