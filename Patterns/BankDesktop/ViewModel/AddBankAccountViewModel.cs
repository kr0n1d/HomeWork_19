using System;
using BankDesktop.Core;
using BankDesktop.Model;
using BankLibrary;
using BankLibrary.Clients;

namespace BankDesktop.ViewModel
{
    public class AddBankAccountViewModel : ObservableObject
    {
        #region Свойства

        private Client client;
        public Client Client
        {
            get => client;
            set => Set(ref client, value);
        }

        private decimal balance = 1000;
        public decimal Balance
        {
            get => balance;
            set => Set(ref balance, Math.Round(value, 2));
        }

        private bool isCapitalization;
        public bool IsCapitalization
        {
            get => isCapitalization;
            set => Set(ref isCapitalization, value);
        }


        public Action<object> OnClose { get; set; }

        #endregion

        #region Команды

        public RelayCommand CloseApplicationCommand { get; set; }
        public RelayCommand AddBankAccountCommand { get; set; }

        #endregion

        #region Конструкторы

        public AddBankAccountViewModel(Client client)
        {
            Initialize();
            Client = client;
        }

        #endregion

        #region Методы

        private void Initialize()
        {
            CloseApplicationCommand = new RelayCommand(o => OnClose?.Invoke(null));
            AddBankAccountCommand = new RelayCommand(o =>
            {
                var bankAccount = new BankAccount(Bank.Instance.CurrentDate, balance, isCapitalization) { ClientId = client.Id};
                Bank.Instance.AddBankAccount(bankAccount);
                OnClose(null);
            });
        }

        #endregion
    }

}
