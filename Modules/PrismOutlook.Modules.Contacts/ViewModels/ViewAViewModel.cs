using Prism.Mvvm;
using Prism.Regions;

namespace PrismOutlook.Modules.Contacts.ViewModels;

public class ViewAViewModel : BindableBase, IRegionMemberLifetime
{
    public string Message { get => _message; set => SetProperty(ref _message, value); }
    private string _message;

    public bool KeepAlive => false;

    public ViewAViewModel() => Message = "View A from your Prism Module";
}
