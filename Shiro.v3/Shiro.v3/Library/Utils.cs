using System.IO;
using System.Linq;
using System.Text;

namespace Shiro.Library
{

    public static class Utils
    {
        public static void GuardFile(string filePath)
        {
            var directoryName = Path.GetDirectoryName(filePath);
            if (directoryName != null)
            {
                var directory = new DirectoryInfo(directoryName);
                if (!directory.Exists) directory.Create();
            }

            if (!File.Exists(filePath))
            {
                FileStream fileStream = File.Create(filePath);
                fileStream.Close();
            }
        }

        public static string GetUnicode(this string moji)
        {
            //var moji = "本";//0x672c
            var encoding = new UnicodeEncoding();
            byte[] bytes = encoding.GetBytes(moji);
            string a = string.Empty;
            foreach (byte b in bytes.Reverse())
            {
                a = a + b.ToString("X2");
            }
            return a;
        }

        public static string Multiply(this string source, int multiplier)
        {
            var sb = new StringBuilder(multiplier * source.Length);
            for (int i = 0; i < multiplier; i++)
            {
                sb.Append(source);
            }

            return sb.ToString();
        }

        public static T Parse<T>(this object a) where T : new()
        {
            if (a is T)
            {
                return (T)a;
            }
            //return new T();
            return default(T);//null
        }
    }
}
