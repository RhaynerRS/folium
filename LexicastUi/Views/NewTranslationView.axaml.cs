using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LexicastUi.Models;

namespace LexicastUi.Views;

public partial class NewTranslationView : UserControl
{
    private readonly INavigationHost _host;
    private string? _selectedFilePath;
    private SubmitKind _submitKind = SubmitKind.APPEND_BLOCK;

    public NewTranslationView(INavigationHost host)
    {
        _host = host;
        InitializeComponent();

        Crumbs.SetCrumbs(
            new BreadcrumbItem("Translations", host.ShowTranslations),
            new BreadcrumbItem("New Translation"));
    }

    private async void FileDropZone_Tapped(object? sender, TappedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select an EPUB file",
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("EPUB") { Patterns = new[] { "*.epub" } } }
        });

        if (files.Count == 0)
        {
            return;
        }

        _selectedFilePath = files[0].Path.LocalPath;
        FileNameText.Text = files[0].Name;
        FileHelperText.Text = "Ready to upload";
        SubmitButton.IsEnabled = true;
    }

    private void ConcurrencySlider_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        ConcurrencyValueText.Text = ((int)ConcurrencySlider.Value).ToString();
    }

    private void ReplaceButton_Click(object? sender, RoutedEventArgs e) => SetSubmitKind(SubmitKind.REPLACE, ReplaceButton);

    private void AppendTextButton_Click(object? sender, RoutedEventArgs e) => SetSubmitKind(SubmitKind.APPEND_TEXT, AppendTextButton);

    private void AppendBlockButton_Click(object? sender, RoutedEventArgs e) => SetSubmitKind(SubmitKind.APPEND_BLOCK, AppendBlockButton);

    private void SetSubmitKind(SubmitKind kind, Button activeButton)
    {
        _submitKind = kind;
        ReplaceButton.Classes.Set("Active", ReferenceEquals(activeButton, ReplaceButton));
        AppendTextButton.Classes.Set("Active", ReferenceEquals(activeButton, AppendTextButton));
        AppendBlockButton.Classes.Set("Active", ReferenceEquals(activeButton, AppendBlockButton));
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e) => _host.ShowTranslations();

    private async void SubmitButton_Click(object? sender, RoutedEventArgs e)
    {
        HideError();

        if (string.IsNullOrEmpty(_selectedFilePath))
        {
            ShowError("Select an .epub file before continuing.");
            return;
        }

        var language = (Language)LanguageBox.SelectedItem!;
        int concurrency = (int)ConcurrencySlider.Value;
        string? userPrompt = string.IsNullOrWhiteSpace(UserPromptBox.Text) ? null : UserPromptBox.Text!.Trim();

        SubmitButton.IsEnabled = false;
        SubmitProgressBar.IsVisible = true;

        try
        {
            TranslationJob job = await App.ApiClient.CreateTranslationAsync(
                _selectedFilePath, language.Label, concurrency, userPrompt, _submitKind);

            _host.ShowProgress(job);
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            SubmitButton.IsEnabled = true;
            SubmitProgressBar.IsVisible = false;
        }
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorBanner.IsVisible = true;
    }

    private void HideError()
    {
        ErrorBanner.IsVisible = false;
    }
}
