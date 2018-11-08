using System;
using NLog;

namespace SupportBank
{
    static class FileManager
    {
        private static readonly ILogger logger = NLog.LogManager.GetCurrentClassLogger();
        public static string LoadTransactionFile(string fileLocation)
        {
            ITransactionsFile loadedFile;
            string fileType;
            if (fileLocation.EndsWith(".csv"))
            {
                logger.Info("Loading CSV File");
                loadedFile = new CsvFile(fileLocation);
                fileType = "csv";
            }
            else if (fileLocation.EndsWith(".json"))
            {
                logger.Info("Loading JSON File");
                loadedFile = new JsonFile(fileLocation);
                fileType = "json";
            }
            else if (fileLocation.EndsWith(".xml"))
            {
                logger.Info("Loading XML File");
                loadedFile = new XmlFile(fileLocation);
                fileType = "xml";
            }
            else
            {
                Console.WriteLine("File type not supported");
                logger.Warn("Invalid file extension");
                return "none";
            }

            var transactions = loadedFile.TransactionLog;
            Accountant.NewAccountsFromTransactions(transactions);
            return fileType;
        }
    }
}