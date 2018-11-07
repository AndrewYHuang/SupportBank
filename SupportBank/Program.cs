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
            var transactions = new List<Transaction>();
            foreach (string line in fileLines.Skip(1))
            {
                Console.WriteLine(line);

            }

            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }
    }

    class Transaction
    {
        private DateTime date;
        private string from;
        private string to;
        private string narrative;
        private int amount;

        public Transaction()
        {
            date = new DateTime();
            from = "";
            to = "";
            narrative = "";
            amount = 0;
        }

        public Transaction(string date, string from, string to, string narrative, int amount)
        {
            this.date = new DateTime();
            this.from = from;
            this.to = to;
            this.narrative = narrative;
            this.amount = amount;
        }

        public void toString()
        {

        }
    }
}
