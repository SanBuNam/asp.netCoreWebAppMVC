using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.General
{
    // Object.MemberwiseClone() method can be used to create a shallow copy of the current object.
    public class CopyObjectMemberwiseClone
    {
        public string str;
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class MemberwiseCloneExample
    {
        public static void RunMain()
        {
            CopyObjectMemberwiseClone obj = new CopyObjectMemberwiseClone();
            obj.str = "Hello!";

            CopyObjectMemberwiseClone copy = (CopyObjectMemberwiseClone)obj.Clone();
            Console.WriteLine(copy.str);
        }
    }

    
}
