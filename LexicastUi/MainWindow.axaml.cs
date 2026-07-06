using Avalonia.Controls;
using Avalonia.Interactivity;
using LexicastUi.Models;
using LexicastUi.Views;

namespace LexicastUi;

public partial class MainWindow : Window, INavigationHost
{
    public MainWindow()
    {
        InitializeComponent();
        ShowTranslations();
    }

    public void ShowTranslations()
    {
        RootContent.Content = new TranslationsView(this);
    }

    public void ShowNewTranslation()
    {
        RootContent.Content = new NewTranslationView(this);
    }

    public void ShowProgress(TranslationJob job)
    {
        RootContent.Content = new ProgressView(this, job);
    }

    public void ShowSuccess(TranslationJob job)
    {
        RootContent.Content = new SuccessView(this, job);
    }

    private async void SettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        await new SettingsWindow().ShowDialog(this);
    }
}
