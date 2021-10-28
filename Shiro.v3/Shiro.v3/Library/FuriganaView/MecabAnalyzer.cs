using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NMeCab;

namespace Shiro.Library.FuriganaView
{
    public class MecabAnalyzer
    {
        static MeCabTagger Tagger { get; set; }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="textToAnalyze"></param>
        /// <param name="outPutFormat"></param>
        /// <param name="retryIfErrorOccurs">When an error occurs (memory overflow is common) i want to retry once more, this will indicate this</param>
        /// <returns></returns>
        public static string AnalyzeString(string textToAnalyze, string outPutFormat = null)//, bool retryIfErrorOccurs = true)
        {
            outPutFormat = outPutFormat ?? OutPutFormats.Wakati;
            InitiateTaggerObject(outPutFormat);

            try
            {
                return Tagger.Parse(textToAnalyze);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.ToString(), "ERROR");
                //if (retryIfErrorOccurs)
                //{
                //    Task.Delay(1000);
                //    return AnalyzeString(textToAnalyze, outPutFormat, false); //retry once more possibly success second time
                //}
                return "";
            }
        }

        private static void InitiateTaggerObject(string outPutFormat)
        {
            if (Tagger == null)
                Tagger = MeCabTagger.Create(); //this line cause memory overflow sometimes
            if (Tagger.OutPutFormatType != outPutFormat)
            {
                Tagger.LatticeLevel = MeCabLatticeLevel.One;
                Tagger.OutPutFormatType = outPutFormat;
                Tagger.AllMorphs = false;
                Tagger.Partial = false;
            }
        }

        /// <summary>
        /// splits japanese sentence to it's components
        /// 
        /// output data is used to show sentences with furigana
        /// </summary>
        /// <param name="textToAnalyze"></param>
        /// <returns></returns>
        public static List<MeCabData> AnalyzeModel(string textToAnalyze)
        {
            string analyzedString = AnalyzeString(textToAnalyze, OutPutFormats.Lattice);
            List<string> lines = analyzedString.Split('\n').ToList();
            List<MeCabData> meCabDatas =
                lines.Take(lines.Count - 2)
                    .Select(l => l.Split('\t'))
                    .Select(p => new MeCabData { Token = p[0], Feature = p[1] })
                    .ToList();
            var result = new List<MeCabData>();
            foreach (MeCabData meCabData in meCabDatas)
            {
                string[] strings = meCabData.Feature.Split(',');
                meCabData.Pos = string.Join("", strings.Take(4));
                meCabData.Inflection = string.Join("", strings.Skip(4).Take(2));
                meCabData.Baseform = string.Join("", strings.Skip(6).Take(1));
                meCabData.Pronounciation = string.Join("", strings.Skip(7).Take(1)); //9.su hep 8.nin ayni gibi
                result.Add(meCabData);
            }
            return result;
        }

        public class OutPutFormats
        {
            public static string Lattice = "lattice";
            public static string Wakati = "wakati";
            public static string None = "none";
            public static string Dump = "dump";
            public static string Em = "em";
        }
    }
}