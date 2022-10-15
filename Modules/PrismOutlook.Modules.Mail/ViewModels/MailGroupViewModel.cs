using System.Collections.ObjectModel;
using Prism.Commands;
using PrismOutlook.Business;
using PrismOutlook.Core;

namespace PrismOutlook.Modules.Mail.ViewModels;

public class MailGroupViewModel : ViewModelBase
{
    public ObservableCollection<NavigationItem> Items { get => _items; set => SetProperty(ref _items, value); }
    private ObservableCollection<NavigationItem> _items;

    public DelegateCommand<NavigationItem> SelectedCommand => _selectedCommand ??= new(ExecuteSelectedCommand);
    private DelegateCommand<NavigationItem> _selectedCommand;

    private readonly IApplicationCommands _applicationCommands;

    public MailGroupViewModel(IApplicationCommands applicationCommands)
    {
        GenerateMenu();
        _applicationCommands = applicationCommands;
    }

    private void ExecuteSelectedCommand(NavigationItem navigationItem)
    {
        if (navigationItem != null)
            _applicationCommands.NavigateCommand.Execute(navigationItem.NavigationPath);
    }

    private void GenerateMenu()
    {
        Items = new();

        var root = new NavigationItem() { Caption = "Personal Folder", NavigationPath = "MailList?id=Default", IsExpanded = true };
        root.Items.Add(new() { Caption = FolderParameters.Inbox, NavigationPath = GetNavigationPath(FolderParameters.Inbox) });
        root.Items.Add(new() { Caption = FolderParameters.Deleted, NavigationPath = GetNavigationPath(FolderParameters.Deleted) });
        root.Items.Add(new() { Caption = FolderParameters.Sent, NavigationPath = GetNavigationPath(FolderParameters.Sent) });

        Items.Add(root);
    }

    private string GetNavigationPath(string folder)
        => $"MailList?{FolderParameters.FolderKey}={folder}";
}
