using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Prism.Common;
using Prism.Ioc;
using Prism.Services.Dialogs;

namespace PrismOutlook.Core.Dialogs;

public class DialogServiceBase : IDialogService
{
    private readonly IContainerExtension _containerExtension;

    //TODO: delete
    protected IDialogWindow DialogWindow { get; private set; }

    public DialogServiceBase(IContainerExtension containerExtension)
        => _containerExtension = containerExtension;



    public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        => ShowDialogInternal(name, parameters, callback, false);

    public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        => ShowDialogInternal(name, parameters, callback, false);

    public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        => ShowDialogInternal(name, parameters, callback, true);

    public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        => ShowDialogInternal(name, parameters, callback, true);

    private void ShowDialogInternal(string name, IDialogParameters parameters, Action<IDialogResult> callback, bool isModal)
    {
        DialogWindow = CreateDialogWindow();

        ConfigureDialogWindowEvents(DialogWindow, callback);
        ConfigureDialogWindowContent(name, DialogWindow, parameters);

        //TODO: remove this
        InitializeDialogWindow(name, parameters);

        //TODO: do this
        //DialogWindow.Initialize(name, parameters);

        if (isModal) _ = DialogWindow.ShowDialog();
        else DialogWindow.Show();
    }

    //TODO: delete this
    protected virtual void InitializeDialogWindow(string name, IDialogParameters parameters) { }

    protected virtual IDialogWindow CreateDialogWindow() => _containerExtension.Resolve<IDialogWindow>();

    protected virtual void ConfigureDialogWindowContent(string dialogName, IDialogWindow window, IDialogParameters parameters)
    {
        var content = _containerExtension.Resolve<object>(dialogName);
        if (content is not FrameworkElement dialogContent)
            throw new NullReferenceException("A dialog's content must be a FrameworkElement");

        if (dialogContent.DataContext is not IDialogAware viewModel)
            throw new NullReferenceException("A dialog's ViewModel must implement the IDialogAware interface");

        ConfigureDialogWindowProperties(window, dialogContent, viewModel);

        MvvmHelpers.ViewAndViewModelAction<IDialogAware>(viewModel, d => d.OnDialogOpened(parameters));
    }

    protected virtual void ConfigureDialogWindowEvents(IDialogWindow dialogWindow, Action<IDialogResult> callback)
    {
        void requestCloseHandler(IDialogResult o)
        {
            dialogWindow.Result = o;
            dialogWindow.Close();
        }

        void loadedHandler(object o, RoutedEventArgs e)
        {
            dialogWindow.Loaded -= loadedHandler;
            dialogWindow.GetDialogViewModel().RequestClose += requestCloseHandler;
        }

        dialogWindow.Loaded += loadedHandler;

        void closingHandler(object o, CancelEventArgs e)
        {
            if (!dialogWindow.GetDialogViewModel().CanCloseDialog())
                e.Cancel = true;
        }

        dialogWindow.Closing += closingHandler;

        void closedHandler(object o, EventArgs e)
        {
            dialogWindow.Closed -= closedHandler;
            dialogWindow.Closing -= closingHandler;
            dialogWindow.GetDialogViewModel().RequestClose -= requestCloseHandler;

            dialogWindow.GetDialogViewModel().OnDialogClosed();

            dialogWindow.Result ??= new DialogResult();

            callback?.Invoke(dialogWindow.Result);

            dialogWindow.DataContext = null;
            dialogWindow.Content = null;
        }

        dialogWindow.Closed += closedHandler;
    }

    protected virtual void ConfigureDialogWindowProperties(IDialogWindow window, FrameworkElement dialogContent, IDialogAware viewModel)
    {
        var windowStyle = Dialog.GetWindowStyle(dialogContent);
        if (windowStyle != null)
            window.Style = windowStyle;

        //TODO: add to prism
        window.Content ??= dialogContent;

        if (viewModel != null)
            window.DataContext = viewModel; //we want the host window and the dialog to share the same data contex

        window.Owner ??= Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
    }
}

internal static class IDialogWindowExtensions
{
    internal static IDialogAware GetDialogViewModel(this IDialogWindow dialogWindow)
        => (IDialogAware)dialogWindow.DataContext;
}
