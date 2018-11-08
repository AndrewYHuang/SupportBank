using System.Collections.Generic;

namespace SupportBank
{
    interface ITransactionsFile
    {
        List<Transaction> TransactionLog { get; }
    }
}