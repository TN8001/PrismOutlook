using System;
using System.Windows.Controls;
using PrismOutlook.Core;
using PrismOutlook.Modules.Mail.Menus;

namespace PrismOutlook.Modules.Mail.Views;

[DependentView(RegionNames.RibbonRegion, typeof(MessageTab))]
public partial class MessageView : UserControl, ISupportDataContext, ISupportRichText
{
    public RichTextBox RichTextEditor { get => _rte; set => throw new NotImplementedException(); }

    public MessageView() => InitializeComponent();
}
