using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PrismOutlook.Business;
using PrismOutlook.Core;

namespace PrismOutlook.Modules.Mail.Menus;

public partial class MailGroup : TabItem, IOutlookBarGroup
{
    public MailGroup()
    {
        InitializeComponent();
        _dataTree.Loaded += DataTree_Loaded;
    }

    private void DataTree_Loaded(object sender, RoutedEventArgs e)
    {
        _dataTree.Loaded -= DataTree_Loaded;

        var parentNode = _dataTree.Items[0] as NavigationItem;
        var nodeToSelect = parentNode.Items[0];

        // TreeViewItemを探すのがそもそも間違い
        // ふつうはvmにIsSelectedを作ってバインド
        var treeViewItem = GetTreeViewItem(_dataTree, nodeToSelect);
        treeViewItem.IsSelected = true;
    }

    public string DefaultNavigationPath => $"MailList?{FolderParameters.FolderKey}={FolderParameters.Inbox}";




    // [方法: TreeView での TreeViewItem の検索 - WPF .NET Framework | Microsoft Learn](https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/controls/how-to-find-a-treeviewitem-in-a-treeview?redirectedfrom=MSDN&view=netframeworkdesktop-4.8#Y486)
    private TreeViewItem GetTreeViewItem(ItemsControl container, object item)
    {
        if (container != null)
        {
            if (container.DataContext == item)
            {
                return container as TreeViewItem;
            }

            if (container is TreeViewItem && !((TreeViewItem)container).IsExpanded)
            {
                container.SetValue(TreeViewItem.IsExpandedProperty, true);
            }

            container.ApplyTemplate();
            var itemsPresenter = (ItemsPresenter)container.Template.FindName("ItemsHost", container);
            if (itemsPresenter != null)
            {
                itemsPresenter.ApplyTemplate();
            }
            else
            {
                itemsPresenter = FindVisualChild<ItemsPresenter>(container);
                if (itemsPresenter == null)
                {
                    container.UpdateLayout();
                    itemsPresenter = FindVisualChild<ItemsPresenter>(container);
                }
            }

            var itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);
            for (int i = 0, count = container.Items.Count; i < count; i++)
            {
                TreeViewItem subContainer;
                if (itemsHostPanel is VirtualizingStackPanel)
                {
                    subContainer = (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(i);
                }
                else
                {
                    subContainer = (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(i);
                    subContainer.BringIntoView();
                }

                if (subContainer != null)
                {
                    var resultContainer = GetTreeViewItem(subContainer, item);
                    if (resultContainer != null) return resultContainer;
                    else subContainer.IsExpanded = false;
                }
            }
        }

        return null;
    }

    private T FindVisualChild<T>(Visual visual) where T : Visual
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
        {
            var child = (Visual)VisualTreeHelper.GetChild(visual, i);
            if (child != null)
            {
                if (child is T correctlyTyped) return correctlyTyped;

                var descendent = FindVisualChild<T>(child);
                if (descendent != null) return descendent;
            }
        }

        return null;
    }
}
