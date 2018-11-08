using System;

namespace SupportBank
{
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
}