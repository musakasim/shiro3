namespace Shiro.Library.FuriganaView
{
    public class MeCabData
    {
        /// <summary>
        ///     Called surface for mecab
        /// </summary>
        public string Token { get; set; } //稼ぎ

        /// <summary>
        ///     All Parse details
        ///     Consists of 9 tokens splitted with commas
        /// </summary>
        public string Feature { get; set; } //動詞,自立,*,*,五段・ガ行,連用形,稼ぐ,カセギ,カセギ

        /// <summary>
        ///     FIRST 4 TOKENS OF FEATURE.Split(',')
        ///     name, verb, adj vs.
        /// </summary>
        public string Pos { get; set; } //<pos>動詞,自立</pos>

        /// <summary>
        ///     5TH AND 6TH TOKENS
        ///     for verbs root and suffix explanations
        /// </summary>
        public string Inflection { get; set; } //<inflection>五段・ガ行,連用形</inflection>


        /// <summary>
        ///     7TH TOKEN
        ///     tokens whose inlections is found token's dictionary form
        /// </summary>
        public string Baseform { get; set; } //<baseform>稼ぐ</baseform>

        /// <summary>
        ///     LAST TWO TOKENS (though they r same)
        ///     katakana reading
        /// </summary>
        public string Pronounciation { get; set; } //<pronounciation>カセギ</pronounciation>
    }
}