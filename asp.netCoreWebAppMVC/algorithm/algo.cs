﻿using System;
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
            string checker = "";
            // Store the result in this string.
            string result = "";
            // Loop over each character.
            foreach (char value in key)
            {   // See if character is in the table.
                if (checker.IndexOf(value) == -1)
                {
                    // Append to the checker and the result.
                    checker += value;
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

            for (var i = x.Length - 1; i >= 0; i--)
                result += x[i];
            
            return result;
        }
    }

    public static class Wordcount
    {
        //Count the number of words in a string (Needs to handle multiple spaces between words)
        public static int Count(string x)
        {
            int result = 0;
            // Trim white space from beginning and end of string
            x = x.Trim();
            // Necessary because foreach will execute once with empty string returing 1
            if (x == "")
                return 0;
            // Ensure there is only one space between each word in the passed string
            while (x.Contains("  "))
                x = x.Replace("  ", " ");
            // count the words
            foreach (string y in x.Split(' '))
                result++;

            return result;
        }
    }

    public class Palindrome
    {
        public static bool IsPalindrome(string word)
        {
            int min = 0;
            int max = word.Length - 1;

            while (true)
            {
                if (min > max)
                    return true;
                
                char a = word[min];
                char b = word[max];

                if (char.ToLower(a) != char.ToLower(b))
                    return false;

                min++;
                max--;
            }
        }
    }

    public class Anagram
    {
        public static bool IsAnagram(string word1, string word2)
        {
            bool result = false;

            char[] char1 = word1.ToLower().ToCharArray();
            char[] char2 = word2.ToLower().ToCharArray();

            Array.Sort(char1);
            Array.Sort(char2);

            string NewWord1 = new string(char1);
            string NewWord2 = new string(char2);

            if (NewWord1 == NewWord2)
                result = true;

            return result;
        }
    }



}
