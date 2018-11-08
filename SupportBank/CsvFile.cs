using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace SupportBank
{
    class CsvFile:ITransactionsFile
    {
        private static readonly ILogger logger = NLog.LogManager.GetCurrentClassLogger();
        private string _fileLocation;
        private string[] _fileLines;

        public CsvFile(string fileLocation)
        {
            _fileLocation = fileLocation;
            _fileLines = System.IO.File.ReadAllLines(_fileLocation);
        }

        public List<Transaction> TransactionLog
        {
            get
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
    }
}