using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Shiro.Converter
{
    /// <summary>
    /// Draws path datas incremenatally, by adding one to total  each step
    /// on every step last drawn ppath is emphasized
    /// </summary>
    [ValueConversion(typeof(List<string>), typeof(string))]
    public class PathDataListToStrokeOrderGraphListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = value as List<string>;
            if (list == null || !list.Any())
                return null;
            StrokeOrderGraphList.SetAppTheme();
            var graphList = new StrokeOrderGraphList("", list);
            return graphList.GetStrokeOrderGraph();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StrokeOrderGraphList : List<UIElement> //every UIElement contains Border->Canvas->List<Path>  
    {
        public StrokeOrderGraphList(string stringOfGraph, List<string> pathData)
        {
            StringOfGraph = stringOfGraph;
            PathData = pathData;
        }

        static StrokeOrderGraphList()
        {
            SetAppTheme();
            //SetPrintTheme1();
            //SetPrintTheme(); 
        }

        public List<string> PathData { get; set; }
        public string StringOfGraph { get; set; }
        public string GraphText { get; set; }
        public static double ScaleRatio { get; set; }
        private static double StartPointStrokeRadius { get; set; }

        private static int ElementWidth
        {
            get { return (int)(125 * ScaleRatio); }
        }

        private static double StrokeThickness { get; set; }
        private static TransformGroup TransformGroup { get; set; }
        private static Thickness BorderThickness { get; set; }
        private static SolidColorBrush StrokeStartPointIndicatorBrush { get; set; }
        private static SolidColorBrush BorderBrush { get; set; }
        private static SolidColorBrush BackgroundBrush { get; set; }
        private static SolidColorBrush BlackBrush { get; set; }
        private static SolidColorBrush FadedBrush { get; set; }


        private static void SetTransformations()
        {
            TransformGroup = new TransformGroup();
            //var tt = new TranslateTransform(itemWidth * strokeIndex, 0);//(wrap panel gibi bir şey kullanılmazsa üst üste gelememeleri için itemleri kaydırır
            //transformGroup.Children.Add(tt);
            var sp = new ScaleTransform(ScaleRatio, ScaleRatio);
            TransformGroup.Children.Add(sp);
        }

        public static void SetAppTheme()
        {
            ScaleRatio = 0.4;
            StrokeThickness = 1.5;
            StartPointStrokeRadius = 1.5;
            BorderThickness = new Thickness(0.5);
            BlackBrush = new SolidColorBrush { Color = Colors.Black };
            FadedBrush = new SolidColorBrush { Color = Colors.AliceBlue };
            BackgroundBrush = new SolidColorBrush(Colors.SteelBlue);
            BorderBrush = new SolidColorBrush(Colors.Silver);
            StrokeStartPointIndicatorBrush = new SolidColorBrush(Colors.Red);
            SetTransformations();
        }

        public static void SetPrintTheme1()
        {
            ScaleRatio = 0.3;
            StrokeThickness = 1.2;
            StartPointStrokeRadius = 1.0;
            BorderThickness = new Thickness(0.2);
            BlackBrush = new SolidColorBrush { Color = Colors.Black };
            FadedBrush = new SolidColorBrush { Color = Colors.DimGray };
            BackgroundBrush = new SolidColorBrush(Colors.Transparent);
            BorderBrush = new SolidColorBrush(Colors.Silver);
            StrokeStartPointIndicatorBrush = new SolidColorBrush(Colors.Red);
            SetTransformations();
        }

        public static void SetPrintTheme()
        {
            ScaleRatio = 0.2;
            StrokeThickness = 1.0;
            StartPointStrokeRadius = 0.8;
            BorderThickness = new Thickness(0.2);
            BlackBrush = new SolidColorBrush { Color = Colors.Black };
            FadedBrush = new SolidColorBrush { Color = Colors.LightGray };
            BackgroundBrush = new SolidColorBrush(Colors.Transparent);
            BorderBrush = new SolidColorBrush(Colors.Silver);
            StrokeStartPointIndicatorBrush = new SolidColorBrush(Colors.Red);
            SetTransformations();
        }

        public static Path GetEllipseAtStarting(Path path)
        {
            Point startPoint = GetStartPosition(path);
            var ellipse = new EllipseGeometry
            {
                Center = new Point(startPoint.X, startPoint.Y),
                RadiusX = StartPointStrokeRadius,
                RadiusY = StartPointStrokeRadius
            };
            var dotPath = new Path
            {
                Data = ellipse,
                Stroke = StrokeStartPointIndicatorBrush,
                Fill = StrokeStartPointIndicatorBrush
            };
            return dotPath;
        }

        private static Point GetStartPosition(Path pathData)
        {
            return pathData.Data.GetFlattenedPathGeometry().Figures[0].StartPoint;
        }

        /// <summary>
        ///     Her adımda bir stroke çizimi ekleyerek tüm strokeların çizimlerini canvaslar içinde hazırlar
        /// Draws path datas incremenatally, by adding one to total  each step
        /// on every step last drawn ppath is emphasized
        /// 
        /// puts drawings in canvases  
        /// </summary>
        /// <returns>canvas list which contains drawn data</returns>
        public StrokeOrderGraphList GetStrokeOrderGraph()
        {
            GraphText = StringOfGraph;
            if (PathData.Any())
            {
                List<string> pathDatas = PathData;

                for (int strokeIndex = 1; strokeIndex <= pathDatas.Count(); strokeIndex++)
                {
                    var border = new Border { BorderThickness = BorderThickness, BorderBrush = BorderBrush };
                    var canvas = new Canvas { Background = BackgroundBrush, Width = ElementWidth, Height = ElementWidth };
                    //todo:alttaki satırda çizgiler sıralı gelmezse hata olur, "StrokePath.Order < strokeIndex" olacak sekilde sorgulanmalı(StrokePath.Order işlevsel olunca)
                    List<string> pathDatasToStrokeIndex = pathDatas.Take(strokeIndex).ToList();
                    List<Path> paths = GetPathGraphic(pathDatasToStrokeIndex, StrokeThickness, FadedBrush,
                        TransformGroup);
                    paths.ForEach(a => canvas.Children.Add(a));

                    Path lastStroke = paths.Last();
                    EmphasizeStroke(lastStroke, canvas);

                    border.Child = canvas;
                    // ReSharper disable once RedundantThisQualifier
                    this.Add(border);
                }
            }
            return this;
        }

        /// <summary>
        ///     canvas içindeki stroke grafiğini belirginleştirir, siyaha boyar ve başlangıç noktasına kırmızı nokta ekler
        /// </summary>
        private static void EmphasizeStroke(Path lastStroke, Canvas canvas)
        {
            Path ellipseAtStarting = GetEllipseAtStarting(lastStroke);
            canvas.Children.Add(ellipseAtStarting);
            lastStroke.Stroke = BlackBrush;
        }

        private static List<Path> GetPathGraphic(IEnumerable<string> geometryDatas, double strokeThickness,
            Brush solidColorBrush, TransformGroup transformGroup)
        {
            var paths = new List<Path>();
            foreach (string geometryData in geometryDatas)
            {
                Geometry geometry = GetGeometry(geometryData, transformGroup);
                //Make a new Path with transformed geometry
                var path = new Path { Data = geometry, StrokeThickness = strokeThickness, Stroke = solidColorBrush };
                paths.Add(path);
            }
            return paths;
        }

        private static Geometry GetGeometry(string geometryData, TransformGroup transformGroup)
        {
            //apply transformations to geometry
            Geometry geometry = Geometry.Parse(geometryData);
            Geometry inputGeometryClone = geometry.Clone();
            // we need a clone since in order to apply a Transform and geometry might be readonly
            inputGeometryClone.Transform = transformGroup;
            geometry = inputGeometryClone.GetFlattenedPathGeometry();
            return geometry;
        }
    }
}