using System.Windows;
using Shiro.View.ViewManagement;
using Shiro.ViewModel;

namespace Shiro.View
{
    /// <summary>
    /// Interaction logic for WritingStudyView.xaml
    /// </summary>
    public partial class WritingStudyView : Pencere, IView
    {
        public WritingStudyViewModel WritingStudyViewModel { get; set; }
        public WritingStudyView()
        {
            InitializeComponent();
            WritingStudyViewModel = new WritingStudyViewModel();
            ApplyEventHandlings();
            DataContext = WritingStudyViewModel;
        }

        public void ApplyEventHandlings()
        {
            DataContextChanged += (sender, args) =>
            {
                var viewModel = DataContext as WritingStudyViewModel;
                if (viewModel != null && viewModel.BoundView != null)
                    viewModel.BoundView.Value = this;
            };
        }

        public FrameworkElement GetFramworkElementByName(string elementName)
        {
            var framworkElementByName = (FrameworkElement)FindName(elementName);
            return framworkElementByName;
        }
    }
}
