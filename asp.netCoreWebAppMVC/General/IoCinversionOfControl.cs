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
            return "Dummy Customer Name";
        }
    }

    /*
     CustomerBusinessLogic and DataAccess classes are tightly coupled classes. So, changes in the DataAccess class will lead to changes in the CustomerBusinessLogic class. 

    For example, if we add, remove or rename any method in the DataAccess class then we need to change the CustomerBusinessLogic class accordingly.
    Suppose the customer data comes from different databases or web services and, in the future, we may need to create different classes, so this will lead to changes in the CustomerBusinessLogic class.
    The CustomerBusinessLogic class creates an object of the DataAccess class using the new keyword. There may be multiple classes which use the DataAccess class and create its objects. 
    So, if you change the name of the class, then you need to find all the places in your source code where you created objects of DataAccess and make the changes throughout the code. 
    This is repetitive code for creating objects of the same class and maintaining their dependencies.

    Because the CustomerBusinessLogic class creates an object of the concrete DataAccess class, it can not be tested independently (TDD). 
    The DataAccess class cannot be replaced with a mock class.
     */

    // Let's use the Factory pattern to implement IoC in the above example, as the first step towards attaining loosely coupled classes.
    public class DataAccessIoC
    {
        public DataAccessIoC()
        {
        }

        public string GetCustomerName(int id)
        {
            return "Dummy Customer Name";
        }
    }

    public class DataAccessFactory
    {
        public static DataAccessIoC GetDataAccessObj()
        {
            return new DataAccessIoC();
        }
    }
    // Now, use this DataAccessFactory class in the CustomerBusinessLogic class to get an object of DataAccess class.
    public class BusinessLogicIoC
    {
        public BusinessLogicIoC()
        {
        }

        public string GetCustomerName(int id)
        {
            DataAccessIoC _dataAccess = DataAccessFactory.GetDataAccessObj();
            return _dataAccess.GetCustomerName(id);
        }
    }
    /*
     As you can see, the CustomerBusinessLogic class uses the DataAccessFactory.GetCustomerDataAccessObj() method to get an object of the DataAccess class instead of creating it using the new keyword. 
    Thus, we have inverted the control of creating an object of a dependent class from the CustomerBusinessLogic class to the DataAccessFactory class.
    This is a simple implementation of IoC and the first step towards achieving fully loose coupled design. As mentioned in the previous chapter, 
    we will not achieve complete loosely coupled classes by only using IoC. Along with IoC, we also need to use DIP, Strategy pattern, and DI (Dependency Injection).
     */

    // DIP (Dependency Inversion Principle) Definition
    // 1. High-level modules should not depend on low-level modules. Both should depend on the abstraction.
    // 2. Abstractions should not depend on details. Details should depend on abstractions.

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
    public interface IDataAccess
    {
        string GetCustomerName(int id);
    }
    // Now, implement ICustomerDataAccess in the CustomerDataAccess class, as shown below (so, instead of the DataAccess class, let's define the new CustomerDataAccess class).
    public class DataAccessDIP : IDataAccess
    {
        public DataAccessDIP()
        {
        }

        public string GetCustomerName(int id)
        {
            return "Dummy Customer Name";
        }
    }
    // Now, we need to change our factory class which returns ICustomerDataAccess instead of the concrete DataAccess class, as shown below.
    public class DataAccessFactoryDIP
    {
        public static IDataAccess GetCustomerDataAccessObj()
        {
            return new DataAccessDIP();
        }
    }
    // Now, change the CustomerBusinessLogic class which uses ICustomerDataAccess instead of the concrete DataAccess, class as shown below.
    public class BusinesLogicDIP
    {
        IDataAccess _custDataAccess;
        public BusinesLogicDIP()
        {
            _custDataAccess = DataAccessFactoryDIP.GetCustomerDataAccessObj();
        }

        public string GetCustomerName(int id)
        {
            return _custDataAccess.GetCustomerName(id);
        }
    }
    /*
     Thus, we have implemented DIP in our example where a high-level module (BusinessLogic) 
    and low-level module (DataAccess) are dependent on an abstraction (IDataAccess).
    Also, the abstraction (IDataAccess) does not depend on details (DataAccess), but the details depend on the abstraction.

     The advantages of implementing DIP in the above example is that the BusinessLogic and DataAccess classes are loosely coupled classes 
    because BusinessLogic does not depend on the concrete DataAccess class, 
    instead it includes a reference of the IDataAccess interface.
    So now, we can easily use another class which implements IDataAccess with a different implementation.
   
     Still, we have not achieved fully loosely coupled classes because the BusinessLogic class includes a factory class to get the reference of IDataAccess. 
    This is where the Dependency Injection pattern helps us. In the next chapter, we will learn how to use the Dependency Injection (DI) and the Strategy pattern using the above example.
    Implement Dependency Injection and strategy pattern together to move the dependency object creation completely out of the class.
    This is our third step in making the classes completely loose coupled.

     The problem with the above example is that we used DataAccessFactory inside the BusinessLogic class.
    So, suppose there is another implementation of IDataAccess and we want to use that new class inside BusinessLogic.
    Then, we need to change the source code of the BusinessLogic class as well.

    The Dependency injection pattern solves this problem by injecting dependent objects via a constructor, a property, or an interface.
    */

    /*
    Dependency Injection (DI) is a design pattern used to implement IoC.
    It allows the creation of dependent objects outside of a class and provides those objects to a class through different ways.
    Using DI, we move the creation and binding of the dependent objects outside of the class that depends on them.

    The Dependency Injection pattern involves 3 types of classes.
    1. Client Class: The client class (dependent class) is a class which depends on the service class.
    2. Service Class: The service class (dependency) is a class that provides service to the client class.
    3. Injector Class: The injector class injects the service class object into the client class.

    The injector class creates an object of the service class, and injects that object to a client object. 
    In this way, the DI pattern separates the responsibility of creating an object of the service class out of the client class.
    */

    /*
    Types of Dependency Injection
    The injector class injects dependencies broadly in three ways: through a constructor, through a property, or through a method.
    1. Constructor Injection: the injector supplies the service (dependency) through the client class constructor.
    2. Property Injection: (aka the Setter Injection), the injector supplies the dependency through a public property of the client class.
    3. Method Injection: the client class implements an interface which declares the method(s) to supply the dependency 
                            and the injector uses this interface to supply the dependency to the client class.
    */

    // Constructor Injection - Implement DI using the constructor
    public class BusinessLogicDIcon
    {
        IDataAccessDIconstructor _dataAccess;
        public BusinessLogicDIcon(IDataAccessDIconstructor dataAccessDIconstructor)
        {
            _dataAccess = dataAccessDIconstructor;
        }

        public BusinessLogicDIcon()
        {
            _dataAccess = new DataAccessDIcon();
        }

        public string ProcessCustomerData(int id)
        {
            return _dataAccess.GetCustomerName(id);
        }
    }

    public interface IDataAccessDIconstructor
    {
        string GetCustomerName(int id);
    }

    public class DataAccessDIcon : IDataAccessDIconstructor
    {
        public DataAccessDIcon()
        {
        }

        public string GetCustomerName(int id)
        {
            return "Dummy Customer Name";
        }
    }
    // In the above example, CustomerBusinessLogic includes the constructor with one parameter of type ICustomerDataAccess.
    // Now, the calling class must inject an object of ICustomerDataAccess.
    public class CustomerService
    {
        BusinessLogicDIcon _customerBL;

        public CustomerService()
        {
            _customerBL = new BusinessLogicDIcon(new DataAccessDIcon());
        }

        public string GetCustomerName(int id)
        {
            return _customerBL.ProcessCustomerData(id);
        }
    }
    /*
     As you can see in the above example, the CustomerService class creates and injects the CustomerDataAccess object into the CustomerBusinessLogic class.
    Thus, the CustomerBusinessLogic class doesn't need to create an object of CustomerDataAccess using the new keyword or using factory class.
    The calling class (CustomerService) creates and sets the appropriate DataAccess class to the CustomerBusinessLogic class.
    In this way, the CustomerBusinessLogic and CustomerDataAccess classes become "more" loosely coupled classes.
     */


    // Property Injection - the dependency is provided through a public property.


    // End Line
}
