using Avalonia.Media;

namespace LexicastUi.Models;

/// <summary>
/// Presentation wrapper around a <see cref="TranslationJob"/> for the translations list
/// (table/cards). Precomputes the display strings and brushes the design calls for.
/// </summary>
public sealed class JobRow
{
    public JobRow(TranslationJob job)
    {
        Job = job;
        var meta = JobStatusMeta.For(job.Status);
        StatusLabel = meta.Label;
        StatusForeground = meta.Foreground;
        StatusBackground = meta.Background;
        Pulsing = meta.Pulsing;
        IsClickable = job.Status is "queued" or "running" or "completed";
        ActionLabel = job.Status switch
        {
            "completed" => "Download",
            "queued" or "running" => "View progress",
            "cancelling" => "Cancelling…",
            "failed" => "Failed",
            "cancelled" => "Cancelled",
            _ => string.Empty,
        };
    }

    public TranslationJob Job { get; }

    public string Name => Job.Name;
    public string JobId => Job.JobId;
    public string Language => Job.TargetLanguage;
    public string SubmitKindLabel => Job.SubmitKind;
    public string Created => Job.CreatedLabel;

    public string StatusLabel { get; }
    public IBrush StatusForeground { get; }
    public IBrush StatusBackground { get; }
    public bool Pulsing { get; }
    public bool IsClickable { get; }
    public string ActionLabel { get; }
}
