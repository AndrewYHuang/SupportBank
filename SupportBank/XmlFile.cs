using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using NLog.Targets;

namespace SupportBank
{
    class XmlFile:ITransactionsFile
    {
        private static readonly ILogger logger = NLog.LogManager.GetCurrentClassLogger();
        private string _fileLocation;
        private XmlTransactionList _fileXData;

        public XmlFile(string fileLocation)
        {
            _fileLocation = fileLocation;
            XmlSerializer serializer = new XmlSerializer(typeof(XmlTransactionList));
            StreamReader reader = new StreamReader(fileLocation);
            _fileXData = (XmlTransactionList)serializer.Deserialize(reader);
            Console.WriteLine(DateTime.FromOADate(_fileXData.XmlTransactions[0].Date));
            reader.Close();
        }

        public List<Transaction> TransactionLog
        {
            get
            {
                throw new NotImplementedException();

            }
        }
    }

    public class XmlTransaction
    {
        [XmlAttribute("Date")]
        public int Date;
        [XmlElement("Description")]
        public string Narrative;
        [XmlElement("Value")]
        public double Amount;
        [XmlElement("Parties")]
        public XmlTransactionParties Parties;
    }

    [XmlRoot("TransactionList")]
    public class XmlTransactionList
    {
        [XmlElement("SupportTransaction")]
        public XmlTransaction[] XmlTransactions;
    }

    public class XmlTransactionParties
    {
        public string From;
        public string To;
    }
}