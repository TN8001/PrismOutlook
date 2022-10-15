using System.Windows.Controls;
using PrismOutlook.Core;

namespace PrismOutlook.Modules.Contacts.Menus;

public partial class ContactsGroup : TabItem, IOutlookBarGroup
{
    public ContactsGroup() => InitializeComponent();
    public string DefaultNavigationPath => "ViewA";
}
