using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xaml;
using Shiro.Library.Extensions;

namespace Shiro.Library
{
    public class ThemeManager
    {
        private static string _directoryName;
        private static string _userThemeUri;
        public List<FileInfo> Themes { get; set; }
        public static string FunnyThemeUri { get; set; }
        public static string DefaultThemeUri { get; set; }

        public static string UserThemeUri
        {
            get { return _userThemeUri; }
            set
            {
                _userThemeUri = value;
                if (value != null)
                    SetUserThemeInConfig();
            }
        }

        private static void SetUserThemeInConfig()
        {
            Bootstrapper.AppConfigSource.Configs["ThemeSettings"].Set("userThemeUri", _userThemeUri);
        }

        /// <summary>
        /// default theme definitions
        /// </summary>
        public static List<string> DefaultThemeUris
        {
            get
            {
                return new List<string>
                {
                    @"pack://application:,,,/StylingCommon\Themes\Euphoria.xaml",
                    @"pack://application:,,,/StylingCommon\Themes\ShinyRed.xaml"
                };
            }
        }

        /// <summary>
        /// produces new settings in settings file if ThemeSettings is not found
        /// then sets the settings to the properties
        /// </summary>
        public static void GetSetConfig()
        {
            if (Bootstrapper.AppConfigSource.Configs["ThemeSettings"] == null)
            {
                Bootstrapper.AppConfigSource.AddConfig("ThemeSettings");
                Bootstrapper.AppConfigSource.Configs["ThemeSettings"].Set("ThemeDirectory", @"StylingCommon\Themes");
                Bootstrapper.AppConfigSource.Configs["ThemeSettings"].Set("funnyThemeUri", @"ShinyRed.xaml");
                Bootstrapper.AppConfigSource.Configs["ThemeSettings"].Set("defaultThemeUri", @"Euphoria.xaml");
            }
            _directoryName = Bootstrapper.AppConfigSource.Configs["ThemeSettings"].Get("ThemeDirectory");
            FunnyThemeUri = Bootstrapper.AppConfigSource.Configs["ThemeSettings"].Get("funnyThemeUri");
            DefaultThemeUri = Bootstrapper.AppConfigSource.Configs["ThemeSettings"].Get("defaultThemeUri");
            UserThemeUri = Bootstrapper.AppConfigSource.Configs["ThemeSettings"].Get("userThemeUri");
        }

        /// <summary>
        /// todo:why this one is only used in tests?? 
        /// </summary>
        public void FindAvailableThemes()
        {
            var directory = FileHelper.FindInAppDirectory("Themes");
            var fileInfos = new DirectoryInfo(directory).GetFiles("*.xaml");
            Themes = fileInfos.ToList();
        }

        public static void ChangeTheme(Uri uri, Application app)
        {
            //var themeResource = app.Resources.MergedDictionaries[0];
            //themeResource.MergedDictionaries.Clear();
            //themeResource.MergedDictionaries.Add(new ResourceDictionary { Source = uri });
            var resourceDictionary = new ResourceDictionary { Source = uri };
            var themeResource = GetCurrentThemeResourceDictionary(app);
            ApplyResource(resourceDictionary, themeResource);
        }

        public static void ChangeTheme(ResourceDictionary userThemeResourceDictionary)
        {
            var currentThemeResourceDictionary = GetCurrentThemeResourceDictionary(Application.Current);
            ApplyResource(userThemeResourceDictionary, currentThemeResourceDictionary);
        }

        public static ResourceDictionary GetCurrentThemeResourceDictionary(Application app)
        {
            //todo: how do i know that app.Resources.MergedDictionaries[0] is the theme resourceDictionary?
            var themeResource = Application.Current.Resources.MergedDictionaries[0].MergedDictionaries[0];
            return themeResource;
        }

        public static ResourceDictionary GetUserThemeResourceDictionary()
        {
            string userThemeUri = UserThemeUri;// ?? DefaultThemeUri;
            ResourceDictionary themeResource = null;
            if (userThemeUri != null)
                themeResource = LoadResource(userThemeUri);
            return themeResource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="themeDirectory">xaml file name to be loaded</param>
        /// <returns></returns>
        public static ResourceDictionary LoadResource(string themeDirectory)
        {
            var themeFullPath = Path.Combine(_directoryName, themeDirectory);
            if (File.Exists(themeFullPath))
            {
                var s = new FileStream(themeFullPath, FileMode.Open);
                var rootElement = XamlServices.Load(s);
                s.Close();
                return (ResourceDictionary)rootElement;
            }
            return new ResourceDictionary();
        }

        /// <summary>
        /// Recursive!!!!
        /// applies values in a resourcedict to another resourcedict, 
        ///     apply means:remove the key and add again, this way there is no need to check if resorcedict contains the key
        /// doing so replaces only the existing keys, others stays as they are
        /// </summary>
        /// <param name="newResourceDictionary"></param>
        /// <param name="resourceToBeAppliedOn"></param>
        public static void ApplyResource(ResourceDictionary newResourceDictionary, ResourceDictionary resourceToBeAppliedOn)
        {
            foreach (DictionaryEntry r in newResourceDictionary)
            {
                //todo: bi if silinmeli: sebep: is it even possible that a resource dictinaries nested, mergeddictionaries used for this.
                //checks for nested resource dictionary, makes recursive call to apply nested dict.
                if (r.Value.GetType() == typeof(ResourceDictionary))
                {
                    //if the key is not exist first add key, then apply with recursive call
                    if (!resourceToBeAppliedOn.Contains(r.Key))
                        resourceToBeAppliedOn.Add(r.Key, r.Value);
                    ApplyResource((ResourceDictionary) r.Value, (ResourceDictionary) resourceToBeAppliedOn[r.Key]);
                }
                else 
                {
                    //remove the key and add again, this way there is no need to check if resorcedict contains the key
                    resourceToBeAppliedOn.Remove(r.Key);
                    resourceToBeAppliedOn.Add(r.Key, r.Value);   
                }
            }
            for (int i = 0; i < newResourceDictionary.MergedDictionaries.Count; i++)
            {
                //todo:MergedDictioaries might not be in same order
                var mergedDict = newResourceDictionary.MergedDictionaries[i];
                ApplyResource(mergedDict, resourceToBeAppliedOn.MergedDictionaries[i]);
            }
        }

        //public static void ApplyBrushesToCurrentTheme(IEnumerable<KeyValuePair<string, SolidColorBrush>> themeBrushes)
        //{
        //    var newRd = BrushesToResourceDictionary(themeBrushes);
        //    ApplyResource(newRd, GetCurrentThemeResourceDictionary(Application.Current));
        //}

        ///// <summary>
        ///// todo:make this an extension method for IEnumerable|KeyValuePair|string, SolidColorBrush|| with the name AsResourceDict
        ///// returns a new resource dictinoary which contains the input brushes
        ///// </summary>
        //public static ResourceDictionary BrushesToResourceDictionary(IEnumerable<KeyValuePair<string, SolidColorBrush>> brushes)
        //{
        //    var newRd = new ResourceDictionary();
        //    foreach (var solidColorBrush in brushes)
        //        newRd.Add(solidColorBrush.Key, solidColorBrush.Value);
        //    return newRd;
        //}

        public static void SaveTheme(ResourceDictionary themeResource, string themeName, string directoryName)
        {
            //todo: buradaki field(_directoryName) bagimliligindan kurtulmaliyim, muhtemelen setconfig yapisi yanlis
            if (directoryName == null)
                directoryName = _directoryName;
            
            //ensure theme directory exists
            //todo: we need a GuardDirectory method!!
            Directory.CreateDirectory(directoryName);
            //get full file path to save:
            var themeFullPath = Path.Combine(directoryName, themeName);

            //method 1:
            //error:Get property 'System.Windows.Media.GradientBrush.GradientStops' threw an exception.
            //var newResourceDictionary = themeResource.Clone();
            //XamlServices.Save(themeFullPath), newResourceDictionary);

            //method 1.5:
            //XamlServices.Save(themeFullPath, newResourceDictionary);

            var newResourceDictionary = themeResource.CloneResource();
                XamlServices.Save(themeFullPath, newResourceDictionary);
         

            //method 1.75:
            //error:Current local value enumeration is outdated because one or more local values have been set since its creation
            //var newResourceDictionary = themeResource.CloneResource();
            //XamlServices.Save(themeFullPath, newResourceDictionary);

            //method 2:
            //var newResourceDictionary = new ResourceDictionary();
            //ApplyResource(themeResource, newResourceDictionary);
            //XamlServices.Save(themeFullPath, newResourceDictionary);

            //method 3:
            //var settings = new XmlWriterSettings { Indent = true };
            //var writer = XmlWriter.Create(themeFullPath, settings);
            //var wr = new StringWriter();
            //var newResourceDictionary = themeResource.CloneResource();
            //System.Windows.Markup.XamlWriter.Save(newResourceDictionary, writer);
            //Console.Write(wr.GetStringBuilder().ToString());
        }
    }
}
