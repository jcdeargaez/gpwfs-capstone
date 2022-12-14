using System.Collections.ObjectModel;
using System.Configuration;
using Capstone6.Domain;
using PropertyChanged;

namespace Capstone6
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        public Customer Owner { get; private set; }
        public Command<int> DepositCommand { get; private set; }
        public Command<int> WithdrawCommand { get; private set; }
        public int Balance { get; private set; }
        public ObservableCollection<Transaction> Transactions { get; private set; }
        private RatedAccount account;
        private readonly IBankApi bankApi;
        private Tuple<bool, int> TryParseInt(object value)
        {
            int output = 0;
            var parsed = Int32.TryParse(value as string, out output);
            return Tuple.Create(parsed, output);
        }

        private void UpdateAccount(RatedAccount newAccount)
        {
            this.account = newAccount;
            this.LoadTransactions();
            this.Balance = (int)account.Balance;
        }

        private void LoadTransactions()
        {
            Transactions.Clear();
            foreach (var txn in bankApi.LoadTransactionHistory(Owner))
                Transactions.Add(txn);
        }

        public MainViewModel()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BankAccountDB"].ConnectionString;
            bankApi = BankApi.BuildWithSqlRepository(connectionString);
            Owner = new Customer("juan");
            Transactions = new ObservableCollection<Transaction>();
            this.LoadTransactions();
            UpdateAccount(bankApi.LoadAccount(Owner));
            DepositCommand = new Command<int>(
                amount =>
                {
                    UpdateAccount(bankApi.Deposit(amount, Owner));
                    WithdrawCommand.Refresh();
                }, TryParseInt);
            WithdrawCommand = new Command<int>(
                amount => UpdateAccount(bankApi.Withdraw(amount, Owner)),
                TryParseInt,
                () => account.IsInCredit);
        }
    }
}
