using System.Windows;

namespace Shiro.View
{
    //todo:IView yerine BaseView abstract class gelecek ve GetFramworkElementByName base class'da olacak, her türeyen class'da aynı kod var!!!!
    public interface IView
    {
        FrameworkElement GetFramworkElementByName(string elementName);
        void ApplyEventHandlings();
    }
}