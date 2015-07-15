using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElucidatClient.Support {
    public static class StringExtensions {
        private static readonly Dictionary<string, string> ToBeEncoded = new Dictionary<string, string>() { 
            { "%", "%25" }, 
            { "!", "%21" }, 
            { "#", "%23" }, 
            { " ", "%20" },
            { "$", "%24" }, 
            { "&", "%26" }, 
            { "'", "%27" }, 
            { "(", "%28" }, 
            { ")", "%29" }, 
            { "*", "%2A" }, 
            { "+", "%2B" }, 
            { ",", "%2C" }, 
            { "/", "%2F" }, 
            { ":", "%3A" }, 
            { ";", "%3B" }, 
            { "=", "%3D" }, 
            { "?", "%3F" }, 
            { "@", "%40" }, 
            { "[", "%5B" }, 
            { "]", "%5D" } 
        };
        private static readonly Regex ReplaceRegex = new Regex(@"[%!# $&'()*+,/:;=?@\[\]]");

        /// <summary>
        /// .NET URL encoding functions (e.g. WebUtility.UrlEncode) do not precisely match the action of php rawurlencode(), 
        /// which is used by the Elucidat API for signature checking. Therefore we use this custom function instead when URL encoding 
        /// any data to be sent to the API or used in signature generation.
        /// </summary>
        /// <param name="s">The string to be encoded</param>
        /// <returns>The encoded string</returns>
        public static string RawUrlEncode(this string s) {
            return ReplaceRegex.Replace(s, match => ToBeEncoded[match.Value]);
        }

        /// <summary>
        /// Transform a .NET style property name (e.g. ProjectCode) into a PHP-style property name (e.g. project_code)
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string TransformPropertyName(this string propertyName) {
            return String.Join("_", Regex.Split(propertyName, "(?=[A-Z])").Where(x => !String.IsNullOrWhiteSpace(x))).ToLower();
        }
    }
}
