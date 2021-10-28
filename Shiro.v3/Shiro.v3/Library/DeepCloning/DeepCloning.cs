using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Shiro.Library.DeepCloning
{

    /// <summary>
    /// Serialize and deserialize an object to produce new instance
    /// 
    /// BUT: fails with dependencyproperties:the error message:
    ///     Current local value enumeration is outdated because one or more local values have been set since its creation
    /// </summary>
	public static class DeepCloning
	{
		public static T Clone<T>(this T obj)
		{
			IFormatter formatter = new BinaryFormatter();
			formatter.SurrogateSelector = new SurrogateSelector();
			formatter.SurrogateSelector.ChainSelector(new NonSerialiazableTypeSurrogateSelector());
			var ms = new MemoryStream();
			formatter.Serialize(ms, obj);
			ms.Position = 0;
			return (T)formatter.Deserialize(ms);
		}
	}
}
