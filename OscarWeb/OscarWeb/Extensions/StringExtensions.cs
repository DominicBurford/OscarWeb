using System;
using System.Text;

namespace OscarWeb.Extensions
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Encode to to Base64
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(this string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Decode from Base64
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string Base64Decode(this string base64EncodedData)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Normalise the specified JSON string to allow serialization
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string NormalizeJsonString(this string input)
        {
            string result = input;
            if (!string.IsNullOrEmpty(input))
            {
                //remove leading and trailing "\" characters and remove "\\" from string
                char[] trailingChar = result.Substring(0, 1).ToCharArray();
                byte[] trailingByte = Encoding.ASCII.GetBytes(trailingChar);
                if (trailingByte[0] == 34)
                {
                    result = result.Substring(1);
                }
                trailingChar = result.Substring(result.Length - 1, 1).ToCharArray();
                trailingByte = Encoding.ASCII.GetBytes(trailingChar);
                if (trailingByte[0] == 34)
                {
                    result = result.Substring(0, result.Length - 1);
                }
                result = result.Replace("\\", "");
            }
            return result;
        }
    }
}
