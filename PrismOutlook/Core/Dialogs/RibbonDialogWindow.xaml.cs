using Prism.Services.Dialogs;

namespace PrismOutlook.Core.Dialogs;

public partial class RibbonDialogWindow : IDialogWindow
{
    public IDialogResult Result { get; set; }

    public RibbonDialogWindow() => InitializeComponent();
}
