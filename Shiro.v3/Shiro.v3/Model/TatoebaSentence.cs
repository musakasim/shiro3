using System.Collections.Generic;
using System.Windows.Documents;
using FrInterfaces;
using Shiro.Library.FuriganaView;

namespace Shiro.Model
{
    public class TatoebaSentence : IBaseModel
    {
        public int JapId { get; set; }
        public string JapSentence { get; set; }
        public List<SentenceToken> JapSentenceTokens { get; set; }
        public int EngId { get; set; }
        public string EngSentence { get; set; }
        public int Id { get; set; }
    }

    public class SentenceToken  
    {
        public string Token { get; set; }
        public string Pronounciation { get; set; } 
    }
}