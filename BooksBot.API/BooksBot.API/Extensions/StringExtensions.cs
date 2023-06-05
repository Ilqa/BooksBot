using System.Text.RegularExpressions;

namespace BooksBot.API.Extensions
{
    public static class StringExtensions
    {
        public static string GetDecimalOrIntPartFromString(this string str)
        {
            return Regex.Match(str, @"\d+\.*\d*").Value;
        }
        public static string RemoveNewLinesAndWhiteSpaces(this string str)
        {
            return Regex.Replace(str, @"\s+", " ").Replace("\n", "").Replace("\r", "").Replace("/g", "").Trim();
        }
    }
}
