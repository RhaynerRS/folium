namespace LexicastUi.Views;

public interface INavigationHost
{
    void ShowUpload();

    void ShowProgress(string jobId, string sourceFileName);
}
