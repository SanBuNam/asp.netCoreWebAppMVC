using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.General
{
    /// <summary>
    /// SOLID object-oriented principle
    /// https://www.tutorialsteacher.com/ioc/inversion-of-control
    /// </summary>
    /*
     Inversion of Control (IoC) is a design principle (although, some people refer to it as a pattern). As the name suggests, 
    it is used to invert different kinds of controls in object-oriented design to achieve loose coupling. 
    Here, controls refer to any additional responsibilities a class has, other than its main responsibility. 
    This include control over the flow of an application, and control over the flow of an object creation or dependent object creation and binding.
     */
    public class CustomerBusinessLogic
    {
        DataAccess _dataAccess;

        public CustomerBusinessLogic()
        {
            _dataAccess = new DataAccess();
        }

        public string GetCustomerName(int id)
        {
            return _dataAccess.GetCustomerName(id);
        }
    }

    public class DataAccess
    {
        public DataAccess()
        {
        }

        public string GetCustomerName(int id)
        {
            return "Dummy Customer Name"; // get it from DB in real app
        }
    }

    /*
     CustomerBusinessLogic and DataAccess classes are tightly coupled classes. So, changes in the DataAccess class will lead to changes in the CustomerBusinessLogic class. 
    For example, if we add, remove or rename any method in the DataAccess class then we need to change the CustomerBusinessLogic class accordingly.
    Suppose the customer data comes from different databases or web services and, in the future, we may need to create different classes, so this will lead to changes in the CustomerBusinessLogic class.
    The CustomerBusinessLogic class creates an object of the DataAccess class using the new keyword. There may be multiple classes which use the DataAccess class and create its objects. 
    So, if you change the name of the class, then you need to find all the places in your source code where you created objects of DataAccess and make the changes throughout the code. 
    This is repetitive code for creating objects of the same class and maintaining their dependencies.
    Because the CustomerBusinessLogic class creates an object of the concrete DataAccess class, it cannot be tested independently (TDD). 
    The DataAccess class cannot be replaced with a mock class.
     */

    // Let's use the Factory pattern to implement IoC in the above example, as the first step towards attaining loosely coupled classes.
    public class DataAccessFactory
    {
        public static DataAccess GetDataAccessObj()
        {
            return new DataAccess();
        }
    }
    // Now, use this DataAccessFactory class in the CustomerBusinessLogic class to get an object of DataAccess class.
    public class CustomerBusinessLogic2
    {
        public CustomerBusinessLogic2()
        {
        }

        public string GetCustomerName(int id)
        {
            DataAccess _dataAccess = DataAccessFactory.GetDataAccessObj();
            return _dataAccess.GetCustomerName(id);
        }
    }

    /*
     As you can see, the CustomerBusinessLogic class uses the DataAccessFactory.GetCustomerDataAccessObj() method to get an object of the DataAccess class instead of creating it using the new keyword. 
    Thus, we have inverted the control of creating an object of a dependent class from the CustomerBusinessLogic class to the DataAccessFactory class.
    This is a simple implementation of IoC and the first step towards achieving fully loose coupled design. As mentioned in the previous chapter, 
    we will not achieve complete loosely coupled classes by only using IoC. Along with IoC, we also need to use DIP, Strategy pattern, and DI (Dependency Injection).
     */


    // Dependency Inversion Principle (DIP)
    // implement the Dependency Inversion Principle as the second step to achieve loosely coupled classes
    // DIP Definition
    // 1. High-level modules should not depend on low-level modules. Both should depend on the abstraction.
    // 2. Abstractions should not depend on details. Details should depend on abstractions.
    //public class CustomerBusinessLogic
    //{
    //    public CustomerBusinessLogic()
    //    {
    //    }

    //    public string GetCustomerName(int id)
    //    {
    //        DataAccess _dataAccess = DataAccessFactory.GetDataAccessObj();

    //        return _dataAccess.GetCustomerName(id);
    //    }
    //}

    //public class DataAccessFactory
    //{
    //    public static DataAccess GetDataAccessObj()
    //    {
    //        return new DataAccess();
    //    }
    //}

    //public class DataAccess
    //{
    //    public DataAccess()
    //    {
    //    }

    //    public string GetCustomerName(int id)
    //    {
    //        return "Dummy Customer Name"; // get it from DB in real app
    //    }
    //}
    // We implemented the factory pattern to achieve IoC. But, the CustomerBusinessLogic class uses the concrete DataAccess class.
    // Therefore, it is still tightly coupled, even though we have inverted the dependent object creation to the factory class.
    // Let's use DIP on the CustomerBusinessLogic and DataAccess classes and make them more loosely coupled.
    /*
    As per the DIP definition, a high-level module should not depend on low-level modules. 
    Both should depend on abstraction. So, first, decide which is the high-level module (class) and the low-level module.
    A high-level module is a module which depends on other modules.
    In our example, CustomerBusinessLogic depends on the DataAccess class, so CustomerBusinessLogic is a high-level module and DataAccess is a low-level module.
    So, as per the first rule of DIP, CustomerBusinessLogic should not depend on the concrete DataAccess class, instead both classes should depend on abstraction.
    The second rule in DIP is "Abstractions should not depend on details. Details should depend on abstractions".
     */

    /*
     What is Abstraction? In programming terms, the above CustomerBusinessLogic and DataAccess are concreate classes, meaning we can create objects of them.
    So, abstraction in programming means to create an interface or an abstract class which is non-concrete. 
    As per DIP, CustomerBusinessLogic (high-level module) should not depend on the concrete DataAccess class (low-level module).
    Both classes should depend on abstractions, meaning both classes should depend on an interface or an abstract class.

    Now, what should be in the interface (or in the abstract class)?
    As you can see, CustomerBusinessLogic uses the GetCustomerName() method of the DataAccess class
    (in real life, there will be many customer-related methods in the DataAccess class).
    So, let's declare the GetCustomerName(int id) method in the interface, as shown below.
     */
    public interface ICustomerDataAccess
    {
        string GetCustomerName(int id);
    }

    // Now, implement ICustomerDataAccess in the CustomerDataAccess class, as shown below (so, instead of the DataAccess class, let's define the new CustomerDataAccess class).
    public class CustomerDataAccess : ICustomerDataAccess
    {
        public CustomerDataAccess()
        {
        }

        public string GetCustomerName(int id)
        {
            return "Dummy Customer Name";
        }
    }

    // Now, we need to change our factory class which returns ICustomerDataAccess instead of the concrete DataAccess class, as shown below.
    public class DataAccessFactory3
    {
        public static ICustomerDataAccess GetCustomerDataAccessObj()
        {
            return new CustomerDataAccess();
        }
    }

    // Now, change the CustomerBusinessLogic class which uses ICustomerDataAccess instead of the concrete DataAccess, class as shown below.
    public class CustomerBusinesLogic
    {
        ICustomerDataAccess _custDataAccess;

        public CustomerBusinesLogic()
        {
            _custDataAccess = DataAccessFactory3.GetCustomerDataAccessObj();
        }

        public string GetCustomerName(int id)
        {
            return _custDataAccess.GetCustomerName(id);
        }
    }

    /*
     Thus, we have implemented DIP in our example where a high-level module (CustomerBusinessLogic) and low-level module (CustomerDataAccess) are dependent on an abstraction (ICustomerDataAccess).
    Also, the abstraction (ICustomerDataAccess) does not depend on details (CustomerDataAccess), but the details depend on the abstraction.
     */

    // The following is the complete DIP example discussed so far.
    //public interface ICustomerDataAccess
    //{
    //    string GetCustomerName(int id);
    //}

    //public class CustomerDataAccess : ICustomerDataAccess
    //{
    //    public CustomerDataAccess()
    //    {
    //    }

    //    public string GetCustomerName(int id)
    //    {
    //        return "Dummy Customer Name";
    //    }
    //}

    //public class DataAccessFactory
    //{
    //    public static ICustomerDataAccess GetCustomerDataAccessObj()
    //    {
    //        return new CustomerDataAccess();
    //    }
    //}

    //public class CustomerBusinessLogic
    //{
    //    ICustomerDataAccess _custDataAccess;

    //    public CustomerBusinessLogic()
    //    {
    //        _custDataAccess = DataAccessFactory.GetCustomerDataAccessObj();
    //    }

    //    public string GetCustomerName(int id)
    //    {
    //        return _custDataAccess.GetCustomerName(id);
    //    }
    //}

    /*
     The advantages of implementing DIP in the above example is that the CustomerBusinessLogic and CustomerDataAccess classes are loosely coupled classes 
    because CustomerBusinessLogic does not depend on the concrete DataAccess class, 
    instead it includes a reference of the ICustomerDataAccess interface.
    So now, we can easily use another class which implements ICustomerDataAccess with a different implementation.
     */
}
