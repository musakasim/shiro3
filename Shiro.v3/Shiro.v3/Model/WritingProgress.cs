using FrInterfaces;

namespace Shiro.Model
{
    public class WritingProgress : IBaseModel
    {
        public int Id { get; set; }

        public string Object { get; set; }

        public int WriteCount { get; set; }
    }
}
