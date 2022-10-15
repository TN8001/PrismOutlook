using System;
using Prism.Mvvm;
using Prism.Regions;

namespace PrismOutlook.Core;

public class ViewModelBase : BindableBase, IConfirmNavigationRequest
{
    public virtual void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        => continuationCallback(true);

    public virtual bool IsNavigationTarget(NavigationContext navigationContext) => true;

    public virtual void OnNavigatedFrom(NavigationContext navigationContext) { }

    public virtual void OnNavigatedTo(NavigationContext navigationContext) { }
}
