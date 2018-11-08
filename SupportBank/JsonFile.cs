using System.Collections.Generic;
using Newtonsoft.Json;
using NLog;

namespace SupportBank
{
    class JsonFile:ITransactionsFile
    {
        private static readonly ILogger logger = NLog.LogManager.GetCurrentClassLogger();
        private string _fileLocation;
        private string _fileSerialized;

        public JsonFile(string fileLocation)
        {
            _fileLocation = fileLocation;
            _fileSerialized = System.IO.File.ReadAllText(_fileLocation);
        }

        public List<Transaction> TransactionLog => JsonConvert.DeserializeObject<List<Transaction>>(_fileSerialized);
    }
}