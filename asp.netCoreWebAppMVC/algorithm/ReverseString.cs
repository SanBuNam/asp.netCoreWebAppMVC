using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.Algorithm
{
    public class ReverseString
    {
        public static string ReverseStr(string str)
        {
            var result = "";

            for (var i = str.Length -1; i >= 0; i--) {
                result += result;
            }

            return result;
        }
    }
}
