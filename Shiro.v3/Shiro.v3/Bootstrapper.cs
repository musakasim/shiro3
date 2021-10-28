using System.IO;
using System.Windows;
using Shiro.Controller;
using Shiro.Library;
using Shiro.Model;

namespace Shiro
{
    public class Bootstrapper
    {
        /// <summary>
        /// todo: accessing this like Bootstrapper.AppConfigSource is not nice, need an appropriate place to put this
        /// </summary>
        public static Nini.Config.XmlConfigSource AppConfigSource { get; set; }
         
        /// <summary>
        /// Initilizes and configures AppWide and AppBefore stuff
        ///     Initializes/Loads config files
        ///     Sets Theme
        /// </summary>
        public static void Bootstrap()
        {

            //Initialize IoC modules
            IoCContainer.Build();

            //Load Configurations
            GuardConfigFile();
            AppConfigSource = new Nini.Config.XmlConfigSource("shiro.stngs") { AutoSave = true };

            ConfigureTheme();

            //ShiroEntryBzzt icindeki static Kontroller propertilerini doldurmak icin ShiroEntryBzzt ctor@u cagiriroyrum.
            // todo: bundan memnun degilim, boyle olmasin
            ShiroEntryBzzt.SetControllerProperties(IoCContainer.Resolve<IKanjiInfoController>(), IoCContainer.Resolve<IBookmarkController>());
        }


        /// <summary>
        /// Ensures config file existence, makes a new one if it's not already exist
        /// </summary>
        private static void GuardConfigFile()
        {
            if (!File.Exists("shiro.stngs"))
            {
                var configSource = new Nini.Config.XmlConfigSource();
                configSource.Save("shiro.stngs");
            }
        }

        /// <summary>
        /// Sets application user theme, 
        /// if only user defined theme is found applies the user theme
        /// </summary>
        private static void ConfigureTheme()
        {
            ThemeManager.GetSetConfig();
            var userThemeResourceDictionary = ThemeManager.GetUserThemeResourceDictionary();
            if (userThemeResourceDictionary != null)
                ThemeManager.ChangeTheme(userThemeResourceDictionary);
        }
    }
}
