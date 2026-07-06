using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LexicastUi.Models;

namespace LexicastUi.Views;

public partial class ProgressView : UserControl
{
    private readonly INavigationHost _host;
    private readonly DispatcherTimer _pollTimer;
    private readonly TranslationJob _job;
    private bool _isPolling;
    private bool _cancelRequested;

    public ProgressView(INavigationHost host, TranslationJob job)
    {
        _host = host;
        _job = job;

        InitializeComponent();

        Crumbs.SetCrumbs(
            new BreadcrumbItem("Translations", host.ShowTranslations),
            new BreadcrumbItem("New Translation", host.ShowNewTranslation),
            new BreadcrumbItem("Translating…"));

        SubtitleText.Text = $"{job.Name} · {job.TargetLanguage}";
        JobIdText.Text = $"Job ID: {job.JobId}";
        SessionsText.Text = $"Sessions: {job.Concurrency}";

        ApplyJobState(job);

        _pollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
        _pollTimer.Tick += PollTimer_Tick;
        _pollTimer.Start();
    }

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _pollTimer.Stop();
    }

    private async void PollTimer_Tick(object? sender, EventArgs e)
    {
        if (_isPolling)
        {
            return;
        }

        _isPolling = true;
        try
        {
            TranslationJob job = await App.ApiClient.GetJobAsync(_job.JobId);
            ApplyJobState(job);

            if (job.IsCompleted)
            {
                _pollTimer.Stop();
                _host.ShowSuccess(job);
            }
            else if (job.IsCancelled)
            {
                _pollTimer.Stop();
                _host.ShowTranslations();
            }
            else if (job.IsFailed)
            {
                _pollTimer.Stop();
                ShowError(job.Error ?? "The translation failed.");
            }
        }
        catch (Exception ex)
        {
            _pollTimer.Stop();
            ShowError(ex.Message);
        }
        finally
        {
            _isPolling = false;
        }
    }

    private void ApplyJobState(TranslationJob job)
    {
        double percent = Math.Clamp(job.Progress * 100.0, 0, 100);
        JobProgressBar.Value = percent;
        ProgressPercentText.Text = $"{percent:0}%";
        StatusLineText.Text = percent >= 100 ? "Finalizing…" : "Translating chapters…";

        CancelButton.IsVisible = job.IsCancellable || job.IsCancelling;
        CancelButton.IsEnabled = job.IsCancellable && !_cancelRequested;
        CancelButton.Content = job.IsCancelling ? "Cancelling…" : "Cancel translation";
    }

    private async void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_cancelRequested)
        {
            return;
        }

        _cancelRequested = true;
        CancelButton.IsEnabled = false;
        CancelButton.Content = "Cancelling…";
        try
        {
            await App.ApiClient.CancelJobAsync(_job.JobId);
            _pollTimer.Stop();
            _host.ShowTranslations();
        }
        catch (Exception ex)
        {
            _cancelRequested = false;
            CancelButton.IsEnabled = true;
            ShowError(ex.Message);
        }
    }

    private void BackLink_Tapped(object? sender, TappedEventArgs e) => _host.ShowTranslations();

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorBanner.IsVisible = true;
    }
}
