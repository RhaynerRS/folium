using Avalonia.Controls;
using LexicastUi.Views;

namespace LexicastUi;

public partial class MainWindow : Window, INavigationHost
{
    public MainWindow()
    {
        InitializeComponent();
        ShowUpload();
    }

    public void ShowUpload()
    {
        RootContent.Content = new UploadView(this);
    }

    public void ShowProgress(string jobId, string sourceFileName)
    {
        RootContent.Content = new ProgressView(this, jobId, sourceFileName);
    }
}