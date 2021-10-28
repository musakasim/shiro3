using System.Windows;
using System.Windows.Input;
using Shiro.View.ViewManagement;
using Shiro.ViewModel;

namespace Shiro.View
{
    /// <summary>
    ///     Interaction logic for DictionaryView.xaml
    /// </summary>
    public partial class DictionaryView : Pencere
    {
        public DictionaryView()
        {
            InitializeComponent();
            DictionaryViewModel = new DictionaryViewModel();
            DataContext = DictionaryViewModel;

        }

        public DictionaryViewModel DictionaryViewModel { get; set; }

        /// <summary>
        /// program açıldığında odağı arama kutusuna getirmek için IsVisibleChanged(Odağı üzerine alabildiğimiz ilk hale bu anda gelmiş olduğuna inandığım için bu event kullanıldı)
        /// event'înda odağı textbox üzerine alıyoruz:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemsFlowListBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // focus on search box:
            SearchTextBox.Select(0, 0);
            SearchTextBox.Focus();
            Keyboard.Focus(SearchTextBox);

            //if ((bool)e.NewValue == true)
            //{
            //    Dispatcher.BeginInvoke(
            //    DispatcherPriority.ContextIdle,
            //    new Action(delegate
            //    {
            //        SearchTextBox.Focus();
            //        //ItemsFlowListBox.ScrollIntoView(ItemsFlowListBox.SelectedItem);
            //    }));
            //}
        }
    }
}