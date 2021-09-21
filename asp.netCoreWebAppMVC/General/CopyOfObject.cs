using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.General
{

    /*
     Object.MemberwiseClone Method is used to create a shallow copy or make clone of the current Object. 
    Shallow copy is a bit-wise copy of an object. In this case, a new object is created and that object has an exact copy of the existing object. 
    Basically, this method copies the non-static fields of the current object to the new object.

    If a field is a reference type, then the only reference is copied not the referred object. 
    So here, the cloned object and the original object will refer to the same object.
    If the field is value type then the bit-by-bit copy of the field will be performed.
     */

    // syntax : protected object MemberwiseClone ();
    // Returns : This method returns a Object, which is the shallow copy of existing Object.

    // c# program to clone a object
    // Using MemberwiseClone() method #1.
    class GFG1
    {
        public int val;
        public GFG1(int val)
        {
            this.val = val;
        }
    }

    class GFG2
    {
        public GFG1 gg;
        public GFG2(int val)
        {
            // copy the reference of GFG1 to gg
            this.gg = new GFG1(val);
        }

        // method for cloning
        public GFG2 Clone()
        {
            // return cloned value using MemberwiseClone() method
            return ((GFG2)MemberwiseClone());
        }
    }

    // Driver code
    class Geek
    {
        // Main method
        public static void GeekMain()
        {
            // object of Class GFG2 with a value 3
            GFG2 g = new GFG2(3);

            // calling Clone()
            // "cc" has the reference of Clone()
            GFG2 cc = g.Clone();

            // accessing the main value
            Console.WriteLine("Value: " + g.gg.val);
            // accessing the cloned value
            Console.WriteLine("cloned value: " + cc.gg.val);

            // set a new value
            // in variable "val"
            cc.gg.val = 6;

            // accessing the main value
            Console.WriteLine("\nValue: " + g.gg.val);
            // accessing the cloned value
            Console.WriteLine("cloned value: " + cc.gg.val);
        
            /*
             returns 
            Value: 3
            cloned value: 3
            Value: 6
            cloned value: 6
             */
        }
    }


    // Example2
    public class GFG : ICloneable
    {
        // data members
        public string Name;
        public string Surname;
        public int Age;

        // constructor
        public GFG(string name, string title, int age)
        {
            Name = name;
            Surname = title;
            Age = age;
        }

        // method for cloning
        public object Clone()
        {
            // return cloned value using MemberwiseClone() method
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("Name = {0}, Surname = {1}, Age {2}", Name, Surname, Age);
        }
    }

    // Driver class
    public class MainGFGClass
    {
        // Main Method
        public static void MainMethod()
        {
            GFG g = new GFG("ABC", "XYZ", 26);

            // calling Clone() "cg" has reference of Clone()
            GFG cg = (GFG)g.Clone();

            Console.WriteLine("For old values\nOriginal :");
            Console.WriteLine(g);

            Console.WriteLine("Cloned :");
            Console.WriteLine(cg);

            Console.WriteLine("\nAfter assigning new values");
            g.Name = "LMN";
            g.Surname = "QRS";
            g.Age = 13;

            Console.WriteLine("Original : ");
            Console.WriteLine(g);

            Console.WriteLine("Cloned : ");
            // prints the old cloned value
            Console.WriteLine(cg);

            /*
             For old values
            Original : 
            Name = ABC, Surname = XYZ, Age 26
            Cloned :
            Name = ABC, Surname = XYZ, Age 26

            After assigning new values
            Original : 
            Name = LMN, Surname = QRS, Age 13
            Cloned :
            Name = ABC, Surname = XYZ, Age 26
             */
        }
    }
}
