using System;
using System.Collections.Generic;
using System.Linq;
using PrismOutlook.Business;
using PrismOutlook.Services.Data;
using PrismOutlook.Services.Interfaces;

namespace PrismOutlook.Services;

public class MailService : IMailService
{
    private static readonly List<MailMessage> InboxItems = new()
    {
        new()
        {
            Id = 1,
            From = "jerrynixon@microsoft.com",
            To = new(){ "jane@doe.com", "john@doe.com" },
            Subject = "This is a test email",
            Body = Resources.DavidSmit_SampleCoverLetterEmail,
            DateSent = DateTime.Now,
        },
        new()
        {
            Id = 2,
            From = "jerrynixon@microsoft.com",
            To = new(){ "jane@doe.com", "john@doe.com" },
            Subject = "This is a test email 2",
            Body = Resources.Barbara_Bailey_RE_GraphicDesignerCoverLetter,
            DateSent = DateTime.Now.AddDays(-1),
        },
        new()
        {
            Id = 3,
            From = "jerrynixon@microsoft.com",
            To = new(){ "jane@doe.com", "john@doe.com" },
            Subject = "This is a test email 3",
            Body = Resources.MargaretJones_RE_GraphicDesignerCoverLetter,
            DateSent = DateTime.Now.AddDays(-5),
        },
    };
    private static readonly List<MailMessage> SentItems = new();
    private static readonly List<MailMessage> DeletedItems = new();

    public void DeleteMessage(int id)
    {
        var messages = new List<MailMessage>();

        var message = DeletedItems.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            _ = DeletedItems.Remove(message);
            return;
        }

        message = InboxItems.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            _ = InboxItems.Remove(message);
        }
        else
        {
            message = SentItems.FirstOrDefault(m => m.Id == id);
            if (message != null) _ = SentItems.Remove(message);
        }

        if (message != null) DeletedItems.Add(message);
    }

    public IList<MailMessage> GetDeletedItems() => DeletedItems;

    public IList<MailMessage> GetInboxItems() => InboxItems;

    public MailMessage GetMessage(int id)
    {
        var messages = new List<MailMessage>();
        messages.AddRange(InboxItems);
        messages.AddRange(SentItems);
        messages.AddRange(DeletedItems);
        return messages.FirstOrDefault(m => m.Id == id);
    }

    public IList<MailMessage> GetSentItems() => SentItems;

    public void SendMessage(MailMessage message)
    {
        message.DateSent = DateTime.Now;
        SentItems.Add(message);
    }
}
