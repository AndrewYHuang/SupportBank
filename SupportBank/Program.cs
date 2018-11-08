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
        static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Program Started");


            ConsoleInterface.MainPrompt();
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

    static class Accountant
    {
        private static List<Account> _accountList;
        private static List<Transaction> _transactionList;

        public static void GenerateAccountsFromTransactions(IEnumerable<Transaction> transactionList)
        {
            _accountList = new List<Account>();
            
            foreach (Transaction transaction in transactionList)
            {
                bool[] accountsFound = Accountant.TryProcessTransaction(transaction);
                if (!accountsFound[0])
                    Accountant.AddOrModifyAccount(transaction.FromAccount, -transaction.Amount);
                if (!accountsFound[1])
                    Accountant.AddOrModifyAccount(transaction.ToAccount, transaction.Amount);
            }
        }

        private static void AddOrModifyAccount(string accountName, double creditChange)
        {
            foreach (var existingAccount in _accountList)
                if (existingAccount.Name == accountName)
                {
                    existingAccount.Credit += creditChange;
                }
            _accountList.Add(new Account(accountName, creditChange));
        }

        private static bool[] TryProcessTransaction(Transaction transaction)
        {
            bool[] accountsFound = {false,false};
            foreach (Account account in _accountList)
            {

                if (account.Name == transaction.FromAccount)
                {
                    account.Credit -= transaction.Amount;
                    accountsFound[0] = true;
                    break;
                }
            }

            foreach (Account account in _accountList)
            {
                if (account.Name == transaction.ToAccount)
                {
                    account.Credit += transaction.Amount;
                    accountsFound[1] = true;
                    break;
                }
            }
            return accountsFound;
        }

        public static void ListAccounts()
        {
            foreach (Account account in _accountList)
            {
                Console.WriteLine(account);
            }

        }

        public static void ListAccountTransactions(string name)
        {
            var nameFound = false;
            foreach (Transaction transaction in _transactionList)
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
    }

    static class ConsoleInterface
    {


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


                                //if (filename.EndsWith(".csv"))
                                //{
                                //    var csvFile = new CsvFile(@"C:\Users\AYH\Documents\DodgyTransactions2015.csv");
                                //    var transactions = csvFile.GenerateTransactionLog();
                                //    var accounts = Accountant.GenerateAccountsFromTransactions(transactions);
                                //}
                                //else if (filename.EndsWith(".json"))
                                //{
                                //    var jsonFile = new JsonFile(@"C:\Users\AYH\Documents\Transactions2013.json");
                                //    Program.TransactionList = jsonFile.GenerateTransactionLog();
                                //    Program.AccountList = Accountant.GenerateAccountsFromTransactions(Program.TransactionList);
                                //}
                                //else
                                //{
                                //    Console.WriteLine("File type not supported");
                                //}

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
                                Accountant.ListAccounts();
                            else if (argument == String.Empty)
                                Console.WriteLine("Please enter a name or \"all\" after list");
                            else
                                Accountant.ListAccountTransactions(argument);
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
}
