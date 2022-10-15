using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using PrismOutlook.Business;
using PrismOutlook.Core;
using PrismOutlook.Services.Interfaces;

namespace PrismOutlook.Modules.Mail.ViewModels;

public class MailListViewModel : MessageViewModelBase
{
    public ObservableCollection<MailMessage> Messages { get => _messages; set => SetProperty(ref _messages, value); }
    private ObservableCollection<MailMessage> _messages = new();

    public DelegateCommand NewMessageCommand => _newMessageCommand ??= new(ExecuteNewMessageCommand);
    private DelegateCommand _newMessageCommand;

    private string _currentFolder = FolderParameters.Inbox;

    public MailListViewModel(IMailService mailService, IRegionDialogService regionDialogService) : base(mailService, regionDialogService) { }

    private void ExecuteNewMessageCommand()
    {
        var parameters = new DialogParameters
        {
            { "id", 0 },
            { "MessageMode", MessageMode.New },
        };

        RegionDialogService.Show("MessageView", parameters, (result) =>
        {
            if (_currentFolder == FolderParameters.Sent)
                Messages.Add(result.Parameters.GetValue<MailMessage>("messageSent"));
        });
    }

    protected override void ExecuteDeleteMessage()
    {
        base.ExecuteDeleteMessage();

        _ = Messages.Remove(Message);
    }

    protected override void HandleMessageCallBack(IDialogResult result)
    {
        var mode = result.Parameters.GetValue<MessageMode>(MailParameters.MessageMode);
        if (mode == MessageMode.Delete)
        {
            var messageId = result.Parameters.GetValue<int>(MailParameters.MessageId);

            var messageToDelete = Messages.Where(x => x.Id == messageId).FirstOrDefault();
            if (messageToDelete != null)
                _ = Messages.Remove(messageToDelete);
        }
    }

    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        _currentFolder = navigationContext.Parameters.GetValue<string>(FolderParameters.FolderKey);
        LoadMessages(_currentFolder);
    }

    private void LoadMessages(string folder)
    {
        Messages = folder switch
        {
            FolderParameters.Inbox => new(MailService.GetInboxItems()),
            FolderParameters.Sent => new(MailService.GetSentItems()),
            FolderParameters.Deleted => new(MailService.GetDeletedItems()),
            _ => null,
        };

        Message = Messages?.FirstOrDefault();
    }
}
