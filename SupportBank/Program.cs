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
            FileLoad csvFile = new FileLoad(@"C:\Users\AYH\Documents\DodgyTransactions2015.csv");
            TransactionLog transactions = csvFile.GenerateTransactionLog();
            var accounts = transactions.CreateAccounts();

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "quit") return;

                if (input.StartsWith("list "))
                {
                    input = input.Remove(0, 5);
                    if (input == "all")
                    {
                        foreach (Account account in accounts)
                        {
                            Console.WriteLine(account);
                        }
                        
                    }

                    else
                    {
                        transactions.ListAccount(input);
                    }
                }

            } 
        
        }

    }

    class FileLoad
    {
        private string FileLocation;
        private string[] FileLines;

        FileLoad(string fileLocation)
        {
            FileLocation = fileLocation;
            FileLines = System.IO.File.ReadAllLines(@"C:\Users\AYH\Documents\DodgyTransactions2015.csv");
        }

        public TransactionLog GenerateTransactionLog()
        {
            TransactionLog transactions = new TransactionLog();
            foreach (string line in FileLines.Skip(1))
            {
                var entries = line.Split(',');
                DateTime date = DateTime.Parse(entries[0]);
                double value = double.Parse(entries[4]);
                transactions.AddTransaction(new Transaction(date, entries[1], entries[2], entries[3], value));
            }

            return transactions;
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

    class TransactionLog
    {
        private List<Transaction> _transactionList;

        public TransactionLog()
        {
            _transactionList = new List<Transaction>();
        }

        public void AddTransaction(Transaction transaction) => _transactionList.Add(transaction);


        public List<Account> CreateAccounts()
        {
            var accounts = new List<Account>();

            // Go through the transaction list
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

                // Create account with debited amount if account not found
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

                // Create account with credited amount if account not found
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
