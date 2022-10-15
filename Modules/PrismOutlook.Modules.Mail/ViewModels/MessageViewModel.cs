using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using PrismOutlook.Business;
using PrismOutlook.Services.Interfaces;

namespace PrismOutlook.Modules.Mail.ViewModels;

public class MessageViewModel : BindableBase, IDialogAware
{
    public MailMessage Message { get => _message; set => SetProperty(ref _message, value); }
    private MailMessage _message;

    public DelegateCommand SendMessageCommand => _sendMessageCommand ??= new(ExecuteSendMessageCommand);
    private DelegateCommand _sendMessageCommand;

    private readonly IMailService _mailService;

    private void ExecuteSendMessageCommand()
    {
        _mailService.SendMessage(Message);

        //todo: fix magic string
        var parameters = new DialogParameters
        {
            { "messageSent", Message },
        };

        RequestClose?.Invoke(new DialogResult(ButtonResult.Yes, parameters));
    }

    public MessageViewModel(IMailService mailService) => _mailService = mailService;

    //TODO: use this
    public string Title => "Mail Message";

    public event Action<IDialogResult> RequestClose;

    public bool CanCloseDialog() => true;

    public void OnDialogClosed() { }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Message = new() { From = "blagunas@infragistics.com" };

        var messageMode = parameters.GetValue<MessageMode>(MailParameters.MessageMode);
        if (messageMode != MessageMode.New)
        {
            var messageId = parameters.GetValue<int>(MailParameters.MessageId);
            var originalMessage = _mailService.GetMessage(messageId);

            Message.To = GetToEmails(messageMode, originalMessage);

            if (messageMode is MessageMode.Reply or MessageMode.ReplyAll)
                Message.CC = originalMessage.CC;

            Message.Subject = GetMessageSubject(messageMode, originalMessage);

            //TODO: append RTF with reply header
            Message.Body = originalMessage.Body;
        }
    }

    private string GetMessageSubject(MessageMode mode, MailMessage originalMessage)
    {
        var prefix = mode switch
        {
            MessageMode.Reply => "RE:",
            MessageMode.ReplyAll => "RE:",
            MessageMode.Forward => "FW:",
            _ => string.Empty,
        };

        return originalMessage.Subject.ToLower()
            .StartsWith(prefix.ToLower()) ? originalMessage.Subject : $"{prefix} {originalMessage.Subject}";
    }

    private ObservableCollection<string> GetToEmails(MessageMode mode, MailMessage message)
    {
        var toEmails = new ObservableCollection<string>();

        switch (mode)
        {
            case MessageMode.Reply:
                toEmails.Add(message.From);
                break;
            case MessageMode.ReplyAll:
                //TODO: create user/account settings for sender email
                _ = toEmails.AddRange(message.To.Where(x => x != "blagunas@infragistics.com"));
                toEmails.Add(message.From);
                break;
            case MessageMode.Forward:
                break;
        }

        return toEmails;
    }
}
