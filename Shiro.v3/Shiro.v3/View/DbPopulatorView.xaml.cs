using System;
using Shiro.View.ViewManagement;
using Shiro.ViewModel;

namespace Shiro.View
{
    /// <summary>
    ///     Interaction logic for DbPopulatorView.xaml
    /// </summary>
    public partial class DbPopulatorView : Pencere
    {
        public DbPopulatorView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            //todo: bu şekilde OnInitialized içinde viewmodel atanırsa, base class içinde genel yapı hazırlanıp
            // IoC ile ilgili viewmodel oluşturulup datacontext'e atanabilir. (Gereksiz karmaşıklık mı olur?)
            // see: MvvmLight.ViewModelLocator
            ViewModel = new DbPopulatorViewModel();
            DataContext = ViewModel;
        }

        public DbPopulatorViewModel ViewModel { get; set; }
    }
}