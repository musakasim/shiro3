using System.Collections.Generic;
using System.Linq;
using FrInterfaces;

namespace Shiro.Model
{
    /// <summary>
    ///     Reworked KanjiVg data
    /// </summary>
    public class KanjiGraph : IBaseModel
    {
        public string Element { get; set; }
        public string Unicode { get; set; }

        /// <summary>
        ///     Strokes at top level will be wrapped in to KGraph obj too
        /// </summary>
        public List<KGraph> Graphs { get; set; }

        /// <summary>
        ///     Graph elements in deep
        /// </summary>
        public List<string> Parts { get; set; }

        //Calculated Property
        public List<Stroke> Strokes
        {
            get { return Graphs.SelectMany(t => t.Strokes).OrderBy(l => l.Order).ToList(); }
        }

        //Calculated Property
        public List<string> PathDataList
        {
            get { return Strokes.OrderBy(l => l.Order).Select(u => u.Data).ToList(); }
        }

        //Calculated Property
        public int StrokeCount
        {
            get { return Strokes.Count; }
        }

        public int Id { get; set; }

        public class KGraph
        {
            public string Element { get; set; }
            public string Unicode { get; set; }
            public List<Stroke> Strokes { get; set; }
        }

        public class Stroke
        {
            public int Order { get; set; }
            public string Data { get; set; }
        }
    }
}