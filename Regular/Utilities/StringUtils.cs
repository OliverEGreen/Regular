using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regular.Utilities
{
    public static class StringUtils
    {
        public static string SubString(this string str, int startIndex, int endIndex)
        {
            return endIndex < startIndex ? string.Empty : str.Substring(startIndex, endIndex - startIndex);
        }
        private static readonly List<string> IllegalRevitCharacters = new List<string>()
        {
            "/",
            ":",
            "{",
            "}",
            "[",
            "]",
            "|",
            ";",
            ">",
            "<",
            "?",
            "`",
            "~",
            Environment.NewLine
        };
        public static string SanitizeRevitChars(string value)
        {
            return IllegalRevitCharacters.Aggregate(value, (current, t) => current.Replace(t, " "));
        }

        public static string ReplaceFirstInstance(string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0) return text;
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string RemoveLineBreaks(string value) => value.Replace("\n", "").Replace("\r", "");
        
        public static string BreakPascalCase(string input)
        {
            string reformattedString = "";
            int counter = 0;
            foreach (char character in input)
            {
                if (counter == 0) reformattedString += character;
                else if (char.IsUpper(character)) reformattedString += $" {char.ToLowerInvariant(character)}";
                else reformattedString += character;
                counter++;
            }
            return reformattedString;
        }
        public static string WrapString(string value, int maxLineLength)
        {
            // Takes a long string and splits across multiple new lines by a maximum length
            int charCount = 0;
            List<string> lines = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(w => (charCount += w.Length + 1) / maxLineLength)
                .Select(g => string.Join(" ", g))
                .ToList();

            return string.Join(Environment.NewLine, lines);
        }

        public static string StripToAlphanumeric(string value) => Regex.Replace(value, "[^A-Za-z0-9]", "");
    }
}