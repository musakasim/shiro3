using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Shiro.Library
{
    /// <summary>
    /// treeview'e bind ederken key value ile bind etmek ve MergedDictionary'lerde 
    /// nested bindingi sağlamak için ResourceDict için sunum şeklidir
    /// </summary>
    public class BindableResourceDict
    {
        public BindableResourceDict()
        {
            Items = new List<DictionaryItem>();
        }

        /// <summary>
        /// merged dictionaries
        /// solidcolorbrushes etc. will be placed in this list
        ///     fonts -><code>Tuple|string,font|</code>
        ///     solidcolorbrush -><code>Tuple|string,solidcolorbrush|</code>
        ///     ResourceDict -><code>BindableResourceDict</code>
        /// </summary>
        public List<DictionaryItem> Items { get; set; }

        public ResourceDictionary OriginalResourceDictionary { get; set; }
    }

    /// <summary>
    /// ResourceDictionary Item representation 
    ///     Value can be a resourceDict stands for any MergedDictionaries in the original resdict
    /// </summary>
    public class DictionaryItem
    {
        public DictionaryItem(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        /// <summary>
        /// either one of these:
        ///     fonts 
        ///     solidcolorbrush  
        ///     ResourceDict  >>>Means this is one of the MergedDictioanary
        /// </summary>
        public object Value { get; set; }
    }

    /// <summary>
    /// Used to select datatemplate when BindableResourceDict is bound to a treeview 
    /// </summary>
    public class TvSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DictionaryItem)
            {
                var dictionaryItem = item as DictionaryItem;
                if (dictionaryItem.Value is SolidColorBrush) return ((FrameworkElement)container).FindResource("Scbrush") as DataTemplate;
                if (dictionaryItem.Value is FontFamily) return ((FrameworkElement)container).FindResource("FontFamilyTemplate") as DataTemplate;
                if (dictionaryItem.Value is Color) return ((FrameworkElement)container).FindResource("ScColor") as DataTemplate;
                //todo: item.Value SolidColorBrush veya font veya her ne ise olması durumu için ayrı datatemplate'ler hazırlanacak
                if (dictionaryItem.Value is BindableResourceDict) return ((FrameworkElement)container).FindResource("ResDict") as HierarchicalDataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
