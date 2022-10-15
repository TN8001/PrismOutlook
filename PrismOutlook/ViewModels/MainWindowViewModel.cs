using System;
using Prism.Commands;
using Prism.Regions;
using PrismOutlook.Core;

namespace PrismOutlook.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Title { get => _title; set => SetProperty(ref _title, value); }
    private string _title = "Prism Application";

    public DelegateCommand<string> NavigateCommand => _navigateCommand ??= new(ExecuteNavigateCommand);
    private DelegateCommand<string> _navigateCommand;

    private readonly IRegionManager _regionManager;

    public MainWindowViewModel(IRegionManager regionManager, IApplicationCommands applicationCommands)
    {
        _regionManager = regionManager;
        applicationCommands.NavigateCommand.RegisterCommand(NavigateCommand);
    }

    private void ExecuteNavigateCommand(string navigationPath)
    {
        if (string.IsNullOrEmpty(navigationPath)) throw new ArgumentNullException(nameof(navigationPath));

        _regionManager.RequestNavigate(RegionNames.ContentRegion, navigationPath);
    }
}
