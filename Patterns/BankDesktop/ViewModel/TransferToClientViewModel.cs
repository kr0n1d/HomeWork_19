using System;
using System.Collections.ObjectModel;
using BankDesktop.Core;
using BankDesktop.Model;
using BankLibrary;
using BankLibrary.Clients;
using Newtonsoft.Json.Linq;

namespace BankDesktop.ViewModel
{
    public class TransferToClientViewModel : ObservableObject
    {

        #region Команды

        public RelayCommand CloseWindowCommand { get; set; }
        public RelayCommand TransferToClientCommand { get; set; }

        #endregion


        #region Поля и свойства

        private Client _client;
        private Client _toClient;
        private BankAccount _bankAccount;
        private BankAccount _toBankAccount;
        private ObservableCollection<BankAccount> _bankAccounts;
        private ObservableCollection<BankAccount> _toBankAccounts;
        private ObservableCollection<Client> _clients;
        private decimal _sum = 0;
        private decimal _maxSumValue;

        public Action<object> OnClose;


        public decimal MaxSumValue
        {
            get => _maxSumValue;
            set => Set(ref _maxSumValue, value);
        }

        public Client Client
        {
            get => _client;
            set => Set(ref _client, value);
        }

        public Client ToClient
        {
            get => _toClient;
            set
            {
                Set(ref _toClient, value);
                ToBankAccounts = _toClient.BankAccounts;
            } 
        }

        public BankAccount BankAccount
        {
            get => _bankAccount;
            set
            {
                Set(ref _bankAccount, value);
                MaxSumValue = _bankAccount.Balance;
            }
        }

        public BankAccount ToBankAccount
        {
            get => _toBankAccount;
            set => Set(ref _toBankAccount, value);
        }

        public ObservableCollection<BankAccount> BankAccounts
        {
            get => _bankAccounts;
            set => Set(ref _bankAccounts, value);
        }

        public ObservableCollection<BankAccount> ToBankAccounts
        {
            get => _bankAccounts;
            set => Set(ref _toBankAccounts, value);
        }
        
        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public decimal Sum
        {
            get => _sum;
            set => Set(ref _sum, Math.Round(value, 2));
        }

        #endregion


        #region Конмтрукторы

        public TransferToClientViewModel(Client client)
        {
            Client = client;
            Initialize();
        }

        #endregion


        #region Методы

        private void Initialize()
        {
            Clients = Bank.Instance.GetAllClients();
            BankAccounts = _client.BankAccounts;


            CloseWindowCommand = new RelayCommand(o =>
            {
                OnClose?.Invoke(null);
            });

            TransferToClientCommand = new RelayCommand(o =>
            {
                if (true)
                {
                    Client.TransferTo(_toClient, _toBankAccount, _bankAccount, _sum);
                    
                    Bank.Instance.UpdateBankAccountDb(_bankAccount);
                    Bank.Instance.UpdateBankAccountDb(_toBankAccount);

                    var str =
                        $"[{Bank.Instance.CurrentDate:dd.MM.yyyy}]Клиент {_client.FullName.Trim()} выполнил перевод средств" +
                        $" клиенту {_toClient.FullName.Trim()} на сумму {_sum}";
                    Bank.Instance.OnAddLog(str);

                    OnClose?.Invoke(null);
                }
            });
        }
        

        #endregion
    }
}
