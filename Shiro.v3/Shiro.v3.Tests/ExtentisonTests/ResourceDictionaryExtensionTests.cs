using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using NUnit.Framework;
using Shiro.Library;
using Shiro.Library.Extensions;

namespace Shiro.v3.Tests.ExtentisonTests
{
    [TestFixture]
    public class ResourceDictionaryExtensionTests
    {

        [Test]
        public void GettingResourceDictionary_AsBindableObject_NestsMergedDictionariesAndAllData()
        {
            var rdRoot = new ResourceDictionary();
            rdRoot.Add("mavi", new SolidColorBrush(Colors.Blue));
            rdRoot.Add("yesil", new SolidColorBrush(Colors.Green));
            {
                var rdMerged1 = new ResourceDictionary();
                rdMerged1.Add("kirmizi", new SolidColorBrush(Colors.Red));
                rdMerged1.Add("mor", new SolidColorBrush(Colors.Purple));
                rdRoot.MergedDictionaries.Add(rdMerged1);
            }

            BindableResourceDict bindableResourceDict = rdRoot.AsBindableObject();
            Assert.AreEqual(3, bindableResourceDict.Items.Count);

            var keyValuePair1 = bindableResourceDict.Items[0];
            Assert.AreEqual("yesil", keyValuePair1.Key);
            Assert.AreEqual(new SolidColorBrush(Colors.Green).Color, ((SolidColorBrush)keyValuePair1.Value).Color);

            var keyValuePair2 = bindableResourceDict.Items[1];
            Assert.AreEqual("mavi", keyValuePair2.Key);
            Assert.AreEqual(new SolidColorBrush(Colors.Blue).Color, ((SolidColorBrush)keyValuePair2.Value).Color);


            var bindableMergedDict1 = (BindableResourceDict)bindableResourceDict.Items[2].Value;
            Assert.AreEqual(2, bindableMergedDict1.Items.Count);
            var keyValuePair3 = bindableMergedDict1.Items[0];
            Assert.AreEqual("mor", keyValuePair3.Key);
            Assert.AreEqual(new SolidColorBrush(Colors.Purple).Color, ((SolidColorBrush)keyValuePair3.Value).Color);
        }

        [Test]
        public void ToResourceDictionary_ConvertsBindableObject_ToResourceDictionary()
        {
            var rdRoot = new ResourceDictionary();
            rdRoot.Add("mavi", new SolidColorBrush(Colors.Blue));
            rdRoot.Add("yesil", new SolidColorBrush(Colors.Green));
            {
                var rdMerged1 = new ResourceDictionary();
                rdMerged1.Add("kirmizi", new SolidColorBrush(Colors.Red));
                rdMerged1.Add("mor", new SolidColorBrush(Colors.Purple));
                rdRoot.MergedDictionaries.Add(rdMerged1);
            }

            BindableResourceDict bindableResourceDict = rdRoot.AsBindableObject();

            ResourceDictionary resourceDictionary = bindableResourceDict.ToResourceDictionary();

            Assert.AreEqual(2,resourceDictionary.Count);
            Assert.AreEqual(1,resourceDictionary.MergedDictionaries.Count);
            Assert.AreEqual(2,resourceDictionary.MergedDictionaries[0].Count);
            Assert.AreEqual(new SolidColorBrush(Colors.Blue).Color, ((SolidColorBrush)resourceDictionary["mavi"]).Color);
            Assert.AreEqual(new SolidColorBrush(Colors.Red).Color, ((SolidColorBrush)resourceDictionary.MergedDictionaries[0]["kirmizi"]).Color);
        }

        [Test]
        public void ToResourceDictionary_KeepsOrderOfMergedDictinaries()
        {
            Assert.Fail();
        }
 
    }
}
