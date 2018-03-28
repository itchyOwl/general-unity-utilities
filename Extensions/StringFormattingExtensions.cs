namespace ItchyOwl.Extensions
{
    /// <summary>
    /// Best: blue (use for important logs)
    /// Subtle: maroon, green, purple, teal (use for normal logs that don't have to stand out)
    /// Striking: magenta, red (but not clearly visible against the red tint -> use only when you need to spot something after the game has been stopped)
    /// Others are either not clearly visible or too close to black.
    /// </summary>
    public enum Colors
    {
        aqua,
        black,
        blue,
        brown,
        cyan,
        darkblue,
        fuchsia,
        green,
        grey,
        lightblue,
        lime,
        magenta,
        maroon,
        navy,
        olive,
        purple,
        red,
        silver,
        teal,
        white,
        yellow
    }

    public static class StringFormattingExtensions
    {
        public static string Colored(this string s, Colors color)
        {
            return string.Format("<color={0}>{1}</color>", color.ToString(), s);
        }

        public static string Colored(this string s, string colorCode)
        {
            return string.Format("<color={0}>{1}</color>", colorCode, s);
        }

        public static string Sized(this string s, int size)
        {
            return string.Format("<size={0}>{1}</size>", size, s);
        }

        public static string Bold(this string s)
        {
            return s.WrapInTag("b");
        }

        public static string Italics(this string s)
        {
            return s.WrapInTag("i");
        }

        public static string Underlined(this string s)
        {
            return s.WrapInTag("u");
        }

        public static string Strikethrough(this string s)
        {
            return s.WrapInTag("s");
        }

        public static string Superscript(this string s)
        {
            return s.WrapInTag("sup");
        }

        public static string Subscript(this string s)
        {
            return s.WrapInTag("sub");
        }

        /// <summary>
        /// Wraps the string in a tag.
        /// Give the tag without the tag characters, i.e. only the content of the tag.
        /// </summary>
        public static string WrapInTag(this string s, string tagContent)
        {
            return string.Format("{0}{1}{2}", string.Format("{0}{1}{2}", "<", tagContent, ">"), s, string.Format("{0}{1}{2}", "</", tagContent, ">"));
        }
    }
}