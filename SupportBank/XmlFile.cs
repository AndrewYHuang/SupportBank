using System;
using System.Collections.Generic;
using NLog;

namespace SupportBank
{
    class XmlFile:ITransactionsFile
    {
        private static readonly ILogger logger = NLog.LogManager.GetCurrentClassLogger();
        private string _fileLocation;


        public XmlFile(string fileLocation)
        {
            _fileLocation = fileLocation;

        }

        public List<Transaction> TransactionLog
        {
            get { throw new NotImplementedException(); }
        }
    }
}