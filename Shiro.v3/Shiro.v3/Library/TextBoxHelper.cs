using System.Windows;
using System.Windows.Controls;

namespace Shiro.Library
{
    /// <summary>
    /// 
    /// <xamlUsage>
    ///     <TextBox Text="{Binding Message}" u:TextBoxHelper.SelectedText="{Binding SelectedText}" />
    /// </xamlUsage>
    /// </summary>
    public static class TextBoxHelper
    {

        public static string GetSelectedText(DependencyObject obj)
        {
            return (string)obj.GetValue(SelectedTextProperty);
        }

        public static void SetSelectedText(DependencyObject obj, string value)
        {
            obj.SetValue(SelectedTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.RegisterAttached(
                "SelectedText",
                typeof(string),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedTextChanged));

        private static void SelectedTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var tb = obj as TextBox;
            if (tb != null)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    tb.SelectionChanged += tb_SelectionChanged;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    tb.SelectionChanged -= tb_SelectionChanged;
                }

                string newValue = e.NewValue as string;

                if (newValue != null && newValue != tb.SelectedText)
                {
                    tb.SelectedText = newValue;
                }
            }
        }

        static void tb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                SetSelectedText(tb, tb.SelectedText);
            }
        }

    }
}
