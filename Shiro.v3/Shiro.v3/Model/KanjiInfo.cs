using FrInterfaces;

namespace Shiro.Model
{
    /// <summary>
    ///     Reformed KanjiDic2 data
    /// </summary>
    public class KanjiInfo : IBaseModel
    {
        /// <summary>
        ///     Kanjidic2 KanjiDic2Char.Literal
        /// </summary>
        public string Kanji { get; set; }

        /// <summary>
        ///     Misc.Freq
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        ///     Misc.Jlpt
        /// </summary>
        public int JlptLvl { get; set; }

        /// <summary>
        ///     Misc.Grade
        /// </summary>
        public int SchoolGrade { get; set; }

        /// <summary>
        ///     Misc.StrokeCount
        /// </summary>
        public int StrokeCount { get; set; }

        /// <summary>
        ///     Codepoint.Where(qcType == "ucs")
        /// </summary>
        public string Unicode { get; set; }

        /// <summary>
        ///     ReadingMeaning.Readings jpn_on (chinese readings)
        /// </summary>
        public string OnReadings { get; set; }

        /// <summary>
        ///     ReadingMeaning.Readings jpn_kun
        /// </summary>
        public string KunReadings { get; set; }

        /// <summary>
        ///     name readings
        /// </summary>
        public string NanoriReadings { get; set; }

        /// <summary>
        ///     ReadingMeaning.Readings Where lang is empty(this means this will contain only english meanings)
        /// </summary>
        public string Meanings { get; set; }

        public KanjiGraph KanjiGraph { get; set; }
        public int Id { get; set; }
    }
}