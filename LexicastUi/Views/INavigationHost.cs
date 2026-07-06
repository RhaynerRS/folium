using LexicastUi.Models;

namespace LexicastUi.Views;

public interface INavigationHost
{
    void ShowTranslations();

    void ShowNewTranslation();

    void ShowProgress(TranslationJob job);

    void ShowSuccess(TranslationJob job);
}
