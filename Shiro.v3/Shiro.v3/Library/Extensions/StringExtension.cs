namespace Shiro.Library.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// returns last characters indicated by tailLength parameter
        /// </summary> 
        public static string GetLast(this string source, int tailLength)
        {
            if (tailLength >= source.Length)
                return source;
            return source.Substring(source.Length - tailLength);
        }

        /// <summary>
        /// if string length is longer than length parameter then crops the string and adds ... at the end of string 
        /// </summary> 
        public static string RestrictLenght(this string source, int length)
        {
            //3 for 3 dots that would be added at the end of string
            if (length+3 >= source.Length)
                return source;
            return source.Substring(0,length)+ "...";
        }
    }
}