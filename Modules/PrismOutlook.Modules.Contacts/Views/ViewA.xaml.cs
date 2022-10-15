using System.Windows.Controls;
using PrismOutlook.Core;
using PrismOutlook.Modules.Contacts.Menus;

namespace PrismOutlook.Modules.Contacts.Views;

[DependentView(RegionNames.RibbonRegion, typeof(HomeTab))]
public partial class ViewA : UserControl
{
    public ViewA() => InitializeComponent();
}
