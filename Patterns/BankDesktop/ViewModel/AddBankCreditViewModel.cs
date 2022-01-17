using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using BankDesktop.Core;
using BankDesktop.Model;
using BankLibrary;
using BankLibrary.Clients;

namespace BankDesktop.ViewModel
{
    public class AddBankCreditViewModel : ObservableObject
    {
        #region Команды

        public RelayCommand CloseWindowCommand { get; set; }
        public RelayCommand AddBankCreditCommand { get; set; }

        #endregion


        #region Поля и свойства

        private Client _client;

        public Action<object> OnClose;

        private decimal _sumCredit = 500;
        public decimal SumCredit
        {
            get => _sumCredit;
            set => Set(ref _sumCredit, Math.Round(value, 2));
        }

        private bool _isEnableChangePercent;
        public bool IsEnableChangePercent
        {
            get => _isEnableChangePercent; 
            set => Set(ref _isEnableChangePercent, value); 

        }

        private ObservableCollection<BankAccount> _bankAccounts;
        public ObservableCollection<BankAccount> BankAccounts
        {
            get => _bankAccounts; 
            set => Set(ref _bankAccounts, value); 
        }

        private decimal _maxValueCredit = 1000;
        public decimal MaxValueCredit 
        { 
            get => _maxValueCredit; 
            set => Set(ref _maxValueCredit, value);
        }


        private BankAccount _bankAccount;

        public BankAccount BankAccount
        {
            get => _bankAccount;
            set
            {
                Set(ref _bankAccount, value);
                MaxValueCredit = _bankAccount.Balance;
            } 
        }
        public int SelectedIndexTerm { get; set; } = 0;
        public int SelectedIndexPercentCredit{ get; set; } = 0;
        public int SelectedIndexBankAccount{ get; set; } = 0;

        #endregion


        #region Конструкторы

        public AddBankCreditViewModel(Client client)
        {
            _client = client;
            Initialize();
        }

        #endregion

        #region Методы

        private void Initialize()
        {
            BankAccounts = _client.BankAccounts;

            CloseWindowCommand = new RelayCommand(o => OnClose?.Invoke(null));
            AddBankCreditCommand = new RelayCommand(o =>
            {
                AddBankCredit();
                OnClose?.Invoke(null);
            });

            IsEnableChangePercent = _client.IsVip;
            BankAccounts = _client.BankAccounts;
        }

        private void AddBankCredit()
        {
            if (BankAccount == null)
                return;

            var termCredit = GetTermCredit(SelectedIndexTerm);
            var percent = SelectedIndexPercentCredit == 0 ? 0.3m : 0.2m;
            var debt = _sumCredit + _sumCredit * percent;
            var bc = new BankCredit(debt, Bank.Instance.CurrentDate, termCredit, _bankAccount, SumCredit) {ClientId = _client.Id};
            _client.AddBankCredit(bc);
            Bank.Instance.AddBankCreditDb(bc);
            Bank.Instance.UpdateBankAccountDb(_bankAccount);
            var str =
                $"[{Bank.Instance.CurrentDate:dd.MM.yyyy}]: Клиент {_client.FullName.Trim()} взял кредит №{bc.Number} на сумму {SumCredit}";
            Bank.Instance.OnAddLog(str);
        }

        private int GetTermCredit(int selectedIndexTerm)
        {
            switch (selectedIndexTerm)
            {
                case 0:
                    return 1;
                case 1:
                    return 3;
                default:
                    return 5;
            }
        }

        #endregion
    }
}
