using System;
using System.Collections.ObjectModel;
using System.Linq;
using BankDesktop.Core;
using BankDesktop.Model;
using BankDesktop.Model.EF;
using BankLibrary.Clients;
using BankLibrary;

namespace BankDesktop.ViewModel
{
    internal class TransferToBankAccountViewModel : ObservableObject
    {
        #region MyRegion

        public RelayCommand CloseApplicationCommand { get; set; }
        public RelayCommand TransferToBankAccountCommand { get; set; }

        #endregion

        #region Поля и свойства
        
        private BankAccount _currentBankAccount;
        private BankAccount _toBankAccount;
        private Client _currentClient;
        private decimal _sum = 0;

        public Action<object> OnClose { get; set; }

        public BankAccount CurrentBankAccount
        {
            get => _currentBankAccount;
            set => Set(ref _currentBankAccount, value);
        }

        public BankAccount ToBankAccount
        {
            get => _toBankAccount;
            set => Set(ref _toBankAccount, value);
        }

        public decimal Sum
        {
            get => _sum;
            set => Set(ref _sum, Math.Round(value, 2));
        }

        public ObservableCollection<BankAccount> BankAccounts { get; set; }

        #endregion

        #region Конструкторы

        public TransferToBankAccountViewModel(Client client, BankAccount currentBankAccount)
        {
            CurrentBankAccount = currentBankAccount;
            Initialize(client, currentBankAccount);
        }

        #endregion

        #region Методы

        private void Initialize(Client client, BankAccount currentBankAccount)
        {
            BankAccounts = new ObservableCollection<BankAccount>();
            _currentClient = client;
            client.BankAccounts.ToList()
                .Where(ba => ba.Number != currentBankAccount.Number)
                .ToList()
                .ForEach(ba => BankAccounts.Add(ba));

            CloseApplicationCommand = new RelayCommand(o => OnClose?.Invoke(null));
            TransferToBankAccountCommand = new RelayCommand(o =>
            {
                if (_sum == 0)
                    return;
                
                client.Put(_toBankAccount, client.Withdraw(_currentBankAccount, _sum));

                Bank.Instance.UpdateBankAccountDb(_currentBankAccount);
                Bank.Instance.UpdateBankAccountDb(_toBankAccount);

                var str = $"[{Bank.Instance.CurrentDate:dd.MM.yyyy}]: Клиент {client.FullName.Trim()} " +
                          $"перевел с {_currentBankAccount} {_sum} на {_toBankAccount}";    
                Bank.Instance.OnAddLog(str);

                OnClose(null);
            });
        }
        

        #endregion
    }
}
