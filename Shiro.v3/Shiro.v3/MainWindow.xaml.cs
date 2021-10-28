using System;
using System.Windows;
using System.Windows.Input; 
using Shiro.ViewModel;

namespace Shiro
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(); 
        }

        /// <summary>
        ///     pencereyi taşıyabilmeyi sağlar, formda boş bir yer tıklanarak pencere taşınabilir
        /// </summary>
        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonMinimize_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(112233);
        }
    }
}