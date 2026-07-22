using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace QCard.ManagementSystem.Service
{
    public static class MD5Helper
    {
        /// <summary>
        /// Encode string to MD5
        /// </summary>
        public static string Encode(string input)
        {
            byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

            var sb = new StringBuilder();

            for (int i = 0, j = data.Length; i < j; i++)
                sb.Append(data[i].ToString("x2"));

            return sb.ToString();
        }
    }
}
