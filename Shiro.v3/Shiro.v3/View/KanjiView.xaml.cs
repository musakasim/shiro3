using System.Windows;
using System.Windows.Controls;
using Shiro.ViewModel;

namespace Shiro.View
{
    /// <summary>
    ///     Interaction logic for KanjiView.xaml
    /// </summary>
    public partial class KanjiView : UserControl, IView
    {
        public KanjiView()
        { 
            InitializeComponent();
            ApplyEventHandlings();
        }

        public void ApplyEventHandlings()
        {
            DataContextChanged += (sender, args) =>
            {
                var kanjiViewModel = DataContext as KanjiViewModel;
                if (kanjiViewModel != null && kanjiViewModel.BoundView != null)
                    kanjiViewModel.BoundView.Value = this;
            };
        }

        public FrameworkElement GetFramworkElementByName(string elementName)
        {
            var framworkElementByName = (FrameworkElement) FindName(elementName);
            return framworkElementByName;
        }
    }
}