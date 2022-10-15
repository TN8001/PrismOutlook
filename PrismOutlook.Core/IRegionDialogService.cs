using System;
using Prism.Services.Dialogs;

namespace PrismOutlook.Core;

public interface IRegionDialogService
{
    void Show(string name, IDialogParameters dialogParameters, Action<IDialogResult> callback);
}
