using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Shiro.Model;

namespace Shiro.Controller
{
    public interface ITatoebaController
    {
        IEnumerable<TatoebaSentence> Search(string searchTerm);
        void ClearAllData();
        void ClearSentenceTokens();
        int GetSentenceCount(Expression<Func<TatoebaSentence, bool>> predicate= null);
        int GetUntokenizedCount();
    }

    public class TatoebaController : BaseController, ITatoebaController
    {
        public IEnumerable<TatoebaSentence> Search(string searchTerm)
        {
            searchTerm = searchTerm ?? "";
            // var tatoebaSentences = ShiroRepository.GetMany<TatoebaSentence>(3, entry => entry.JapSentenceTokens.Any(t => t.Token == searchTerm));
            var tatoebaSentences = ShiroRepository.GetMany<TatoebaSentence>(5, entry => entry.JapSentence.Contains(searchTerm));
            return tatoebaSentences;
        }

        public void ClearAllData()
        {
            ShiroRepository.Drop<TatoebaSentence>();
        }

        public void ClearSentenceTokens()
        {
            var tatoebaSentences = ShiroRepository.GetMany<TatoebaSentence>();
            foreach (var sentence in tatoebaSentences)
            {
                sentence.JapSentenceTokens = null;
                ShiroRepository.Save(sentence);
            }
        }

        public int GetSentenceCount(Expression<Func<TatoebaSentence, bool>> predicate = null)
        {
            return ShiroRepository.GetMany(predicate: predicate).Count();
        }

        public int GetUntokenizedCount()
        {
            return GetSentenceCount(t => t.JapSentenceTokens == null);
        }
    }
}