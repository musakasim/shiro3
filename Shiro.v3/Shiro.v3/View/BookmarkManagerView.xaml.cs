using Shiro.View.ViewManagement;
using Shiro.ViewModel;

namespace Shiro.View
{
    /// <summary>
    /// Interaction logic for BookmarkManagerView.xaml
    /// </summary>
    public partial class BookmarkManagerView : Pencere
    {
        public BookmarkManagerViewModel BookmarkManagerViewModel { get; set; }

        public BookmarkManagerView()
        {
            InitializeComponent();
            BookmarkManagerViewModel = new BookmarkManagerViewModel();
            DataContext = BookmarkManagerViewModel;
        }

    }
}
