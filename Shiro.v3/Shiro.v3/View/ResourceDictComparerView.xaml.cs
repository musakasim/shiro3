using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Shiro.View.ViewManagement;
using Shiro.ViewModel;

namespace Shiro.View
{
    /// <summary>
    /// Interaction logic for ResourceDictComparerView.xaml
    /// </summary>
    public partial class ResourceDictComparerView : Pencere
    {
        public ResourceDictComparerViewModel ResourceDictComparerViewModel { get; set; }

        public ResourceDictComparerView()
        {
            InitializeComponent();
            ResourceDictComparerViewModel = new ResourceDictComparerViewModel();
            DataContext = ResourceDictComparerViewModel;
        }

        /// <summary>
        /// Sync ListBox1 and ListBox2 scroll positions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var sourceScrollViewer = FindVisualChild<ScrollViewer>(sender as DependencyObject);
            var frameworkElement = sender as FrameworkElement;
            // ReSharper disable once PossibleUnintendedReferenceComparison
            var otherList = frameworkElement == ListBox1 ? ListBox2 : ListBox1;
            var targetScrollViewer = FindVisualChild<ScrollViewer>(otherList);
            targetScrollViewer.ScrollToVerticalOffset(sourceScrollViewer.VerticalOffset);
        }

        // helper method
        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}
