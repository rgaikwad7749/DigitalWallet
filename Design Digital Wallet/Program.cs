using System;
using System.Collections.Generic;

namespace DigitalWallet
{
    class Wallet
    {
        public int AccountNumber { get; private set; }
        public string UserName { get; private set; }
        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; private set; }

        public Wallet(int accountNumber, string userName, decimal initialAmount)
        {
            AccountNumber = accountNumber;
            UserName = userName;
            Balance = initialAmount;
            Transactions = new List<Transaction>();
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
        }

        public bool Withdraw(decimal amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                return true;
            }
            return false;
        }
    }

    class Transaction
    {
        public int FromAccount { get; set; }
        public int ToAccount { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public Transaction(int fromAccount, int toAccount, decimal amount)
        {
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
            Date = DateTime.Now;
        }
    }

    class Program
    {
        static Dictionary<int, Wallet> wallets = new Dictionary<int, Wallet>();
        static int nextAccountNumber = 1;

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("OPTIONS:");
                Console.WriteLine("1. Create wallet");
                Console.WriteLine("2. Transfer Amount");
                Console.WriteLine("3. Account Statement");
                Console.WriteLine("4. Overview");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int option)) continue;

                switch (option)
                {
                    case 1:
                        CreateWallet();
                        break;
                    case 2:
                        TransferAmount();
                        break;
                    case 3:
                        AccountStatement();
                        break;
                    case 4:
                        Overview();
                        break;
                    case 5:
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        static void CreateWallet()
        {
            Console.Write("Enter name: ");
            string name = Console.ReadLine();

            Console.Write("Enter amount: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal initialAmount))
            {
                var wallet = new Wallet(nextAccountNumber, name, initialAmount);
                wallets[nextAccountNumber] = wallet;
                Console.WriteLine($"Account created for user {name} with account number {nextAccountNumber}");
                nextAccountNumber++;
            }
            else
            {
                Console.WriteLine("Invalid amount.");
            }
        }

        static void TransferAmount()
        {
            Console.Write("Enter SENDER account number: ");
            if (!int.TryParse(Console.ReadLine(), out int senderAccount)) return;

            Console.Write("Enter RECEIVER account number: ");
            if (!int.TryParse(Console.ReadLine(), out int receiverAccount)) return;

            Console.Write("Enter amount: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                if (wallets.TryGetValue(senderAccount, out Wallet sender) &&
                    wallets.TryGetValue(receiverAccount, out Wallet receiver))
                {
                    if (sender.Withdraw(amount))
                    {
                        receiver.Deposit(amount);
                        var transaction = new Transaction(senderAccount, receiverAccount, amount);
                        sender.Transactions.Add(transaction);
                        receiver.Transactions.Add(transaction);
                        Console.WriteLine("Transfer Successful");
                    }
                    else
                    {
                        Console.WriteLine("Insufficient balance.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid account numbers.");
                }
            }
        }

        static void AccountStatement()
        {
            Console.Write("Enter account number: ");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber) || !wallets.TryGetValue(accountNumber, out Wallet wallet)) return;

            Console.WriteLine($"Summary for account number: {accountNumber}");
            Console.WriteLine($"Current Balance: {wallet.Balance}");
            Console.WriteLine("Your Transaction History:");
            foreach (var transaction in wallet.Transactions)
            {
                Console.WriteLine($"[Transaction from={transaction.FromAccount}, to={transaction.ToAccount}, amount={transaction.Amount}, date={transaction.Date}]");
            }
        }

        static void Overview()
        {
            Console.WriteLine("All Accounts Overview:");
            foreach (var wallet in wallets.Values)
            {
                Console.WriteLine($"Balance for account number {wallet.AccountNumber}: {wallet.Balance}");
            }
        }
    }
}
