using System.Windows;
using System.Windows.Controls;
using PrismOutlook.Core;

namespace PrismOutlook.Views;

public partial class MainWindow // : Window
{
    private readonly IApplicationCommands _applicationCommands;

    public MainWindow(IApplicationCommands applicationCommands)
    {
        InitializeComponent();
        _applicationCommands = applicationCommands;
    }

    private void XamOutlookBar_SelectedGroupChanged(object sender, RoutedEventArgs e)
    {
        if (((TabControl)sender).SelectedItem is IOutlookBarGroup group)
            _applicationCommands.NavigateCommand.Execute(group.DefaultNavigationPath);
    }
}
