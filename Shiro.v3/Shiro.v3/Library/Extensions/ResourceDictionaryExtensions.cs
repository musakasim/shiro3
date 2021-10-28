using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Shiro.Library.DeepCloning;

namespace Shiro.Library.Extensions
{
    /// <summary>
    /// Extension methods for the ResourceDictionary class.
    /// </summary>
    public static class ResourceDictionaryExtensions
    {
        /// <summary>
        /// Bu method  System.Windows.Controls.DataVisualization.Toolkit; 'da tanýmlýymýþ:P
        /// Makes a shallow copy of the specified ResourceDictionary.
        /// </summary>
        /// <param name="dictionary">ResourceDictionary to copy.</param>
        /// <returns>Copied ResourceDictionary.</returns>
        public static ResourceDictionary ShallowCopy(this ResourceDictionary dictionary)
        {
            var clone = new ResourceDictionary();
            foreach (var key in dictionary.Keys)
            {
                clone.Add(key, dictionary[key]);
            }
            return clone;
        }

        /// <summary>
        /// RECURSIVE!!!!!
        /// mergeddictionary@leri de kopyaliyor
        /// todo:test and compare with ShallowCopy
        /// </summary>
        /// <param name="sourceDictionary">ResourceDictionary to copy.</param>
        /// <returns>Cloned ResourceDictionary</returns>
        public static ResourceDictionary CloneResource(this ResourceDictionary sourceDictionary)
        {
            var destDict = new ResourceDictionary();
            //t.InjectFrom(sourceDictionary);
            foreach (var key in sourceDictionary.Keys)
            {
                destDict.Add(key, sourceDictionary[key]);
            }
            foreach (var mergedDictionary in sourceDictionary.MergedDictionaries)
            {
                var cloneResource = ShallowCopy(mergedDictionary);
                destDict.MergedDictionaries.Add(cloneResource);
            }
            return destDict;
        }

        /// <summary>
        /// Converts resource dictionary to a type which can be used to bind to treeview
        /// Also sorts the Keys
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static BindableResourceDict AsBindableObject(this ResourceDictionary dictionary)
        {
            var bindableObject = new BindableResourceDict { OriginalResourceDictionary = dictionary };
            
            //first merged dictionaries  
            for (int i = 0; i < dictionary.MergedDictionaries.Count; i++)
            {
                var md = dictionary.MergedDictionaries[i];
                var mdAsBindable = md.AsBindableObject();
                bindableObject.Items.Add(new DictionaryItem(i.ToString(CultureInfo.InvariantCulture), mdAsBindable));
            }

            foreach (var key in dictionary.Keys.Cast<string>().OrderBy(a => a))
            {
                var value = dictionary[key];
                bindableObject.Items.Add(new DictionaryItem (key, value));
            }
            return bindableObject;
        }

        /// <summary>
        /// reverts what AsBindableObject did, returns a new ResourceDictionary
        /// </summary>
        /// <param name="bindableObject"></param>
        /// <returns></returns>
        public static ResourceDictionary ToResourceDictionary(this BindableResourceDict bindableObject)
        {
            var resourceDictionary = new ResourceDictionary();

            foreach (var item in bindableObject.Items)
            {
                var dictionary = item.Value as BindableResourceDict;
                if (dictionary != null)
                    resourceDictionary.MergedDictionaries.Add(dictionary.ToResourceDictionary());
                else
                    resourceDictionary.Add(item.Key, item.Value);
            }

            return resourceDictionary;
        }
    }
}