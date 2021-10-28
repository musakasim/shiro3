using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kelebron.Utils.Japanese;
using Shiro.Library;
using Shiro.Model;

namespace Shiro.Controller
{
    public enum SearchLangMode
    {
        EngToJap,
        JapToEng,
        EngToJapJapToEng,
    }

    public interface IShiroDictionaryController
    {
        IEnumerable<ShiroEntry> Search(string searchTerm, SearchLangMode mode = SearchLangMode.JapToEng, bool bringSampleResult = false);
        Task<IEnumerable<ShiroEntry>> SearchAsync(string searchTerm, SearchLangMode mode = SearchLangMode.JapToEng, bool bringSampleResult = false);
        ShiroEntryBzzt Get(int id);
        void ClearAllData();
    }

    public class ShiroDictionaryController : BaseController, IShiroDictionaryController
    {
        public ShiroEntryBzzt Get(int id)
        {
            var shiroEntry = ShiroRepository.GetSingle<ShiroEntry>(id);
            return new ShiroEntryBzzt(shiroEntry);
        }

        public void ClearAllData()
        {
            ShiroRepository.Drop<ShiroEntry>();
        }

        public async Task<IEnumerable<ShiroEntry>> SearchAsync(string searchTerm, SearchLangMode mode = SearchLangMode.JapToEng, bool bringSampleResult = false)
        {
            var a = await Task.Run(()=> Search(searchTerm, mode, bringSampleResult));

            return a;
        }

        /// <summary>
        /// Searches 400 matches in db
        /// orders result set by score
        /// returns first 30 result
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="mode"></param>
        /// <param name="bringSampleResult"></param>
        /// <returns></returns>
        public IEnumerable<ShiroEntry> Search(string searchTerm, SearchLangMode mode = SearchLangMode.JapToEng, bool bringSampleResult = false)
        {
            IEnumerable<ShiroEntry> entries = new List<ShiroEntry>();

            if (string.IsNullOrEmpty(searchTerm))
            {
                if (bringSampleResult)
                    searchTerm = "";
                else
                    return new List<ShiroEntryBzzt>();
            }

            searchTerm = searchTerm.ToLower();
            if (mode == SearchLangMode.JapToEng)
            {
                JapaneseKanaConverter.TryParseRomajiToHiragana(searchTerm, out searchTerm);
                entries = ShiroRepository.GetMany<ShiroEntry>(400, entry => entry.Spellings.Any(k => k.Value.Contains(searchTerm)));
            }
            else if (mode == SearchLangMode.EngToJap)
            {
                entries = ShiroRepository.GetMany<ShiroEntry>(400, entry => entry.Meanings.Any(k => k.Gloss.Contains(searchTerm)));
            }

            // order results by score and return first 30
            return entries.OrderBy(s => Score(searchTerm, s)).Take(30);
        }

        /// <summary>
        ///     returns similarity index (or score) of the dictionary entry for searched term
        ///     exact match will be scored as -1 and upcomming matches will be scored with ascending values
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        private int Score(string searchTerm, ShiroEntry entry)
        {
            int score = -1;
            //if not the complete match, calculate accurancy
            if (entry.Spellings.All(k => k.Value != searchTerm))
            {
                var spellingsStartingWithSearchTerm = entry.Spellings.Where(k => k.Value.StartsWith(searchTerm)).ToList();
                var spellingsContainingSearchTerm = entry.Spellings.Where(k => k.Value.Contains(searchTerm) && !k.Value.StartsWith(searchTerm)).ToList();
                if (spellingsStartingWithSearchTerm.Any())
                {
                    score += spellingsStartingWithSearchTerm.Min(t => t.Value.Length - searchTerm.Length) * 10;
                }
                else if (spellingsContainingSearchTerm.Any())
                {
                    //aranan ifadenin sonuc icinde basladigi index + degerin uzunlugunun en kucugu :
                    score += spellingsContainingSearchTerm.Min(t => t.Value.IndexOf(searchTerm, StringComparison.Ordinal)
                                    .Pipe(index => index * 2 + ((t.Value.Length - searchTerm.Length) * 10)));
                }
            }
            return score;
        }
    }
}