using System;
using System.Runtime.CompilerServices;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Reactive.Bindings;
using Shiro.Annotations;
using Shiro.Controller;
using Shiro.Library;
using Shiro.View;
using Shiro.View.ViewManagement;

namespace Shiro.ViewModel
{
    //todo: make this class BaseViewModel!? inherit MainViewModel and other viewmodels from baseViewModel
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    ///     Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    ///     You can also use Blend to data bind with the tool's support.
    ///     See http://www.galasoft.ch/mvvm
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        static MainViewModel()
        {
            if (IsInDesignModeStatic)
                return;

            BookmarkController = IoCContainer.Resolve<IBookmarkController>();
            TatoebaController = IoCContainer.Resolve<ITatoebaController>();
            ShiroDictionaryController = IoCContainer.Resolve<IShiroDictionaryController>();
            KanjiInfoController = IoCContainer.Resolve<IKanjiInfoController>();
            WritingProgressController = IoCContainer.Resolve<IWritingProgressController>();

            MainPanelManager = new PencereManager();
            MainPanelManager.AddNewPencereType(typeof(DictionaryView));
            MainPanelManager.AddNewPencereType(typeof(WritingStudyView));
            MainPanelManager.AddNewPencereType(typeof(DbPopulatorView));
            MainPanelManager.AddNewPencereType(typeof(ThemeManagementView));
            MainPanelManager.AddNewPencereType(typeof(BookmarkManagerView));
            MainPanelManager.AddNewPencereType(typeof(ResourceDictComparerView));

            ChangeThemeCommand = new RelayCommand(() => ThemeManager.ChangeTheme(new Uri(ThemeManager.FunnyThemeUri), Application.Current));
            //todo:add window manager an option to let multiple view model instances for each view
            ChangeToDictionary = new RelayCommand(() => MainPanelManager.ChangeToWindow(typeof(DictionaryView)), () => true);
            ChangeToWritingStudy = new RelayCommand(() => MainPanelManager.ChangeToWindow(typeof(WritingStudyView)), () => true);
            ChangeToDbPopulator = new RelayCommand(() => MainPanelManager.ChangeToWindow(typeof(DbPopulatorView)), () => true);
            ChangeToThemeManagement = new RelayCommand(() => MainPanelManager.ChangeToWindow(typeof(ThemeManagementView)), () => true);
            ChangeToResourceDictComparer = new RelayCommand(() => MainPanelManager.ChangeToWindow(typeof(ResourceDictComparerView)), () => true);
            ViewCommandManagerWindowCommand = new RelayCommand(() => MainPanelManager.ChangeToWindow(typeof(BookmarkManagerView)), () => true);

            MainPanelManager.ChangeToWindow(typeof(DictionaryView));
        }

        public MainViewModel()
        {
            if (IsInDesignMode)
                return;
            BoundView = new ReactiveProperty<IView>();
        }

        //todo: ViewModelCtor will be divided to two; in case of base.IsInDesigMode==true this method will be called, otherwise LoadViewModel or sth like InitiateviewModel will be called, which sould be implemented in derived class
        public virtual void LoadDesignTimeStaticData()
        {

        }

        protected static IBookmarkController BookmarkController { get; set; }
        protected static ITatoebaController TatoebaController { get; set; }
        protected static IShiroDictionaryController ShiroDictionaryController { get; set; }
        protected static IKanjiInfoController KanjiInfoController { get; set; }
        protected static IWritingProgressController WritingProgressController { get; set; }

        public static PencereManager MainPanelManager { get; set; }
        public static RelayCommand ChangeToDictionary { get; set; }
        public static RelayCommand ChangeToWritingStudy { get; set; }
        public static RelayCommand ChangeToDbPopulator { get; set; }
        public static RelayCommand ChangeToThemeManagement { get; set; }
        public static RelayCommand ChangeThemeCommand { get; set; }
        public static RelayCommand ChangeToResourceDictComparer { get; set; }
        public static RelayCommand CloseAppCommand { get; set; }
        public static RelayCommand MinimizeAppCommand { get; set; }
        public static RelayCommand ViewCommandManagerWindowCommand { get; set; }

        /// <summary>
        /// Provides reference to the view element which's DataContext is bound to this viewmodel
        /// !Must be set in the view
        /// With this object we can access directly any FrameworkElement in the view
        /// </summary>
        public ReactiveProperty<IView> BoundView { get; set; }

        #region INPC

        /// <summary>
        ///     This gives us the ReSharper option to transform an autoproperty into a property with change notification
        ///     Also leverages .net 4.5 callermembername attribute
        /// </summary>
        /// <param name="property">name of the property</param>
        [NotifyPropertyChangedInvocator]
        // ReSharper disable once OptionalParameterHierarchyMismatch
        protected override void RaisePropertyChanged([CallerMemberName] string property = "")
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            base.RaisePropertyChanged(property);
        }

        #endregion
    }
}