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

    // Copy Constructor takes another instance of the same class and defines the complier's actions when copying the object.
    // The copy constructor implementation should perform deep copy for any referenced objects in the class by creating new objects and copying the immutable type's values.
    // The following code example shows how to implement the copy constructor method. It also implements a static copy factory method that essentially does the same thing as the copy constructor method.
    // The problem with the copy constructors is their maintenance, i.e., if an object is structually modified, you have to modify the copy constructor.
    public class CopyObjectConstructor
    {
        public string str;
        public CopyObjectConstructor() {}

        // copy constructor
        public CopyObjectConstructor(CopyObjectConstructor other)
        {
            this.str = other.str;
        }
        // copy factory
        public static CopyObjectConstructor GetInstance(CopyObjectConstructor x)
        {
            return new CopyObjectConstructor(x);
        }
    }
}
