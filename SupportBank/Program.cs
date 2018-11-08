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
            /*
            var csvFile = new CsvFile(@"C:\Users\AYH\Documents\DodgyTransactions2015.csv");
            var transactions = csvFile.GenerateTransactionLog();
            var accounts = AccountFactory.CreateAccounts(transactions);
            */
            var jsonFile = new JsonFile(@"C:\Users\AYH\Documents\Transactions2013.json");
            var transactions = jsonFile.GenerateTransactionLog();
            var accounts = AccountFactory.CreateAccounts(transactions);
            
            while (true)
            {
                var option = ConsoleInterface.UserMainPrompt(out var name);
                switch (option)
                {
                    case 'f':
                    {
                        break;
                    }
                    case 'q':
                        return;
                    case 'l':
                    {
                        ConsoleInterface.ListAccounts(accounts);
                        break;
                    }
                    case 'a':
                    {
                        ConsoleInterface.ListAccountTransactions(transactions, name);
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
                    logger.Error($"The line \" {line} \" is formatted badly: {e}");
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
    static class AccountFactory
    {
        public static List<Account> CreateAccounts(IEnumerable<Transaction> transactionList)
        {
            var accounts = new List<Account>();

            // Go through the transaction list
            foreach (Transaction transaction in transactionList)
            {
                // Calculate credit deducted from senders
                var accountFound = false;
                foreach (Account account in accounts)
                {
                    if (account.Name == transaction.FromAccount)
                    {
                        account.Credit -= transaction.Amount;
                        accountFound = true;
                        break;
                    }
                }

                // Create account with debited amount if account not found
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

                // Create account with credited amount if account not found
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
            foreach (Transaction transaction in transactionList)
                if (transaction.FromAccount == name || transaction.ToAccount == name)
                    Console.WriteLine(transaction);
            
        }

        public static char UserMainPrompt(out string name)
        {
            while (true)
            {
                name = "";
                string input = Console.ReadLine();

                if (input == "q" || input == "quit" || input == "exit") return 'q';

                if (input.StartsWith("list "))
                {
                    input = input.Remove(0, 5);

                    if (input == "all") // "list all"
                        return 'l';

                    name = input; // otherwise it's "list [name]"
                    return 'a';
                }
            }

        }
    }
}
