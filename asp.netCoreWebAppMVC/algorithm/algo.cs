using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.algorithm
{
    public class RemoveDupChars
    {
        // --- Removes duplicate chars using string concats. ---
        static string RemoveDuplicateChars(string key)
        {
            // Store encountered letters in this string.
            string table = "";
            // Store the result in this string.
            string result = "";
            // Loop over each character.
            foreach(char value in key)
            {
                // See if character is in the table.
                if(table.IndexOf(value) == -1)
                {
                    // Append to the table and the result.
                    table += value;
                    result += value;
                }
            }
            return result;
        }
    }

    public static class ReverseString
    {
        public static string Reverse(string x)
        {
            string result = "";

            for (int i = x.Length - 1; i >= 0; i--)
                result += x[i];

            return result;
        }
    }
}
