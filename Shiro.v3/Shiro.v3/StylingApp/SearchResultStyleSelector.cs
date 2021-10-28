using System.Windows;
using System.Windows.Controls;

namespace Shiro.StylingApp
{

    /// <summary>
    /// Item style selector for a listing container which returns one of two set style 
    ///  for odd index first style, for even index second style
    /// Use like this:
    ///     <code>
    ///         <stylingApp:SearchResultStyleSelector x:Key="SearchResultStyleSelector" 
    ///                             EvenIndexedItemStyle="{StaticResource ShiroEntrySearchResultListViewItemStyle}" 
    ///                             OddIndexedItemStyle="{StaticResource ShiroEntrySearchResultListViewItemStyle}" />
    ///        and for ListView;
    ///         instead of
    ///                ItemContainerStyle="{StaticResource ShiroEntrySearchResultListViewItemStyle}"
    ///         use this
    ///                ItemContainerStyleSelector="{StaticResource SearchResultStyleSelector}"
    ///     </code>
    /// </summary>
    public class SearchResultStyleSelector : StyleSelector
    {

        public Style EvenIndexedItemStyle { get; set; }
        public Style OddIndexedItemStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var listViewItem = container as ListViewItem;
            var listView = ItemsControl.ItemsControlFromItemContainer(listViewItem) as ListView;
            if (listView != null)
            {
                int index = listView.ItemContainerGenerator.IndexFromContainer(container);
                var styleIndex = index % 2;
                if (styleIndex == 1)
                    return OddIndexedItemStyle;
                else
                    return EvenIndexedItemStyle;
            }
            return base.SelectStyle(item, container);
        }

    }
}
