using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace PrismOutlook.Core.Dialogs;

public class RegionDialogService : IRegionDialogService
{
    private readonly IContainerExtension _containerExtension;
    private readonly IRegionManager _regionManager;

    public RegionDialogService(IContainerExtension containerExtension, IRegionManager regionManager)
    {
        _containerExtension = containerExtension;
        _regionManager = regionManager;
    }

    public void Show(string name, IDialogParameters dialogParameters, Action<IDialogResult> callback)
    {
        var window = _containerExtension.Resolve<RibbonDialogWindow>();

        var scopedRegionManager = _regionManager.CreateRegionManager();
        RegionManager.SetRegionManager(window, scopedRegionManager);

        var region = scopedRegionManager.Regions[RegionNames.ContentRegion];

        region.RequestNavigate(name);

        var activeView = region.ActiveViews.FirstOrDefault() as FrameworkElement;
        if (activeView.DataContext is not IDialogAware dialogAware)
            throw new ArgumentNullException(nameof(dialogAware));

        dialogAware.OnDialogOpened(dialogParameters);

        void requestCloseHandler(IDialogResult result)
        {
            window.Result = result;
            window.Close();
        }

        void closingHandler(object o, CancelEventArgs e)
        {
            if (!dialogAware.CanCloseDialog()) e.Cancel = true;
        }

        window.Closing += closingHandler;

        void loadedHandler(object o, RoutedEventArgs e)
        {
            window.Loaded -= loadedHandler;
            dialogAware.RequestClose += requestCloseHandler;
        }

        window.Loaded += loadedHandler;

        void closedHandler(object o, EventArgs e)
        {
            window.Closed -= closedHandler;
            window.Closing -= closingHandler;
            dialogAware.RequestClose -= requestCloseHandler;

            dialogAware.OnDialogClosed();

            var result = window.Result;
            result ??= new DialogResult();

            callback.Invoke(result);

            window.DataContext = null;
            window.Content = null;
        }

        window.Closed += closedHandler;

        window.Owner = Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Show();
    }
}
