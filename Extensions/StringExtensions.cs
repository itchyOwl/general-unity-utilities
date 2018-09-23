using System;
using System.Linq;

namespace ItchyOwl.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static string Remove(this string s, Func<char, bool> predicate)
        {
            return new string(s.ToCharArray().Where(c => !predicate(c)).ToArray());
        }

        public static string RemoveWhitespace(this string s)
        {
            return s.Remove(c => char.IsWhiteSpace(c));
        }
    }
}
