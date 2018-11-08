using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLog;
using NLog.Config;
using NLog.Targets;

using Newtonsoft.Json;

namespace SupportBank
{
    class Program
    {
        public static List<Transaction> TransactionList;
        public static List<Account> AccountList;
        static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Program Started");

            while (true)
            {
                var command = ConsoleInterface.PromptCommand(out var argument);
                switch (command)
                {
                    case "import":
                        {
                            if (argument.ToLower().StartsWith("file "))
                            {
                                argument = argument.Split(' ')[1];
                                var filename = argument.Length == 1 ? String.Empty : argument;
                                if (filename.EndsWith(".csv"))
                                {
                                    var csvFile = new CsvFile(@"C:\Users\AYH\Documents\DodgyTransactions2015.csv");
                                    var transactions = csvFile.GenerateTransactionLog();
                                    var accounts = Accountant.GenerateAccountsFromTransactions(transactions);
                                }
                                else if (filename.EndsWith(".json"))
                                {
                                    var jsonFile = new JsonFile(@"C:\Users\AYH\Documents\Transactions2013.json");
                                    Program.TransactionList = jsonFile.GenerateTransactionLog();
                                    Program.AccountList = Accountant.GenerateAccountsFromTransactions(Program.TransactionList);
                                }
                                else
                                {
                                    Console.WriteLine("File type not supported");
                                }

                            }
                            break;
                        }
                    case "quit":
                    case "q":
                    case "exit":
                        return;
                    case "list":
                        {
                            if (argument == "all")
                                ConsoleInterface.ListAccounts(Program.AccountList);
                            else if (argument == String.Empty)
                                Console.WriteLine("Please enter a name or \"all\" after list");
                            else
                                ConsoleInterface.ListAccountTransactions(Program.TransactionList, argument);
                            break;
                        }
                    default:
                        {
                            break;
                        }

                }

            }

        }

    }

    class CsvFile
    {
        private static readonly ILogger logger = NLog.LogManager.GetCurrentClassLogger();
        private string _fileLocation;
        private string[] _fileLines;

        public CsvFile(string fileLocation)
        {
            _fileLocation = fileLocation;
            _fileLines = System.IO.File.ReadAllLines(_fileLocation);
        }

        public List<Transaction> GenerateTransactionLog()
        {
            
            List<Transaction> transactions = new List<Transaction>();
            foreach (string line in _fileLines.Skip(1))
            {

                try
                {
                    var entries = line.Split(',');
                    DateTime date = DateTime.Parse(entries[0]);
                    double value = double.Parse(entries[4]);
                    transactions.Add(new Transaction(date, entries[1], entries[2], entries[3], value));

                }

                catch (System.FormatException e)
                {
                    logger.Error($"The line \" {line} \" can't be parsed: {e}");
                    Console.WriteLine($"The line \"{line}\" is formatted badly, entry ignored");
                }
            }
            return transactions;
        }
    }
    class JsonFile
    {
        private static readonly ILogger logger = NLog.LogManager.GetCurrentClassLogger();
        private string _fileLocation;
        private string _fileSerialized;

        public JsonFile(string fileLocation)
        {
            _fileLocation = fileLocation;
            _fileSerialized = System.IO.File.ReadAllText(_fileLocation);
        }

        public List<Transaction> GenerateTransactionLog()
        {
            return JsonConvert.DeserializeObject<List<Transaction>>(_fileSerialized);
        }
    }
    class Account
    {
        public string Name;
        public double Credit;

        public Account(string name, double credit)
        {
            Name = name;
            Credit = credit;
        }

        public override string ToString() => $"{Name}, {Credit}";
    }
    class Transaction
    {
        public DateTime Date;
        public string FromAccount;
        public string ToAccount;
        public string Narrative;
        public double Amount;

        public Transaction(DateTime date, string fromAccount, string toAccount, string narrative, double amount)
        {
            Date = date;
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Narrative = narrative;
            Amount = amount;
        }

        public override string ToString() => $"{Date.ToShortDateString()}, {FromAccount}, {ToAccount}, {Narrative}, {Amount}";
    }

    public class AccountsDoNotExistException : Exception
    {
        public string[] accountNames;

        public AccountsDoNotExistException(string message, string[] accountNames) : base(message)
        {
            this.accountNames = accountNames;
        }
    }
    static class Accountant
    {
        private static List<Account> _accountList;
        private static List<Transaction> _transactionList;

        public static List<Account> GenerateAccountsFromTransactions(IEnumerable<Transaction> transactionList)
        {
            var accounts = new List<Account>();
            
            foreach (Transaction transaction in transactionList)
            {
                // Calculate credit deducted from senders
                var accountFound = false;
                try
                {
                    Accountant.ProcessTransaction(transaction);
                }
                catch (AccountsDoNotExistException e)
                {
                    foreach (var accountName in e.accountNames)
                    Accountant.AddAccount(accountName);
                }

                foreach (Account account in accounts)
                {
                    if (account.Name == transaction.FromAccount)
                    {
                        account.Credit -= transaction.Amount;
                        accountFound = true;
                        break;
                    }
                }
                // accountfound = accountfactory.deductfees(accounts,transaction)
                // if (!accountfound) 
                //      account.add()

                if (!accountFound)
                {
                    accounts.Add(new Account(transaction.FromAccount, -transaction.Amount));
                }

                // Calculate credit added to receivers
                accountFound = false;
                foreach (Account account in accounts)
                {
                    if (account.Name == transaction.ToAccount)
                    {
                        account.Credit += transaction.Amount;
                        accountFound = true;
                        break;
                    }
                }
                
                if (!accountFound)
                {
                    accounts.Add(new Account(transaction.ToAccount, transaction.Amount));
                }
            }
            return accounts;
        }
    }

    static class ConsoleInterface
    {
        public static void ListAccounts(List<Account> accounts)
        {
            foreach (Account account in accounts)
            {
                Console.WriteLine(account);
            }
        }
        public static void ListAccountTransactions(List<Transaction> transactionList,string name)
        {
            var nameFound = false;
            foreach (Transaction transaction in transactionList)
                if (String.Equals(transaction.FromAccount, name, StringComparison.CurrentCultureIgnoreCase) 
                    || String.Equals(transaction.ToAccount, name, StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine(transaction);
                    nameFound = true;
                }

            if (!nameFound)
            {
                Console.WriteLine("Account not found");
            }

        }

        public static string PromptCommand(out string arguments)
        {
            while (true)
            {
                Console.WriteLine("Please enter a command:");
                string[] input = Console.ReadLine().Split(new Char[]{' '}, 2, StringSplitOptions.None);

                arguments = input.Length == 1 ? String.Empty : input[1];
 
                return input[0].ToLower();
            }
        }

        public static void MainPrompt()
        {
            
        }
    }
}
