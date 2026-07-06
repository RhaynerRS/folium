using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LexicastUi.Models;

namespace LexicastUi.Views;

public partial class TranslationsView : UserControl
{
    private readonly INavigationHost _host;
    private readonly DispatcherTimer _refreshTimer;
    private bool _isLoading;
    private bool _showingCards;

    public TranslationsView(INavigationHost host)
    {
        _host = host;
        InitializeComponent();

        Crumbs.SetCrumbs(new BreadcrumbItem("Translations"));
        SetViewMode(showingCards: false);

        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
        _refreshTimer.Tick += async (_, _) => await RefreshJobsAsync();
        _refreshTimer.Start();

        _ = RefreshJobsAsync();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _refreshTimer.Stop();
    }

    private async Task RefreshJobsAsync()
    {
        if (_isLoading)
        {
            return;
        }

        _isLoading = true;
        try
        {
            List<TranslationJob> jobs = await App.ApiClient.ListJobsAsync();
            IReadOnlyList<JobRow> rows = jobs
                .OrderByDescending(j => j.CreatedAt)
                .Select(j => new JobRow(j))
                .ToList();

            TableJobsItems.ItemsSource = rows;
            CardJobsItems.ItemsSource = rows;
            JobCountText.Text = $"{rows.Count} EPUBs translated";
        }
        catch
        {
            // Keep showing the last known list; the refresh timer will retry.
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void TableViewButton_Click(object? sender, RoutedEventArgs e) => SetViewMode(showingCards: false);

    private void CardsViewButton_Click(object? sender, RoutedEventArgs e) => SetViewMode(showingCards: true);

    private void SetViewMode(bool showingCards)
    {
        _showingCards = showingCards;
        TableCard.IsVisible = !showingCards;
        CardJobsItems.IsVisible = showingCards;
        TableViewButton.Classes.Set("Active", !showingCards);
        CardsViewButton.Classes.Set("Active", showingCards);
    }

    private void NewTranslationButton_Click(object? sender, RoutedEventArgs e) => _host.ShowNewTranslation();

    private void JobRow_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control { DataContext: JobRow row } || !row.IsClickable)
        {
            return;
        }

        if (row.Job.IsCompleted)
        {
            _host.ShowSuccess(row.Job);
        }
        else
        {
            _host.ShowProgress(row.Job);
        }
    }
}
