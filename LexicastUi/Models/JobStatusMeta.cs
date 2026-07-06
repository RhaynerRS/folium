using System.Collections.Generic;
using Avalonia.Media;

namespace LexicastUi.Models;

public sealed class JobStatusMeta
{
    public required string Label { get; init; }
    public required IBrush Foreground { get; init; }
    public required IBrush Background { get; init; }
    public bool Pulsing { get; init; }

    private static readonly IBrush Blue = new SolidColorBrush(Color.Parse("#2E6FED"));
    private static readonly IBrush BlueBg = new SolidColorBrush(Color.Parse("#E9EDFF"));
    private static readonly IBrush Green = new SolidColorBrush(Color.Parse("#1E9E6B"));
    private static readonly IBrush GreenBg = new SolidColorBrush(Color.Parse("#E6F6EF"));
    private static readonly IBrush Grey = new SolidColorBrush(Color.Parse("#7A7F87"));
    private static readonly IBrush GreyBg = new SolidColorBrush(Color.Parse("#F0EFEC"));
    private static readonly IBrush Red = new SolidColorBrush(Color.Parse("#D6483F"));
    private static readonly IBrush RedBg = new SolidColorBrush(Color.Parse("#FBEAE8"));

    private static readonly Dictionary<string, JobStatusMeta> ByStatus = new()
    {
        ["queued"] = new JobStatusMeta { Label = "Queued", Foreground = Blue, Background = BlueBg, Pulsing = true },
        ["running"] = new JobStatusMeta { Label = "In Progress", Foreground = Blue, Background = BlueBg, Pulsing = true },
        ["cancelling"] = new JobStatusMeta { Label = "Cancelling", Foreground = Grey, Background = GreyBg, Pulsing = true },
        ["completed"] = new JobStatusMeta { Label = "Completed", Foreground = Green, Background = GreenBg },
        ["failed"] = new JobStatusMeta { Label = "Failed", Foreground = Red, Background = RedBg },
        ["cancelled"] = new JobStatusMeta { Label = "Cancelled", Foreground = Grey, Background = GreyBg },
    };

    private static readonly JobStatusMeta Fallback =
        new() { Label = "Unknown", Foreground = Grey, Background = GreyBg };

    public static JobStatusMeta For(string status) => ByStatus.GetValueOrDefault(status, Fallback);
}
