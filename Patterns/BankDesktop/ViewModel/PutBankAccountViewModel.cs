using System;
using System.Windows.Markup.Localizer;
using BankDesktop.Core;
using BankDesktop.Model;
using BankLibrary;
using BankLibrary.Clients;

namespace BankDesktop.ViewModel
{
    public class PutBankAccountViewModel : ObservableObject
    {
        #region Команды

        public RelayCommand CloseApplicationCommand { get; set; }
        public RelayCommand PutBankAccountCommand { get; set; }

        #endregion

        #region Поля и Свойства

        private BankAccount _currentBankAccount;
        private Client _currentClient;

        public Action<object> OnClose;

        private decimal _sum = 10;
        public decimal Sum
        {
            get => this._sum;
            set => Set(ref _sum, Math.Round(value, 2));
        }

        #endregion

        #region Конструкторы

        public PutBankAccountViewModel(Client client, BankAccount bankAccount)
        {
            _currentBankAccount = bankAccount;
            _currentClient = client;
            CloseApplicationCommand = new RelayCommand(o => OnClose?.Invoke(o));
            PutBankAccountCommand = new RelayCommand(o =>
            {
                _currentClient.Put(_currentBankAccount, _sum);
                var str = $"[{Bank.Instance.CurrentDate:dd.MM.yyyy}]: Клиент {_currentClient.FullName.Trim()} пополнил {_currentBankAccount} на сумму {_sum}";
                Bank.Instance.OnAddLog(str);
                Bank.Instance.UpdateBankAccountDb(_currentBankAccount);
                OnClose?.Invoke(o);
            });
        }

        #endregion

    }
}
