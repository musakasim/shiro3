using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Shiro.Converter;
using Shiro.Model;

namespace Shiro.Library
{
    public static class PrintingUtils
    {

        /// <summary>
        /// </summary>
        /// <param name="flowDocument"></param>
        /// <returns>
        /// returns if printing process completed all the way to end,
        /// false return value should be considered as printing is interrupted by user
        /// </returns>
        public static bool Print(FlowDocument flowDocument)
        {
            // Serialize RichTextBox content into a stream in Xaml or XamlPackage format. (Note: XamlPackage format isn't supported in partial trust.)
            var sourceDocument = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
            var stream = new MemoryStream();
            sourceDocument.Save(stream, DataFormats.Xaml);

            // Clone the source document's content into a new FlowDocument.
            var flowDocumentCopy = new FlowDocument();
            var copyDocumentRange = new TextRange(flowDocumentCopy.ContentStart, flowDocumentCopy.ContentEnd);
            copyDocumentRange.Load(stream, DataFormats.Xaml);

            // Create a XpsDocumentWriter object, open a Windows common print dialog.
            // This methods returns a ref parameter that represents information about the dimensions of the printer media.
            PrintDocumentImageableArea ia = null;
            //Dont be scared if Debugger hits this, it  hits inner try-catch Block, just continue
            XpsDocumentWriter docWriter = PrintQueue.CreateXpsDocumentWriter(ref ia);

            if (docWriter != null && ia != null)
            {
                DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocumentCopy).DocumentPaginator;

                // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
                paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
                Thickness pagePadding = flowDocumentCopy.PagePadding;
                flowDocumentCopy.PagePadding = new Thickness(
                    Math.Max(ia.OriginWidth, pagePadding.Left),
                    Math.Max(ia.OriginHeight, pagePadding.Top),
                    Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), pagePadding.Right),
                    Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), pagePadding.Bottom));
                flowDocumentCopy.ColumnWidth = double.PositiveInfinity;

                // Send DocumentPaginator to the printer.
                docWriter.Write(paginator);
                return true;
            }
            return false;
        }

        public static IEnumerable<string> GetFileNames(string selectedFolder)
        {
            return Directory.GetFiles(selectedFolder).Select(Path.GetFileName);
        }

        /// <summary>
        ///     Draws a rounded rectangle with four individual corner radius
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")] //https://stackoverflow.com/a/3839419/1538014 lanet olasıca warning! using kullandık diye iftira ediliyor!
        public static void DrawRoundedRectangle(this DrawingContext dc, Brush brush, Pen pen, Rect rect,
            CornerRadius cornerRadius)
        {
            var geometry = new StreamGeometry();
            using (StreamGeometryContext context = geometry.Open())
            {
                bool isStroked = pen != null;
                const bool isSmoothJoin = true;

                context.BeginFigure(rect.TopLeft + new Vector(0, cornerRadius.TopLeft), brush != null, true);
                context.ArcTo(new Point(rect.TopLeft.X + cornerRadius.TopLeft, rect.TopLeft.Y),
                    new Size(cornerRadius.TopLeft, cornerRadius.TopLeft),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.TopRight - new Vector(cornerRadius.TopRight, 0), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.TopRight.X, rect.TopRight.Y + cornerRadius.TopRight),
                    new Size(cornerRadius.TopRight, cornerRadius.TopRight),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.BottomRight - new Vector(0, cornerRadius.BottomRight), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.BottomRight.X - cornerRadius.BottomRight, rect.BottomRight.Y),
                    new Size(cornerRadius.BottomRight, cornerRadius.BottomRight),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.BottomLeft + new Vector(cornerRadius.BottomLeft, 0), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.BottomLeft.X, rect.BottomLeft.Y - cornerRadius.BottomLeft),
                    new Size(cornerRadius.BottomLeft, cornerRadius.BottomLeft),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);

                context.Close();

            }
            dc.DrawGeometry(brush, pen, geometry);
        }


        public static void VisualToXps(object obj)
        {
            if ((obj as IEnumerable<StrokeOrderGraphList>) != null)
            {
                List<StrokeOrderGraphList> strokePathGraphics = (obj as IEnumerable<StrokeOrderGraphList>).ToList();

                var container = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(100) };
                //Buradaki margin ile print sayfasında kenar boşluğu sağlanıyor
                foreach (StrokeOrderGraphList graphic in strokePathGraphics)
                {
                    var content = new WrapPanel { Orientation = Orientation.Horizontal };

                    content.Children.Add(new TextBlock { Text = graphic.GraphText, FontSize = 20 });
                    while (graphic.Count > 0)
                    {
                        //todo:Kesinlikle yap-blogla CloneBoundViualProperty (Blog: Working with bound visual elements) gibi guvenlik saglayan bir metod yap (hatta using içinde geçici süreç yonetsin)
                        //to avoid error:Specified Visual is already a child of another Visual or the root of the component target
                        var canvas = graphic.First();
                        var border = canvas as Border;
                        graphic.Remove(canvas);
                        content.Children.Add(canvas);
                    }
                    container.Children.Add(content);
                }
                //Window window = Application.Current.MainWindow;
                //var template = window.FindResource("StrokeOrderPrintTemplate") as DataTemplate;
                //var container = new ListView();
                //container.ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));
                //container.ItemTemplate = template;
                //if (strokePathGraphics.Count > 0) container.ItemsSource = strokePathGraphics[0];

                VisualToXpsWithPrintDialog(container);

                //a.ForEach(strokePathGraphics.Add);
                //a.Clear();
            }
            else if ((obj as ObservableCollection<KanjiInfo>) != null)
            {
            }
        }

        public static void VisualToXps(string fileName, Visual visual)
        {
            Package package = Package.Open(fileName, FileMode.Create);
            {
                var doc = new XpsDocument(package);
                {
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                    writer.Write(visual);
                }
                doc.Close();
            }
            package.Close();
        }

        public static void VisualToXpsWithPrintDialog(Visual visual)
        {
            PrintDocumentImageableArea ia = null;
            XpsDocumentWriter docWriter = PrintQueue.CreateXpsDocumentWriter(ref ia);
            if (docWriter != null && ia != null)
            {
                docWriter.Write(visual);
            }
            //var printDialog = new PrintDialog();

            //if (printDialog.ShowDialog() != true)
            //{
            //    return;
            //}
            //PrintQueue pq = printDialog.PrintQueue;
            //Visual scaledVisual = ScaleVisual(visual, pq);

            //XpsDocumentWriter xpsdw = PrintQueue.CreateXpsDocumentWriter(pq);

            //xpsdw.Write(scaledVisual);
        }


        public static void PrintWithPrintDialog(FlowDocument flowDocument)
        {
            //PrintingUtils.VisualToXps("deneme.xps", rtxtCalismaYazisi.Document);

            var dlg = new PrintDialog { PageRangeSelection = PageRangeSelection.AllPages, UserPageRangeEnabled = true };

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Print document
                //dlg.PrintVisual(rtxtCalismaYazisi as Visual, "printing as visual");
                DocumentPaginator documentPaginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
                dlg.PrintDocument(documentPaginator, "printing as paginator");
            }
        }

        private static Visual ScaleVisual(Visual v, PrintQueue pq)
        {
            var root = new ContainerVisual();
            //const double inch = 96;

            const double xMargin = 150;
            const double yMargin = 100;

            PrintTicket pt = pq.UserPrintTicket;
            double printableWidth = pt.PageMediaSize.Width.Value;
            double printableHeight = pt.PageMediaSize.Height.Value;
            Console.WriteLine(printableWidth);
            Console.WriteLine(printableHeight);

            const double xScale = 2;
            const double yScale = 3;

            root.Children.Add(v);
            root.Transform = new MatrixTransform(xScale, 0, 0, yScale, xMargin, yMargin);

            return root;
        }
    }
}