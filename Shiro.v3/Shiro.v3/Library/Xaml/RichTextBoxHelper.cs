using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Shiro.Library.Xaml
{
    /// <summary>
    /// Provides accessor for richtextboxes' Document data
    /// </summary>
    public class RichTextBoxHelper : DependencyObject
    {
        private static readonly HashSet<Thread> _recursionProtection = new HashSet<Thread>();

        public static readonly DependencyProperty DocumentXamlProperty = DependencyProperty.RegisterAttached(
            "DocumentXaml",
            typeof (string),
            typeof (RichTextBoxHelper),
            new FrameworkPropertyMetadata(
                "",
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (obj, e) =>
                {
                    if (_recursionProtection.Contains(Thread.CurrentThread))
                        return;

                    var richTextBox = (RichTextBox) obj;

                    // Parse the XAML to a document (or use XamlReader.Parse())
                    try
                    {
                        string documentXaml = GetDocumentXaml(richTextBox);
                        if (documentXaml == null)
                            return;
                        var stream = new MemoryStream(Encoding.UTF8.GetBytes(documentXaml));
                        var doc = (FlowDocument) XamlReader.Load(stream);

                        // Set the document
                        richTextBox.Document = doc;
                    }
                    finally
                    {
                        if (richTextBox.Document == null) richTextBox.Document = new FlowDocument();
                    }

                    // When the document changes update the source
                    richTextBox.TextChanged += (obj2, e2) =>
                    {
                        var richTextBox2 = obj2 as RichTextBox;
                        if (richTextBox2 != null)
                        {
                            SetDocumentXaml(richTextBox, XamlWriter.Save(richTextBox2.Document));
                        }
                    };
                }
                )
            );

        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string) obj.GetValue(DocumentXamlProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            _recursionProtection.Add(Thread.CurrentThread);
            obj.SetValue(DocumentXamlProperty, value);
            _recursionProtection.Remove(Thread.CurrentThread);
        }
    }
}