using System;
using System.Collections.Generic;

namespace SupportBank
{
    static class Accountant
    {
        private static List<Account> _accountList;
        private static List<Transaction> _transactionList;

        public static void NewAccountsFromTransactions(List<Transaction> transactionList)
        {
            _transactionList = transactionList;
            _accountList = new List<Account>();
            
            foreach (Transaction transaction in _transactionList)
            {
                bool[] accountsFound = Accountant.TryProcessTransaction(transaction);
                if (!accountsFound[0])
                    Accountant.AddOrModifyAccount(transaction.FromAccount, -transaction.Amount);
                if (!accountsFound[1])
                    Accountant.AddOrModifyAccount(transaction.ToAccount, transaction.Amount);
            }
        }

        private static void AddOrModifyAccount(string accountName, double creditChange)
        {
            foreach (var existingAccount in _accountList)
                if (existingAccount.Name == accountName)
                {
                    existingAccount.Credit += creditChange;
                }
            _accountList.Add(new Account(accountName, creditChange));
        }

        private static bool[] TryProcessTransaction(Transaction transaction)
        {
            bool[] accountsFound = {false,false};
            foreach (Account account in _accountList)
            {

                if (account.Name == transaction.FromAccount)
                {
                    account.Credit -= transaction.Amount;
                    accountsFound[0] = true;
                    break;
                }
            }

            foreach (Account account in _accountList)
            {
                if (account.Name == transaction.ToAccount)
                {
                    account.Credit += transaction.Amount;
                    accountsFound[1] = true;
                    break;
                }
            }
            return accountsFound;
        }

        public static void ListAccounts()
        {
            foreach (Account account in _accountList)
            {
                Console.WriteLine(account);
            }

        }

        public static void ListAccountTransactions(string name)
        {
            var nameFound = false;
            foreach (Transaction transaction in _transactionList)
                if (String.Equals(transaction.FromAccount, name, StringComparison.CurrentCultureIgnoreCase)
                    || String.Equals(transaction.ToAccount, name, StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine(transaction);
                    nameFound = true;
                }

            if (!nameFound)
            {
                Console.WriteLine("Account not found");
            }

        }
    }
}