using System;
using System.Windows;

namespace Shiro
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Application Entry Point.
        /// I changed build action ApplicationDefinition to Page and implemented Main explicitly to show splash screen
        /// otherwise ApplicationDefinition produces default main method
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var splashScreen = new SplashScreen("StylingApp\\Image\\Japanese-Tea-Ceremony-600x320.jpg");
            splashScreen.Show(false);
            var app = new App();
            app.InitializeComponent();
            Bootstrapper.Bootstrap();
            splashScreen.Close(TimeSpan.FromMilliseconds(300));
            app.Run();
        } 
    }
}