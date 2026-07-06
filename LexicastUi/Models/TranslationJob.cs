using System;
using System.Globalization;
using System.IO;
using System.Text.Json.Serialization;

namespace LexicastUi.Models;

public sealed class TranslationJob
{
    [JsonPropertyName("job_id")]
    public string JobId { get; set; } = string.Empty;

    [JsonPropertyName("source_filename")]
    public string SourceFilename { get; set; } = string.Empty;

    [JsonPropertyName("target_language")]
    public string TargetLanguage { get; set; } = string.Empty;

    [JsonPropertyName("submit_kind")]
    public string SubmitKind { get; set; } = string.Empty;

    [JsonPropertyName("concurrency")]
    public int Concurrency { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("progress")]
    public double Progress { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("warning")]
    public string? Warning { get; set; }

    public string Name => Path.GetFileNameWithoutExtension(SourceFilename);

    public bool IsCompleted => Status == "completed";

    public bool IsFailed => Status == "failed";

    public bool IsCancelling => Status == "cancelling";

    public bool IsCancelled => Status == "cancelled";

    public bool IsCancellable => Status is "queued" or "running";

    public string ProgressDisplay => $"{Progress * 100:0}%";

    public string? StatusDetail => Error ?? Warning;

    public string CreatedLabel => CreatedAt == default
        ? string.Empty
        : CreatedAt.ToLocalTime().ToString("MMM d, yyyy", CultureInfo.InvariantCulture);
}
