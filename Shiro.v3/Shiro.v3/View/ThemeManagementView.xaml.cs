using System.Windows;
using System.Windows.Media;
using Shiro.View.ViewManagement;
using Shiro.ViewModel;

namespace Shiro.View
{
    /// <summary>
    /// Interaction logic for ThemeManagementView.xaml
    /// </summary>
    public partial class ThemeManagementView : Pencere
    {
        public ThemeManagementViewModel ThemeManagementViewModel { get; set; }

        public ThemeManagementView()
        {
            InitializeComponent();
            ThemeManagementViewModel = new ThemeManagementViewModel();
            DataContext = ThemeManagementViewModel;
        }

        private void ColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            ThemeManagementViewModel.ApplyBrushesCommand.Execute(sender);
        }
    }
}
