using System.Collections.Generic;
using PrismOutlook.Business;

namespace PrismOutlook.Services.Interfaces;

public interface IMailService
{
    IList<MailMessage> GetInboxItems();
    IList<MailMessage> GetSentItems();
    IList<MailMessage> GetDeletedItems();

    MailMessage GetMessage(int id);
    void DeleteMessage(int id);
    void SendMessage(MailMessage message);
}
