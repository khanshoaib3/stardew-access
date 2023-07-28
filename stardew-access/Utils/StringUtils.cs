using System;
using System.Collections.Generic;
using System.Text;

namespace stardew_access.Utils
{
    public static class StringUtils
    {
        const string startEscape = "\uFFF0";
        const string endEscape = "\uFFF1";

        public static string FormatWith(this string format, IDictionary<string, object> values)
        {
            if (format.Contains(startEscape) || format.Contains(endEscape))
            {
                throw new ArgumentException($"The format string cannot contain the characters '{startEscape}' or '{endEscape}'.");
            }

            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var result = new StringBuilder(format);

            // Replace escaped braces with a temporary placeholder
            result.Replace("{{", startEscape).Replace("}}", endEscape);

            foreach (var pair in values)
            {
                result.Replace("{" + pair.Key + "}", pair.Value.ToString());
            }

            // Check if there are any unreplaced keys
            if (result.ToString().Contains("{") || result.ToString().Contains("}"))
            {
                throw new FormatException("One or more keys in the format string do not have corresponding entries in the dictionary.");
            }

            // Replace the temporary placeholders with braces
            result.Replace(startEscape, "{").Replace(endEscape, "}");

            return result.ToString();
        }
    }
}