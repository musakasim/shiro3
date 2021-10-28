using System.Windows;
using System.Windows.Input;

namespace Shiro.Library
{
    public static class FocusAdvancement
    {
        public static readonly DependencyProperty AdvancesByEnterKeyProperty =
            DependencyProperty.RegisterAttached("AdvancesByEnterKey", typeof (bool), typeof (FocusAdvancement),
                new UIPropertyMetadata(OnAdvancesByEnterKeyPropertyChanged));

        public static bool GetAdvancesByEnterKey(DependencyObject obj)
        {
            return (bool) obj.GetValue(AdvancesByEnterKeyProperty);
        }

        public static void SetAdvancesByEnterKey(DependencyObject obj, bool value)
        {
            obj.SetValue(AdvancesByEnterKeyProperty, value);
        }

        private static void OnAdvancesByEnterKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            if (element == null) return;

            if ((bool) e.NewValue) element.KeyDown += Keydown;
            else element.KeyDown -= Keydown;
        }

        //todo:textbox a enter için inputbinding tanımlanmışsa burası çalışmıyor?!! çözüm?
        private static void Keydown(object sender, KeyEventArgs e)
        {
            if (!e.Key.Equals(Key.Enter)) return;

            var element = sender as UIElement;
            if (element != null) element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}