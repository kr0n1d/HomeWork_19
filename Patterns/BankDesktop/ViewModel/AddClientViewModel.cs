using System;
using System.Windows;
using BankDesktop.Core;
using BankDesktop.Model;
using BankLibrary;
using BankLibrary.Clients;

namespace BankDesktop.ViewModel
{
    public class AddClientViewModel : ObservableObject
    {
        #region Свойства

        private string fullName;
        public string FullName
        {
            get => fullName;
            set => Set(ref fullName, value);
        }

        private int indexClientType = 0;
        public int IndexClientType
        {
            get => indexClientType;
            set => Set(ref indexClientType, value);
        }

        private bool isVip;
        public bool IsVip
        {
            get => isVip;
            set => Set(ref isVip, value);
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


        #endregion

        #region Команды

        public RelayCommand AddClientCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }

        #endregion


        public Action<object> OnClose;


        #region Конструкторы

        public AddClientViewModel()
        {
            AddClientCommand = new RelayCommand(o =>
            {
                string type = string.Empty;
                Client client = 
                    new Individual(fullName, new BankAccount(DateTime.Now, balance, isCapitalization), isVip);
                if (indexClientType == 0)
                {
                    Bank.Instance.AddClient(client);
                    type = "физическое лицо";
                }
                else
                {
                    client =
                        new LegalEntity(fullName, new BankAccount(DateTime.Now, balance, isCapitalization), isVip);
                    Bank.Instance.AddClient(client);
                    type = "юридическое лицо";
                }
                OnClose?.Invoke(null);
            }, can => !string.IsNullOrWhiteSpace(FullName));
            CloseCommand = new RelayCommand(o => OnClose?.Invoke(null));
        }

        #endregion
    }
}
