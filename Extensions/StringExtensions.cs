using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Capitalises the first letter (invariant) and forces the rest to lower case (invariant).
        /// </summary>
        public static string CapitaliseFirstInvariant(this string s)
        {
            if (string.IsNullOrEmpty(s)) { return string.Empty; }
            return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1, s.Length - 1).ToLowerInvariant();
        }

        /// <summary>
        /// Adds spaces into a CamelCase string.
        /// </summary>
        public static string FormatCamelCaseWithSpaces(this string str)
        {
            return new string(InsertSpacesBeforeCaps(str).ToArray());
            IEnumerable<char> InsertSpacesBeforeCaps(IEnumerable<char> input)
            {
                int i = 0;
                int lastChar = input.Count() - 1;
                foreach (char c in input)
                {
                    if (char.IsUpper(c) && i > 0)
                    {
                        yield return ' ';
                    }
                    yield return c;
                    i++;
                }
            }
        }
    }
}
