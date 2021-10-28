using System.Collections.Generic;
using Shiro.Model;

namespace Shiro.Controller
{
    public interface IKanjiInfoController
    {
        IEnumerable<KanjiInfo> Search(string unicodeOrCharacter);
        KanjiInfo Get(int id);
        void ClearAllData();
    }

    public class KanjiInfoController : BaseController, IKanjiInfoController
    {
        public IEnumerable<KanjiInfo> Search(string unicodeOrCharacter)
        {
            return ShiroRepository.GetMany<KanjiInfo>(100,
                entry => entry.Kanji == unicodeOrCharacter || entry.Unicode == unicodeOrCharacter);
        }

        public KanjiInfo Get(int id)
        {
            return ShiroRepository.GetSingle<KanjiInfo>(id);
        }

        public void ClearAllData()
        {
            ShiroRepository.Drop<KanjiInfo>();
        }
    }
}