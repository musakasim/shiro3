using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Shiro.Library
{
    public class ResourceDictionaryComparer
    {
        public ResourceDictionary Dict1 { get; set; }
        public ResourceDictionary Dict2 { get; set; }

        public ResourceDictionaryComparer(ResourceDictionary dict1, ResourceDictionary dict2)
        {
            Dict1 = dict1;
            Dict2 = dict2;
        }

        public Tuple<List<KeyValuePair<string, SolidColorBrush>>, List<KeyValuePair<string, SolidColorBrush>>> Compare()
        {
            var list1 = new List<KeyValuePair<string, SolidColorBrush>>();
            var list2 = new List<KeyValuePair<string, SolidColorBrush>>();
            var dict1Brushes = GetBrushesInResourceDictionary(Dict1).OrderBy(x => x.Key).ToList();
            var dict2Brushes = GetBrushesInResourceDictionary(Dict2).OrderBy(x => x.Key).ToList();

            for (int i = 0; i < Dict1.Count; i++)
            {
                var pairInFirstDict = dict1Brushes[i];
                var existsInSecDict = Dict2[pairInFirstDict.Key] != null;
                list1.Add(new KeyValuePair<string, SolidColorBrush>(pairInFirstDict.Key, pairInFirstDict.Value));

                //add other list same item if matches or add an empty record
                if (existsInSecDict)
                    list2.Add(new KeyValuePair<string, SolidColorBrush>(pairInFirstDict.Key, pairInFirstDict.Value));
                else
                    list2.Add(new KeyValuePair<string, SolidColorBrush>("", new SolidColorBrush(Colors.White)));

            }

            //we checked all first list items, so this time only those not exist in first list will be added to second list, and an empty counterpart to fisrtlist
            for (int j = 0; j < Dict2.Count; j++)
            {
                var pairInSecondDict = dict2Brushes[j];
                var existInFirstDict = Dict1[pairInSecondDict.Key] != null;
                if (!existInFirstDict)
                {
                    list2.Add(new KeyValuePair<string, SolidColorBrush>(pairInSecondDict.Key, pairInSecondDict.Value));
                    list1.Add(new KeyValuePair<string, SolidColorBrush>("", new SolidColorBrush(Colors.White)));
                }
            }
            return new Tuple<List<KeyValuePair<string, SolidColorBrush>>, List<KeyValuePair<string, SolidColorBrush>>>(list1, list2);
        }

        /// <summary>
        /// returns all SolidColorBrushes in a resourceDictory, recurses through MergedDictionaries
        /// </summary>
        /// <param name="resourceDictionary"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, SolidColorBrush>> GetBrushesInResourceDictionary(ResourceDictionary resourceDictionary)
        {
            foreach (DictionaryEntry r in resourceDictionary)
            {
                if (r.Value.GetType() == typeof(ResourceDictionary))
                    foreach (DictionaryEntry rv in (ResourceDictionary)r.Value)
                        foreach (var tuple in EnumerateSolidColorBrushes(rv)) yield return tuple;
                else
                    foreach (var tuple in EnumerateSolidColorBrushes(r)) yield return tuple;
            }
            foreach (var mergedDict in resourceDictionary.MergedDictionaries)
            {
                foreach (var themeBrush in GetBrushesInResourceDictionary(mergedDict))
                    yield return themeBrush;
            }
        }

        /// <summary>
        /// returns a keyvalue pair for DictionaryEntry if entry value is a SolidColorBrush
        /// todo: soru: neden sadece solidcolorbrush?
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<string, SolidColorBrush>> EnumerateSolidColorBrushes(DictionaryEntry r)
        {
            var brush = r.Value as SolidColorBrush;
            if (brush != null)
                yield return new KeyValuePair<string, SolidColorBrush>(r.Key.ToString(), brush);
        }
    }
}
