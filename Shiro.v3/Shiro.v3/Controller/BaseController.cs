using FrInterfaces;

namespace Shiro.Controller
{
    public interface IBaseController
    {
    }

    //todo:SORU : Burada BaseController ve ShiroRepository diye 2 basamak olmasi çok mu saçma  yoksa matıklı mı silesim var birini!!!!!!!!!
    public class BaseController : IBaseController
    {
        public BaseController()
        {
            ShiroRepository = new ShiroRepository(IoCContainer.Resolve<IRepository>());
        }

        protected ShiroRepository ShiroRepository { get; set; }
    }
}