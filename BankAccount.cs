using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public abstract class BankAccount
{
    public int AccountNumber { get; set; }
    public int UserId { get; set; }//UserId to link with user calass
    public User? AccountHolder { get; set; }//Refrence to the user object
    public int AccountType { get; set; }
    public double Balance { get; set; }
    public List<Transaction> Transactions { get; private set; } = new List<Transaction>();
    public delegate void TransactionHandler(string message); // Delegate for transaction notifications
    public event TransactionHandler? OnTransaction; //Evnet for logging
    public abstract void Deposit(double amount); // Abstract method, must be implemented in derived classes
    public abstract void Withdraw(double amount);
    protected void RecordTransaction(TrnasactionType type, double amount)
    {
        Transaction transaction = new Transaction
        {
            TransactionId = Transactions.Count + 1,
            TransactionDate = DateTime.Now,
            AccountNumber = AccountNumber,
            Type = type,
            Amount = amount,
            BalanceAfterTransaction = Balance
        };
        Transactions.Add(transaction);
        OnTransaction?.Invoke($"Transaction recorded: {transaction.TransactionDetail()}"); // Notify subscribers about the transaction
    }
}

public class SavingAccount : BankAccount
{
    private const double InterestRate = 0.03; // 3% interest rate

    public SavingAccount()
    {
        AccountType = 1;
    }
    public override void Deposit(double amount)
    {
        Balance += amount + (amount * InterestRate);
        Console.WriteLine($"Deposited {amount} to Saving Account. New Balance: {Balance}");
        RecordTransaction(TrnasactionType.Deposit, amount); // Record the transaction
    }
    public override void Withdraw(double amount)
    {
        if (amount <= Balance)
        {
            Balance -= amount;
            Console.WriteLine($"Withdrew {amount} from Saving Account. New Balance: {Balance}");
            RecordTransaction(TrnasactionType.Withdraw, amount); // Record the transaction
        }
        else
        {
            throw new InsufficientBalanceException("Insufficient balance for withdrawal.");
        }
    }
}

public class CurrentAccount : BankAccount
{
    public CurrentAccount()
    {
        AccountType = 2;
    }
    public override void Deposit(double amount)
    {
        Balance += amount; // Implementation for current account deposit
        Console.WriteLine($"Deposited {amount} to Current Account. New Balance: {Balance}");
        RecordTransaction(TrnasactionType.Deposit, amount); // Record the transaction
    }
    public override void Withdraw(double amount)
    {
        if (amount <= Balance)
        {
            Balance -= amount; // Implementation for current account withdrawal
            Console.WriteLine($"Withdrew {amount} from Current Account. New Balance: {Balance}");
            RecordTransaction(TrnasactionType.Withdraw, amount); // Record the transaction
        }
        else
        {
            throw new InsufficientBalanceException("Insufficient balance for withdrawal.");
        }
    }
}

public class BankAccountFactory
{
    private static HashSet<int> accountNumbers = new HashSet<int>(); // Store unique account numbers
    static List<BankAccount> bankAccounts = new List<BankAccount>();
    static int GenerateAccountNumber()
    {
        Random random = new Random();
        int accountNumber;
        do
        {
            accountNumber = random.Next(10000, 99999);
        } while (accountNumbers.Contains(accountNumber));
        return accountNumber;
    }
    public static BankAccount? AccountDetail(int accountNumber, string userName)
    {
        BankAccount? account = bankAccounts.FirstOrDefault(acc => acc.AccountNumber == accountNumber && acc.AccountHolder?.UserName == userName);
        if (account != null)
        {
            return account;
        }
        else
        {
            Console.Write("\nAccount does not exist!");
            return null;
        }
    }
    public static void ViewAccountDetail(int accountNumber, string userName)
    {
        BankAccount? account = bankAccounts.FirstOrDefault(acc => acc.AccountNumber == accountNumber && acc.AccountHolder?.UserName == userName);
        if (account != null)
        {
            AccountDetail(account);
        }
        else
        {
            Console.Write("\nAccount does not exist!");
        }
    }
    public static void AccountDetail(BankAccount account)
    {
        Console.Write("\nAccount Details: ");
        Console.Write($"\nAccountNumber: {account.AccountNumber}\tUserName: {account.AccountHolder?.UserName}\tAccountType: {(AccountType)account.AccountType}\tBalanceAvailable: {account.Balance}\n");
    }
    public static BankAccount CreateAccount(int accountType, double initialDeposit, User user)
    {
        BankAccount newAccount;
        if (accountType == 1)
        {
            newAccount = new SavingAccount
            {
                AccountNumber = GenerateAccountNumber(),
                UserId = user.UserId,
                AccountHolder = user,
                Balance = initialDeposit
            };
        }
        else if (accountType == 2)
        {
            newAccount = new CurrentAccount
            {
                AccountNumber = GenerateAccountNumber(),
                UserId = user.UserId,
                AccountHolder = user,
                Balance = initialDeposit
            };
        }
        else
        {
            throw new ArgumentException("Invalid account type!");
        }
        bankAccounts.Add(newAccount);
        return newAccount;
    }
    public static void RemoveAccount(int accountNumber, string userName)
    {
        //find the account
        BankAccount? account = bankAccounts.FirstOrDefault(acc => acc.AccountNumber == accountNumber && acc?.AccountHolder?.UserName == userName);
        if (account != null)
        {
            //temporarily store the account and user for rollback
            User? user = account.AccountHolder;
            //attempt to remove the account
            bool isAccountRemoved = bankAccounts.Remove(account);
            if (isAccountRemoved)
            {
                //attempt to remove the user
                bool isUserRemoved = false;

                if (account?.AccountHolder != null)
                {
                    isUserRemoved = UserFactory.RemoveUser((int)account.AccountHolder.UserId);
                    if (!isUserRemoved)
                    {
                        bankAccounts.Add(account);
                        throw new Exception("Failed to remove user associated with the account. Rolled back: account restored.");
                    }
                    else
                    {
                        Console.Write("\nSuccessfully deleted the account and associated user.");
                    }
                }
            }
            else
            {
                Console.Write("\nFailed to remove the account.");
            }
        }
        else
        {
            Console.Write("\nAccount does not exist!");
        }
    }
    public static void Deposit(BankAccount account, double amount)
    {
        if (account != null)
        {
            account.Deposit(amount);
        }
    }
    public static void Withdraw(BankAccount account, double amount)
    {
        if (account != null)
        {
            account.Withdraw(amount);
        }
    }
    public static void Transfer(BankAccount debiter, BankAccount crediter, double amount)
    {
        try
        {
            debiter.Withdraw(amount);
            try
            {
                crediter.Deposit(amount);

            }
            catch (Exception ex)
            {
                //Rollback: re-deposit the amount back into the debiter's account
                debiter.Deposit(amount);
                Console.Write($"\nTransfer failed: {ex.Message}. Rolled back withdrawal.");
            }
        }
        catch (InsufficientBalanceException ex)
        {
            //hanlde insufficient balance in the debit account
            Console.Write($"\nTransfer failed: {ex.Message}");
        }


    }
}
