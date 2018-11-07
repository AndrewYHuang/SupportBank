using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            var pathToFile = @"C:\Users\AYH\Documents\Transactions2014.csv";
            var fileLines = System.IO.File.ReadAllLines(pathToFile);
            var transactions = new TransactionLog();
            foreach (string line in fileLines.Skip(1))
            {
                var entries = line.Split(',');
                transactions.AddTransaction(new Transaction(DateTime.Parse(entries[0]), entries[1], entries[2], entries[3], double.Parse(entries[4])));
            }
            transactions.ListAllTransactions();
            transactions.ListAccount("Gergana I");
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

    }

    internal class Account
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

    class TransactionLog
    {
        private List<Transaction> _transactionList;

        public TransactionLog()
        {
            _transactionList = new List<Transaction>();
        }

        public TransactionLog(List<Transaction> transactionList) => _transactionList = transactionList;

        public void AddTransaction(Transaction transaction) => _transactionList.Add(transaction);

        public void ListAllTransactions()
        {
            foreach (Transaction transaction in _transactionList)
            {
                Console.WriteLine(transaction);
            }
        }

        public List<Account> CreateAccounts()
        {
            var accounts = new List<Account>();
            foreach (Transaction transaction in _transactionList)
            {
                // Calculate credit deducted from senders
                var accountFound = false;
                foreach (Account account in accounts)
                {
                    if (account.Name == transaction.From)
                    {
                        account.Credit -= transaction.Amount;
                        accountFound = true;
                        break;
                    }
                }

                if (!accountFound)
                {
                    accounts.Add(new Account(transaction.From, -transaction.Amount));
                }

                // Calculate credit added to receivers
                accountFound = false;
                foreach (Account account in accounts)
                {
                    if (account.Name == transaction.To)
                    {
                        account.Credit += transaction.Amount;
                        accountFound = true;
                        break;
                    }
                }

                if (!accountFound)
                {
                    accounts.Add(new Account(transaction.To, transaction.Amount));
                }
            }
            return accounts;
        }

        public void ListAccount(string name)
        {
            foreach (Transaction transaction in _transactionList)
                if (transaction.From == name || transaction.To == name)
                    Console.WriteLine(transaction);
            
        }
    }

    class Transaction
    {
        public DateTime Date;
        public string From;
        public string To;
        public string Narrative;
        public double Amount;

        public Transaction(DateTime date, string from, string to, string narrative, double amount)
        {
            Date = date;
            From = from;
            To = to;
            Narrative = narrative;
            Amount = amount;
        }

        public override string ToString() => $"{Date.ToShortDateString()}, {From}, {To}, {Narrative}, {Amount}";
    }
}
