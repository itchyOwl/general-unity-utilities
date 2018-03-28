using System.Linq;
using System;
using System.ComponentModel;

namespace ItchyOwl.Extensions
{
    public static class EnumExtensions
    {
        // Modified from: https://stackoverflow.com/questions/479410/enum-tostring-with-user-friendly-strings
        public static string FormatAccordingToDescription<T>(this T value) where T : struct
        {
            Type type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("[EnumExtensions] The value must be of type Enum", "value");
            }
            // Tries to find a DescriptionAttribute for a potential friendly name for the enum
            var memberInfo = type.GetMember(value.ToString()).FirstOrDefault();
            if (memberInfo != null)
            {
                var attribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false).FirstOrDefault(a => a as DescriptionAttribute != null) as DescriptionAttribute;
                if (attribute != null)
                {
                    return attribute.Description;
                }
            }
            // If we have no description attribute, just return the ToString of the enum
            return value.ToString();
        }

        public static string FormatWithSpaces<T>(this T value) where T : struct
        {
            Type type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("[EnumExtensions] The value must be of type Enum", "value");
            }
            string original = value.ToString();
            var separators = original.Where(c => c.Equals('_') || char.IsUpper(c));
            var splitted = original.Split(separators.ToArray()).Where(s => !s.IsNullOrEmpty());
            // Use Linq.Zip if on .Net 4.0?
            string joined = string.Empty;
            for (int i = 0; i < splitted.Count(); i++)
            {
                joined += separators.ElementAt(i).ToString() + splitted.ElementAt(i);
                if (i < splitted.Count() - 1)
                {
                    joined += " ";
                }
            }
            return joined;
        }
    }
}
