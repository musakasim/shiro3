using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Reactive.Bindings;
using Shiro.Library;
using Shiro.Library.Extensions;

namespace Shiro.ViewModel
{
    public class ThemeManagementViewModel : MainViewModel
    {
        #region bound property definitions

        public ReactiveProperty<BindableResourceDict> ResourceDictItems { get; set; }
        public ObservableCollection<object> UndoList { get; set; }
        public ReactiveProperty<DictionaryItem> SelectedDictItem { get; set; }
        //public ReactiveProperty<SolidColorBrush> SelectedBrush { get; set; }
        public ReactiveProperty<string> NewThemeName { get; set; }

        #endregion

        #region command definitions
        public RelayCommand LoadThemeResources { get; set; }

        /// <summary>
        /// save modified theme brushes as a new resource dict(xaml)
        /// </summary>
        public RelayCommand SaveBrushesCommand { get; set; }
        public RelayCommand ApplyBrushesCommand { get; set; }
        //public RelayCommand ApplyBrushCommand { get; set; }

        /// <summary>
        /// TreeView SelectedItem'i parametre olarak gonderilir, gelen degeri SelectedDictItem prop'una esitleyerek bind etkisi saglar
        /// </summary>
        public RelayCommand<DictionaryItem> TreeViewSelectedItemChangedCommand { get; set; }

        #endregion command definitions

        public DictionaryEntryViewModel DictionaryEntryViewModel { get; set; }
        public DictionaryViewModel DictionaryViewModel { get; set; }

        public ThemeManagementViewModel()
        {
            if (IsInDesignMode)
            {
                ResourceDictItems = new ReactiveProperty<BindableResourceDict>(DesignTimeData.BindableResourceDictionary1);
            }
            else
            {
                SelectedDictItem = new ReactiveProperty<DictionaryItem>();
                NewThemeName = new ReactiveProperty<string>("NewThemeName");

                //todo:Undo list gerçeklenecek, değişiklikler geri alınabilecek
                UndoList = new ObservableCollection<object>();
                ResourceDictItems = new ReactiveProperty<BindableResourceDict>();

                // TreeView SelectedItem'i parametre olarak gonderilir, gelen degeri SelectedDictItem prop'una esitleyerek bind etkisi saglar:
                TreeViewSelectedItemChangedCommand = new RelayCommand<DictionaryItem>(selectedDictItem =>
                {
                    SelectedDictItem.Value = selectedDictItem;
                });

                //SelectedBrush = SelectedDictItem.Select(t => t != null && t.Value is SolidColorBrush ? (SolidColorBrush)t.Value : null).ToReactiveProperty();

                LoadThemeResources = new RelayCommand(() =>
                {
                    ResourceDictionary themeResourceDictionary = ThemeManager.GetCurrentThemeResourceDictionary(Application.Current);
                    ResourceDictItems.Value = themeResourceDictionary.AsBindableObject();
                });

                SaveBrushesCommand = new RelayCommand(() =>
                {
                    var modifiedResrcDict = ResourceDictItems.Value.ToResourceDictionary();
                    string themeName = string.Format("{0}" + ".xaml", NewThemeName.Value);
                    ThemeManager.SaveTheme(modifiedResrcDict, themeName, null);
                    //todo: to be refactored
                    //SET AS USER THEME
                    ThemeManager.UserThemeUri = themeName;
                }, () => UndoList.Any()); // if undolist has an element then we may want to save brushes as a theme

                //apply all brushes
                ApplyBrushesCommand = new RelayCommand(() =>
                {
                    var modifiedResrcDict = ResourceDictItems.Value.ToResourceDictionary();
                    ThemeManager.ApplyResource(modifiedResrcDict, ResourceDictItems.Value.OriginalResourceDictionary);
                    UndoList.Add(new BindableResourceDict());
                });

                //todo: teker teker degisikliklerin uygulanabilmesi icin ApplyBrushCommand hazirlanacak
                //apply selected brush (used when a brush color is changed)
                //ApplyBrushCommand = new RelayCommand(() =>
                //{
                //    if (SelectedDictItem != null)
                //    {
                //        var asDict = SelectedDictItem.Value.Value as BindableResourceDict;
                //        if (asDict == null
                //            && SelectedDictItem.Value != null
                //            && !string.IsNullOrEmpty(SelectedDictItem.Value.Key))
                //        {
                //        }
                //    }
                //    //todo:undolist yapılınca burası silinecek
                //    UndoList.Add(new BindableResourceDict());
                //});
            }

            //todo: Designmode ve çalışmaMode'u için ViewModel yüklemeleri aynı şekilde olacağı için, LoadSubViewModels gibi bir method istiyorum!

            // in both  DesignMode and RunningMode we use static content, so this block should work for both modes
            // we want to see how the look would be when a color is changed
            // SET STATIC CONTENT to see styles in dictionary view(for both design and Running mode):
            //todo: need really static content here, some parts are still working
            DictionaryViewModel = new DictionaryViewModel(false);
        }
    }
}
