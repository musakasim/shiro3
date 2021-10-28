using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xaml;
using NUnit.Framework;
using Shiro.Library;

namespace Shiro.v3.Tests
{
    [TestFixture]
    public class ThemeManagerTests
    {
        private const string ThemesDirectoryName = @"Themes";

        [Test]
        public void FindAvailableThemes_LoadsAllAvailableXamlFilesUnderThemeDirectory()
        {
            //Preperations
            Directory.Delete(ThemesDirectoryName, true);
            Directory.CreateDirectory(ThemesDirectoryName);
            using (File.Create(@"Themes\Theme1.xaml"))
            using (File.Create(@"Themes\Theme2.xaml")) { }

            //Act
            var themeManager = new ThemeManager();
            themeManager.FindAvailableThemes();

            //Assert
            Assert.AreEqual(2, themeManager.Themes.Count);
            Assert.IsTrue(themeManager.Themes.Any(t => t.Name == "Theme1.xaml"));
            Assert.IsTrue(themeManager.Themes.Any(t => t.Name == "Theme2.xaml"));

            Directory.Delete("Themes", true);
        }

        [Test]
        public void ResourceDictionariesCanBeParsedWithXamlServiceParse()
        {
            //Preperations
            //Act
            var themeManager = new ThemeManager();
            themeManager.FindAvailableThemes();

            //little assert
            Assert.IsTrue(themeManager.Themes[0].Name == "TestTheme.xaml");

            var rd = (ResourceDictionary)XamlServices.Load(themeManager.Themes[0].FullName);
            Assert.AreEqual(32, rd.Count);
        }

        [Test]
        public void SaveTheme_ProducesNewResourceDictionaryFile()
        {
            //Preperations
            //Act
            var themeManager = new ThemeManager();
            themeManager.FindAvailableThemes();

            //little assert
            Assert.IsTrue(themeManager.Themes.Any(t => t.Name == "TestTheme.xaml"));

            // ReSharper disable once PossibleNullReferenceException
            //var task = TaskExtensions.StartSTATask(() =>
            //{
                var rd = (ResourceDictionary)XamlServices.Load(themeManager.Themes.FirstOrDefault(t => t.Name == "TestTheme.xaml").FullName);
                ThemeManager.SaveTheme(rd, "tttt2.xaml", AppDomain.CurrentDomain.BaseDirectory);
                //return "tamamlandi";
            //});
            //var result = task.Result;
            //Assert.AreEqual("tamamlandi", result);
            Assert.IsTrue(new DirectoryInfo(ThemesDirectoryName).GetFiles().Any(f => f.Name == "tttt2.xaml"));
        }


        [Test]
        [Ignore("Not Implemented!")]
        public void ApplyResourceChangesValuesWithNewOnes()
        {

        }

        [Test]
        [Ignore("Not Implemented!")]
        public void ApplyResourceChangesValuesForNestedDictionariesToo()
        {

        }

        [Test]
        [Ignore("Not Implemented!")]
        public void ApplyResourceChangesValuesForMergedDictionariesToo()
        {

        }
    }
}
