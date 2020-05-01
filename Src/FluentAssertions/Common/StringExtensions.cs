using System;
using System.Linq;
using FluentAssertions.Formatting;

namespace FluentAssertions.Common
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Finds the first index at which the <paramref name="value"/> does not match the <paramref name="expected"/>
        /// string anymore, accounting for the specified <paramref name="stringComparison"/>.
        /// </summary>
        public static int IndexOfFirstMismatch(this string value, string expected, StringComparison stringComparison)
        {
            Func<char, char, bool> comparer = GetCharComparer(stringComparison);

            for (int index = 0; index < value.Length; index++)
            {
                if ((index >= expected.Length) || !comparer(value[index], expected[index]))
                {
                    return index;
                }
            }

            return -1;
        }

        private static Func<char, char, bool> GetCharComparer(StringComparison stringComparison) =>
            stringComparison == StringComparison.Ordinal
                ? (Func<char, char, bool>)((x, y) => x == y)
                : (x, y) => char.ToUpperInvariant(x) == char.ToUpperInvariant(y);

        /// <summary>
        /// Gets the quoted three characters at the specified index of a string, including the index itself.
        /// </summary>
        public static string IndexedSegmentAt(this string value, int index)
        {
            int length = Math.Min(value.Length - index, 3);
            string formattedString = Formatter.ToString(value.Substring(index, length));

            return $"{formattedString} (index {index})".EscapePlaceholders();
        }

        /// <summary>
        /// Replaces all characters that might conflict with formatting placeholders with their escaped counterparts.
        /// </summary>
        public static string EscapePlaceholders(this string value) =>
            value.Replace("{", "{{", StringComparison.Ordinal).Replace("}", "}}", StringComparison.Ordinal);

        /// <summary>
        /// Replaces all characters that might conflict with formatting placeholders with their escaped counterparts.
        /// </summary>
        internal static string UnescapePlaceholders(this string value) =>
            value.Replace("{{", "{", StringComparison.Ordinal).Replace("}}", "}", StringComparison.Ordinal);

        /// <summary>
        /// Joins a string with one or more other strings using a specified separator.
        /// </summary>
        /// <remarks>
        /// Any string that is empty (including the original string) is ignored.
        /// </remarks>
        public static string Combine(this string @this, string other, string separator = ".")
        {
            if (@this.Length == 0)
            {
                return (other.Length != 0) ? other : string.Empty;
            }

            if (other.Length == 0)
            {
                return @this;
            }

            return @this + separator + other;
        }

        /// <summary>
        /// Changes the first character of a string to uppercase.
        /// </summary>
        public static string Capitalize(this string @this)
        {
            if (@this.Length == 0)
            {
                return @this;
            }

            char[] charArray = @this.ToCharArray();
            charArray[0] = char.ToUpperInvariant(charArray[0]);
            return new string(charArray);
        }

        /// <summary>
        /// Appends tab character at the beginning of each line in a string.
        /// </summary>
        /// <param name="this"></param>
        public static string IndentLines(this string @this)
        {
            return string.Join(Environment.NewLine,
                @this.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => $"\t{x}"));
        }

        public static string RemoveNewLines(this string @this)
        {
            return @this.Replace("\n", "", StringComparison.Ordinal)
                        .Replace("\r", "", StringComparison.Ordinal)
                        .Replace("\\r\\n", "", StringComparison.Ordinal);
        }

        /// <summary>
        /// Counts the number of times a substring appears within a string by using the specified <see cref="StringComparison"/>.
        /// </summary>
        /// <param name="substring">The substring to search for.</param>
        /// <param name="comparisonType">The <see cref="StringComparison"/> option to use for comparison.</param>
        public static int CountSubstring(this string @this, string substring, StringComparison comparisonType)
        {
            string actual = @this ?? "";
            string search = substring ?? "";

            int count = 0;
            int index = 0;

            while ((index = actual.IndexOf(search, index, comparisonType)) >= 0)
            {
                index += search.Length;
                count++;
            }

            return count;
        }
    }
}
