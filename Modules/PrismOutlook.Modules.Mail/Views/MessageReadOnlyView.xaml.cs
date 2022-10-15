using System.Windows.Controls;
using PrismOutlook.Core;
using PrismOutlook.Modules.Mail.Menus;

namespace PrismOutlook.Modules.Mail.Views;

[DependentView(RegionNames.RibbonRegion, typeof(MessageReadOnlyTab))]
public partial class MessageReadOnlyView : UserControl, ISupportDataContext
{
    public MessageReadOnlyView() => InitializeComponent();
}
