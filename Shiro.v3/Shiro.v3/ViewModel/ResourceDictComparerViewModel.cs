using System.Collections.Generic;
using System.Reactive;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using Reactive.Bindings;
using Shiro.Library;

namespace Shiro.ViewModel
{
    public class ResourceDictComparerViewModel : MainViewModel
    {
        public ReactiveCollection<KeyValuePair<string, SolidColorBrush>> ResourceDictPairs1 { get; set; }
        public ReactiveCollection<KeyValuePair<string, SolidColorBrush>> ResourceDictPairs2 { get; set; }

        private ResourceDictionaryComparer ResourceDictionaryComparer { get; set; }

        public ReactiveCollection<KeyValuePair<string, string>> ThemeFiles { get; set; }
        public ReactiveProperty<KeyValuePair<string, string>> SelectedThemeFileUri { get; set; }

        public RelayCommand CompareCommand { get; set; }

        public ResourceDictComparerViewModel()
        {
            if (IsInDesignMode)
            {
            }
            else
            {
                ThemeFiles = new ReactiveCollection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("UserThemeUri", ThemeManager.UserThemeUri),
                    new KeyValuePair<string, string>("DefaultThemeUri", ThemeManager.DefaultThemeUri),
                    new KeyValuePair<string, string>("FunnyThemeUri", ThemeManager.FunnyThemeUri)
                };
                SelectedThemeFileUri = new ReactiveProperty<KeyValuePair<string, string>>(ThemeFiles[0]);

                ResourceDictPairs1 = new ReactiveCollection<KeyValuePair<string, SolidColorBrush>>();
                ResourceDictPairs2 = new ReactiveCollection<KeyValuePair<string, SolidColorBrush>>();

                SelectedThemeFileUri.Subscribe(new AnonymousObserver<KeyValuePair<string, string>>(a => Compare(a.Value)));

                CompareCommand = new RelayCommand(() => Compare(SelectedThemeFileUri.Value.Value));
            }
        }

        private void Compare(string secondResourceDictUri)
        {
            var themeResourceDictionary = ThemeManager.GetCurrentThemeResourceDictionary(Application.Current);
            var resourcDictFromFile = ThemeManager.LoadResource(secondResourceDictUri);
            ResourceDictionaryComparer = new ResourceDictionaryComparer(themeResourceDictionary,
                resourcDictFromFile);

            var compare = ResourceDictionaryComparer.Compare();
            ResourceDictPairs1.Clear();
            ResourceDictPairs2.Clear();
            compare.Item1.ForEach(a => ResourceDictPairs1.Add(a));
            compare.Item2.ForEach(a => ResourceDictPairs2.Add(a));
        }
    }
}
