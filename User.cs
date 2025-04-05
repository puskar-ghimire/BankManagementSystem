/*
    Abstract class in C# is a class that cannot be instantiated directly 
    and is ment to be inherited by other classes.
    It can contain abstract methods (methods without implementation) that must be overridden in 
    derived classes, as well as non-abstract (concreate) methods, events and fields.

    Abstract classes are used to provide a base for other classes to build upon,
    allowing for code reuse and a clear structure in the class hierarchy.

    Why Abstract class?
    1.Enforces a structure: Ensures that derived classes implement the necessary methods.
    2.Provides common functionality: Can have concrete methods that all child classes inherit.
    3.Supports OOP principle: Helps in abstraction and polymorphism.

    Use Abstract clases when you need common functionality + mandatory methods.
*/
public abstract class User
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public int userType { get; set; }
    public abstract void DisplayInfo(); // Abstract method, must be implemented in derived classes
}

class Customer : User
{
    public Customer()
    {
        userType = 2;
    }
    public List<BankAccount> Accounts { get; set; } = new List<BankAccount>();
    public override void DisplayInfo()
    {
        Console.WriteLine($"Customer ID: {UserId}, Name: {UserName}");
    }
}
class SystemUser : User
{
    public SystemUser()
    {
        userType = 1;
    }
    public override void DisplayInfo()
    {
        Console.WriteLine($"System User ID: {UserId}, Name: {UserName}");
    }
}

public class UserFactory
{
    public static int lastUserId = 0;//keep the track of the last assigned userid
    public static List<User> users = new List<User>();
    public static User CreateUser(int userType, string userName)
    {
        lastUserId++;
        User newUser;
        if (userType == 1)
        {
            newUser = new SystemUser
            {
                UserId = lastUserId,
                UserName = userName
            };
        }
        else
        {
            newUser = new Customer
            {
                UserId = lastUserId,
                UserName = userName,
            };
        }
        users.Add(newUser);
        return newUser;
    }
    public static bool RemoveUser(int userid)
    {
        User? user = users.FirstOrDefault(u => u.UserId == userid);
        if (user != null)
        {
            return users.Remove(user);
        }
        return false;
    }
    public static User? UserDetail(int userid)
    {
        User? user = users?.FirstOrDefault(u => u.UserId == userid);
        if (user == null)
        {
            Console.WriteLine($"No user found for userid {userid}");
            return null;
        }
        return user;
    }
    public static void UserDetail(User user)
    {
        Console.WriteLine("\nUser Info:");
        Console.WriteLine($"UserID: {user.UserId}\tUserName: {user.UserName}\n");
    }
    public static bool IsUserExists(string userName)
    {
        return users.Any(u => u.UserName == userName);
    }
    public static void LinkAccountToUser(User user, BankAccount account)
    {
        if (user is Customer customer)
        {
            customer.Accounts.Add(account);
        }
    }
}