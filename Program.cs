using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization.Metadata;
class Program
{
    bool isValid;
    char yesOrNo;
    string? input;
    double amount;
    public void EnterUserName(out string userName)
    {
        Console.Write("\nEnter Account Holder Name: ");
        userName = Console.ReadLine() ?? string.Empty;

        while (string.IsNullOrEmpty(userName))
        {
            Console.Write("\nPlease enter a valid account holder name: ");
            userName = Console.ReadLine() ?? string.Empty;
        }
    }
    public void EnterAccountNumber(out int accountNumber)
    {
        Console.Write("\nEnter Account Number: ");
        while (true)
        {
            input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out accountNumber))
            {
                Console.Write("\nAccount Number must contain only numbers: ");
                continue;
            }
            if (input.Length != 5)
            {
                Console.Write("\nPlease enter a valid account number with 5 digits: ");
                continue;
            }
            break;
        }
    }
    public void EnterAccountNumberAndUserName(out int accountNumber, out string userName)
    {
        EnterAccountNumber(out accountNumber);
        EnterUserName(out userName);
    }
    public void ChooseAccountType(out int accountType)
    {
        Console.Write("\nChoose account type: 1 for Saving Account, 2 for Current Account: ");
        do
        {
            input = Console.ReadLine()?.Trim() ?? string.Empty;
            if (int.TryParse(input, out accountType) && (accountType == 1 || accountType == 2))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
                Console.WriteLine("\nInvalid input! Please press '1' for saving or '2' for current account: ");
            }

        } while (!isValid);
        // charInput = Console.Read();
        // char keyPressed = (char)charInput;
        // // ConsoleKeyInfo keyInfo = Console.ReadKey();
        // // char keyPressed = keyInfo.KeyChar;
        // isValid = keyPressed == '1' || keyPressed == '2';
        // if (isValid)
        //     accountType = keyPressed - '0';
        // else
        // {
        //     continue;
        // }
    }
    public double ValidateAmount()
    {
        while (true)
        {
            input = Console.ReadLine();
            isValid = double.TryParse(input, out amount);
            if (!isValid)
            {
                Console.Write("\nInvalid input! Please enter a valid amount: ");
                continue;
            }
            break;
        }
        return amount;
    }
    public char ChooseYesOrNo()
    {
        do
        {
            input = Console.ReadLine()?.ToLower() ?? string.Empty;
            if (input.Length == 1)
            {
                yesOrNo = input[0];
                isValid = yesOrNo == 'y' || yesOrNo == 'n';
            }
            else
            {
                isValid = false;
                Console.Write("\nInvalid input! Please enter 'y' or 'n': ");
            }
        } while (!isValid);
        return yesOrNo;
    }
    public void TransactionAborted()
    {
        Console.Write("\nTransaction Aborted! Please Try Again.");
    }
    public void ExitMessage()
    {
        Console.Write("\nThank you for using Banking system.");
        return;
    }
    public void Deposit()
    {
        string userName;
        int accountNumber;
        EnterAccountNumberAndUserName(out accountNumber, out userName);
        Console.Write("\nEnter the amount to deposit: ");
        amount = ValidateAmount();
        BankAccount? account = BankAccountFactory.AccountDetail(accountNumber, userName);
        if (account is null)
            TransactionAborted();
        else
            BankAccountFactory.Deposit(account, amount);
    }
    public void Withdraw()
    {
        string userName;
        int accountNumber;
        EnterAccountNumberAndUserName(out accountNumber, out userName);
        Console.Write("\nEnter the amount to withdraw: ");
        amount = ValidateAmount();
        BankAccount? account = BankAccountFactory.AccountDetail(accountNumber, userName);
        if (account is null)
            TransactionAborted();
        else
            BankAccountFactory.Withdraw(account, amount);
    }
    public void Trnasfer()
    {
        string debiter;
        string crediter;
        int debitAccountNumber;
        int creditAccountNumber;
        Console.Write("\nEnter Transferring Account Details:");
        EnterAccountNumberAndUserName(out debitAccountNumber, out debiter);
        Console.Write("\nEnter the amount to transfer: ");
        amount = ValidateAmount();
        BankAccount? debitAccount = BankAccountFactory.AccountDetail(debitAccountNumber, debiter);
        if (debitAccount is null)
        {
            TransactionAborted();
            return;
        }
        EnterAccountNumberAndUserName(out creditAccountNumber, out crediter);
        BankAccount? creditAccount = BankAccountFactory.AccountDetail(creditAccountNumber, crediter);
        if (creditAccount is null)
        {
            TransactionAborted();
            return;
        }
        BankAccountFactory.Transfer(debitAccount, creditAccount, amount);

    }
    public void UpdateAccountInfo()
    {
        string userName;
        int accountNumber;
        Console.Write("\nEnter the account details of which you want to update");
        EnterAccountNumberAndUserName(out accountNumber, out userName);
        BankAccount? account = BankAccountFactory.AccountDetail(accountNumber, userName);
        if (account is null)
        {
            Console.Write("\nCannot update the account detail which do not exist.");
            return;
        }
        User? userDetails = account.AccountHolder != null
            ? UserFactory.UserDetail(account.AccountHolder.UserId)
            : null;
        if (userDetails != null)
        {
            while (true)
            {

                Console.Write("\nWhat you want to update? \n" +
                                "1.Account Holder Name\n" +
                                "2.Account Type\n" +
                                "3.Do Nothing");
                int[] validOptions = { 1, 2 };
                Console.Write("\nSequentially enter the number correspoing to the action you want to perfom from the above list: ");
                string? input = (Console.ReadLine() ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int choice))
                {
                    continue;
                }
                switch (choice)
                {
                    case 1:
                        string newUserName;
                        EnterUserName(out newUserName);
                        if (userDetails.UserName != newUserName)
                        {
                            userDetails.UserName = newUserName;
                            Console.Write("\nAccount Holder Name updated successfully.");
                        }
                        else
                        {
                            Console.Write("\nNew Account holder name is same as the old name. No changes were made.");
                        }
                        break;
                    case 2:
                        int newAccountType;
                        ChooseAccountType(out newAccountType);
                        if (account.AccountType != newAccountType)
                        {
                            account.AccountType = newAccountType;
                            Console.Write("\nAccount type udpated successfully.");
                        }
                        else
                        {
                            Console.Write("\nNew Account type is same as old type. No changes were made.");
                        }
                        break;
                    case 3:
                        Console.Write("\nNo further updates. Exiting..");
                        return;
                    default:
                        Console.Write("\nInvalid option! Please select a valid option.");
                        break;

                }
            }
        }
        else
        {
            Console.Write("\nUser details could not be found.");
        }
    }
    public void RemoveAccount()
    {
        Console.Write("\nEnter Account Number and Account holder name to reomve account.");
        string userName;
        int accountNumber;
        EnterAccountNumberAndUserName(out accountNumber, out userName);
        BankAccountFactory.RemoveAccount(accountNumber, userName);
    }
    public void ViewAccountDetails()
    {
        Console.Write("\nEnter Account Number and Account holder name to view the details");
        string userName;
        int accountNumber;
        EnterAccountNumberAndUserName(out accountNumber, out userName);
        BankAccountFactory.ViewAccountDetail(accountNumber, userName);
    }
    public void TransactionSelectionMethod(bool accountExist = false)
    {
        bool userHasAccount = accountExist;
        while (true)
        {
            int option;
            int[] validOptions;
            if (userHasAccount)
            {
                validOptions = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                Console.Write("\nWhat do you want to perfom ? \n" +
                                    "1.Deposit Balance\n" +
                                    "2.Withdraw Balance\n" +
                                    "3.Transfer Balance\n" +
                                    "4.Update Account Information\n" +
                                    "5.Create New Account\n" +
                                    "6.Remove Account\n" +
                                    "7.View Account Details");
            }
            else
            {
                validOptions = new int[] { 1, 2 };
                Console.Write("\nWhat do you want to perfom ? \n" +
                                    "1.Create New Account\n" +
                                    "2.Deposit Balance\n");
            }
            Console.Write("\nPlease enter the corresponding action number from above list: ");

            string input = Console.ReadLine() ?? string.Empty;
            do
            {
                if (!int.TryParse(input, out option))
                {
                    Console.Write("\nPlease enter a valid number form the list.");
                    input = Console.ReadLine() ?? string.Empty;
                }
                if (!validOptions.Contains(option))
                {
                    Console.Write("\nPlease select the valid option from the list.");
                    input = Console.ReadLine() ?? string.Empty;
                }
            } while (!validOptions.Contains(option));

            switch (option, userHasAccount)
            {
                case (1, true):
                    Deposit();
                    break;
                case (1, false):
                    CreateAccount();
                    userHasAccount = true;//update the flag since the user now has an account
                    break;
                case (2, true):
                    Withdraw();
                    break;
                case (2, false):
                    Deposit();
                    break;
                case (3, true):
                    Trnasfer();
                    break;
                case (4, true):
                    UpdateAccountInfo();
                    break;
                case (5, true):
                    CreateAccount();
                    break;
                case (6, true):
                    RemoveAccount();
                    break;
                case (7, true):
                    ViewAccountDetails();
                    break;
                case (_, _)://default
                    Console.WriteLine("Thank you for working with Banking system.");
                    break;
            }
            //Ask if user wants to perform another transaction
            Console.Write("\nDo you want to perfom another transaction (y/n)? ");
            char yesOrNo = ChooseYesOrNo();
            if (yesOrNo == 'n')
            {
                ExitMessage();
                break;//Exit the loop and end the program
            }
        }
    }
    public User CreateUser(string userName)
    {
        User newUser = UserFactory.CreateUser((int)UserType.Customer, userName);
        // UserFactory.UserDetail(newUser);
        return newUser;
    }
    public BankAccount CreateAccount()
    {
        string userName;
        double initialDeposit;
        Console.Write($"\nEnter the following details to create an account.");
        EnterUserName(out userName);
        ChooseAccountType(out int accountType);

        Console.Write("\nEnter an initial deposit (minimum 1000): ");
        while (true)
        {
            input = Console.ReadLine();
            if (!double.TryParse(input, out initialDeposit))
            {
                Console.Write("\nInvalid input, Please enter a valid amount: ");
                continue;
            }
            if (initialDeposit < 1000)
            {
                Console.Write("\nAmount must be at least 1000, Enter again: ");
                continue;
            }
            break;
        }
        User user = CreateUser(userName); // Ensure the return value is assigned
        BankAccount account = BankAccountFactory.CreateAccount(accountType, initialDeposit, user);
        UserFactory.LinkAccountToUser(user, account);
        BankAccountFactory.AccountDetail(account);
        return account;
    }
    public void AskForActionsToPerfom()
    {
        Console.Write("\nDo you want to perform any Transactions (y/n)? ");
        yesOrNo = ChooseYesOrNo();
        if (yesOrNo == 'y')
        {
            Console.Write("\nDo you have an account (y/n)?: ");
            yesOrNo = ChooseYesOrNo();
            if (yesOrNo == 'y')
            {
                TransactionSelectionMethod(true);
            }
            else
            {
                TransactionSelectionMethod(false);
            }
        }
        else
        {
            ExitMessage();
        }
    }
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Bank Mangement System.");
        Program program = new Program();
        program.AskForActionsToPerfom();
        return;
    }

}