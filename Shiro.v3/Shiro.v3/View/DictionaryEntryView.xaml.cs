using System.Windows.Controls;

namespace Shiro.View
{
    /// <summary>
    ///     Interaction logic for DictionaryEntryView.xaml
    /// </summary>
    public partial class DictionaryEntryView : UserControl
    {
        /// <summary>
        ///     this usercontrol will be used in other controls, and datacontext will be decended from them
        ///     DataContext must be DictionaryEntryViewModel
        /// </summary>
        public DictionaryEntryView()
        {
            InitializeComponent();
        }
    }
}