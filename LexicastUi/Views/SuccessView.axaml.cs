using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LexicastUi.Models;

namespace LexicastUi.Views;

public partial class SuccessView : UserControl
{
    private readonly INavigationHost _host;
    private readonly TranslationJob _job;

    public SuccessView(INavigationHost host, TranslationJob job)
    {
        _host = host;
        _job = job;

        InitializeComponent();

        Crumbs.SetCrumbs(
            new BreadcrumbItem("Translations", host.ShowTranslations),
            new BreadcrumbItem("Download Ready"));

        SubtitleText.Text = $"{job.Name} is ready to download in {job.TargetLanguage}.";
        FileNameText.Text = job.SourceFilename;
        MetaLineText.Text = $"Translated to {job.TargetLanguage} · {job.SubmitKind}";
    }

    private async void DownloadButton_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save translation",
            SuggestedFileName = $"translated_{_job.SourceFilename}",
            FileTypeChoices = new[] { new FilePickerFileType("EPUB") { Patterns = new[] { "*.epub" } } }
        });

        if (file is null)
        {
            return;
        }

        DownloadButton.IsEnabled = false;
        DownloadProgressBar.IsVisible = true;
        try
        {
            await App.ApiClient.DownloadAsync(_job.JobId, file.Path.LocalPath);
        }
        catch (Exception ex)
        {
            ErrorText.Text = ex.Message;
            ErrorBanner.IsVisible = true;
        }
        finally
        {
            DownloadButton.IsEnabled = true;
            DownloadProgressBar.IsVisible = false;
        }
    }

    private void BackLink_Tapped(object? sender, TappedEventArgs e) => _host.ShowTranslations();
}
