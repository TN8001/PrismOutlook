﻿using Prism.Commands;
using Prism.Services.Dialogs;
using PrismOutlook.Business;
using PrismOutlook.Core;
using PrismOutlook.Services.Interfaces;

namespace PrismOutlook.Modules.Mail.ViewModels;

public class MessageViewModelBase : ViewModelBase
{
    public MailMessage Message { get => _message; set => SetProperty(ref _message, value); }
    private MailMessage _message;

    public DelegateCommand DeleteMessageCommand => _deleteMessageCommand ??= new(ExecuteDeleteMessage);
    private DelegateCommand _deleteMessageCommand;

    public DelegateCommand<string> MessageCommand => _messageCommand ??= new(ExecuteMessageCommand);
    private DelegateCommand<string> _messageCommand;

    protected IRegionDialogService RegionDialogService { get; private set; }

    protected IMailService MailService { get; private set; }

    public MessageViewModelBase(IMailService mailService, IRegionDialogService regionDialogService)
    {
        MailService = mailService;
        RegionDialogService = regionDialogService;
    }

    protected virtual void ExecuteDeleteMessage()
    {
        if (Message == null) return;

        MailService.DeleteMessage(Message.Id);
    }

    private void ExecuteMessageCommand(string parameter)
    {
        if (Message == null) return;

        var parameters = new DialogParameters();
        var viewName = "MessageView";
        var replyType = MessageMode.Read;

        switch (parameter)
        {
            case nameof(MessageMode.Read):
                viewName = "MessageReadOnlyView";
                replyType = MessageMode.Read;
                break;
            case nameof(MessageMode.Reply):
                replyType = MessageMode.Reply;
                break;
            case nameof(MessageMode.ReplyAll):
                replyType = MessageMode.ReplyAll;
                break;
            case nameof(MessageMode.Forward):
                replyType = MessageMode.Forward;
                break;
        }

        parameters.Add(MailParameters.MessageId, Message.Id);
        parameters.Add(MailParameters.MessageMode, replyType);

        RegionDialogService.Show(viewName, parameters, (result) => HandleMessageCallBack(result));
    }

    protected virtual void HandleMessageCallBack(IDialogResult result) { }
}
