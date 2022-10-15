using System.Windows.Controls;
using PrismOutlook.Core;
using PrismOutlook.Modules.Mail.Menus;

namespace PrismOutlook.Modules.Mail.Views;

[DependentView(RegionNames.RibbonRegion, typeof(HomeTab))]
public partial class MailList : UserControl, ISupportDataContext
{
    public MailList() => InitializeComponent();
}
