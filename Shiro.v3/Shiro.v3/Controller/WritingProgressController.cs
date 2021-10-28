using Shiro.Model;

namespace Shiro.Controller
{
    public interface IWritingProgressController :IBaseController
    {

        /// <summary>
        /// increase writing count for object and returns final count
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="count">increase count</param>
        /// <returns>returns final count</returns>
        int IncreaseProgress(string obj, int count);

        /// <summary>
        /// fetches progress from db if exists or returns a new one
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>writingprogress object for string</returns>
        WritingProgress GetProgress(string obj);
    }

    public class WritingProgressController : BaseController,IWritingProgressController 
    {
        public int IncreaseProgress(string obj, int count)
        {
            var writingProgress = GetProgress(obj);
            writingProgress.WriteCount += count;
            ShiroRepository.Save(writingProgress);
            return writingProgress.WriteCount;
        }

        public WritingProgress GetProgress(string obj)
        {
            var writingProgress = ShiroRepository.GetSingle<WritingProgress>(t => t.Object == obj) ?? new WritingProgress();
            writingProgress.Object = obj;
            return writingProgress;
        }
    }

}
