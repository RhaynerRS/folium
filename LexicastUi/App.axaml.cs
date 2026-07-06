using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LexicastUi.Services;

namespace LexicastUi;

public partial class App : Application
{
    /// <summary>
    /// Shared client used by every view to talk to the Python translation API.
    /// </summary>
    public static TranslationApiClient ApiClient { get; } = new TranslationApiClient();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}