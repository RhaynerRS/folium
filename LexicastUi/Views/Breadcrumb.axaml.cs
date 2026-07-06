using Avalonia.Controls;

namespace LexicastUi.Views;

public partial class Breadcrumb : UserControl
{
    public Breadcrumb()
    {
        InitializeComponent();
    }

    public void SetCrumbs(params BreadcrumbItem[] items)
    {
        Root.Children.Clear();
        for (int i = 0; i < items.Length; i++)
        {
            BreadcrumbItem item = items[i];
            if (item.OnClick is { } onClick)
            {
                var link = new Button { Content = item.Label };
                link.Classes.Add("BreadcrumbLink");
                link.Click += (_, _) => onClick();
                Root.Children.Add(link);
            }
            else
            {
                var current = new TextBlock { Text = item.Label };
                current.Classes.Add("BreadcrumbCurrent");
                Root.Children.Add(current);
            }

            if (i < items.Length - 1)
            {
                var separator = new TextBlock { Text = "/" };
                separator.Classes.Add("BreadcrumbSeparator");
                Root.Children.Add(separator);
            }
        }
    }
}
