using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace IMCMS.Common.Extensions
{

    public static class StringExtensions
    {
        /// <summary>
        /// Strip all HTML tags
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Text with HTML tags removed</returns>
        public static String StripHtml(this String str)
        {
			if(str == null)
				return String.Empty;

            return Regex.Replace(str, @"<[\/!A-z]+(?:.*?(?:=\s?(?:(""|')[^\1]*\1|[^\s>]*))?)+(?:>|(<))", string.Empty);
        }

        /// <summary>
        /// Strip all HTML tags, and replace with a space instead of an empty string
        /// </summary>
        /// <param name="str">input string</param>
        /// <param name="replaceWithSpace">If the replaced tag should be replaced with a space</param>
        /// <returns>Text with HTML tags removed</returns>
        public static String StripHtml(this String str, bool replaceWithSpace)
        {
			if(str == null)
				return String.Empty;

            return Regex.Replace(str, @"<[\/!A-z]+(?:.*?(?:=\s?(?:(""|')[^\1]*\1|[^\s>]*))?)+(?:>|(<))", replaceWithSpace ? " " : String.Empty);
        }

        /// <summary>
        /// Replace plan text line breaks [\n] with HTML break lines
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>String with line breaks replaced with HTML breaks</returns>
        public static String nl2br(this String str)
        {
			if(str == null)
				return String.Empty;
				
            return str.Replace("\n", "<br />");
        }

        public static HtmlString nl2br(this String str, bool encodeInput)
        {
            if (str == null)
                return new HtmlString("");

            if (encodeInput)
                str = HttpUtility.HtmlEncode(str);

            return new HtmlString(str.Replace("\n", "<br />"));
        }

        /// <summary>
        /// Truncate string to number of characters, avoids truncating in the middle of a word
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static String Truncate(this string input, int length)
        {
			if(input == null)
				return String.Empty;
				
            if (input == null || input.Length < length)
                return input;
            int iNextSpace = input.LastIndexOf(" ", length);
            return string.Format("{0}...", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
        }

        /// <summary>
        /// Truncate string to a number of characters, avoids truncating in the middle of a word, and appends moreText to the end of string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <param name="moreLink"></param>
        /// <returns></returns>
        public static String Truncate(this string input, int length, string moreText)
        {
			if(input == null)
					return String.Empty;
					
            if (input == null || input.Length < length)
                return input;
            int iNextSpace = input.LastIndexOf(" ", length);
            return string.Format("{0}... {1}", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim(), moreText);
        }

        /// <summary>Determines if a url is internal using an array of local domains.</summary>
        /// <param name="str">The source text to verify. It should be an absolute url to verify with local domains, otherwise always will return true since it is relative.</param>
        /// <param name="localDomains">Array of domains to verify if url is inside of.</param>
        /// <returns>True if url provided is internal and match with one of the local domains specified in the array.</returns>
        public static bool IsUrlInternal(this String str, string[] localDomains)
        {
            if (localDomains == null || localDomains.Length == 0)
                throw new ArgumentNullException("localDomains", "domains array cannot be null or empty.");
            if (String.IsNullOrEmpty(str))
                return false;

            Uri uri;
            if (Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out uri))
            {
                return !uri.IsAbsoluteUri || localDomains.Any(domain => domain != null && domain.IndexOf(uri.Host, StringComparison.InvariantCultureIgnoreCase) != -1);
            }
            return false;
        }

        /// <summary>Determines if a url is an internal file that exists using an array of local domains.</summary>
        /// <param name="str">The source text to verify. local domains will only be used if url is absolute, otherwise always it is asumed to be internal since it is relative.</param>
        /// <param name="localDomains">Array of domains to verify if url is inside of.</param>
        /// <returns>True if url is an file that exists in local domains.</returns>
        public static bool IsUrlAnInternalExistingFile(this String str, string[] localDomains)
        {
            if (localDomains == null || localDomains.Length == 0)
                throw new ArgumentNullException("localDomains", "domains array cannot be null or empty.");
            if (String.IsNullOrEmpty(str))
                return false;

            if (str.IsUrlInternal(localDomains))
            {
                Uri uri;
                if (Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out uri))
                {
                    string serverPath = HttpContext.Current.Server.MapPath((!uri.IsAbsoluteUri) ? str : uri.PathAndQuery);
                    return System.IO.File.Exists(serverPath);
                }
                throw new ArgumentException("input string was internal, but it could not be created as relative or absolute path.");
            }
            return false;
        }

        /// <summary>
        /// Trims the standard quotes from either end of a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimQuotes(this String str)
        {
            if (str == null)
                return String.Empty;

            return str.Trim(new char[] { '"', '“', '”' });
        }


        /// <summary>
        /// Simple extension methods to replace String.IsNullOrEmpty()
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string input)
        {
            return !String.IsNullOrEmpty(input);
        }

        public static bool IsEmpty(this string input)
        {
            return String.IsNullOrEmpty(input);
        }

        public static string FormatWith(this string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(format, args);
        }

        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(provider, format, args);
        }
    }
}
