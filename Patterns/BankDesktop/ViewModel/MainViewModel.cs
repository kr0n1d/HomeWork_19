using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using BankDesktop.Core;
using BankDesktop.Model;
using BankDesktop.View;
using BankLibrary;
using BankLibrary.Clients;

namespace BankDesktop.ViewModel
{
    internal class MainViewModel : ObservableObject
    {

        #region Команды
        public RelayCommand CloseApplicationCommand { get; set; }
        public RelayCommand RemoveClientCommand { get; set; }
        public RelayCommand NextDayCommand { get; set; }
        public RelayCommand ShowAddClientWindowCommand { get; set; }
        public RelayCommand ShowAddBankAccountWindowCommand { get; set; }
        public RelayCommand ShowAddBankCreditWindowCommand { get; set; }
        public RelayCommand ShowEditClientWindowCommand { get; set; }
        public RelayCommand PutBankAccountCommand { get; set; }
        public RelayCommand TransferToBankAccountCommand { get; set; }
        public RelayCommand CloseBankAccountCommand { get; set; }
        public RelayCommand TransferToClientCommand { get; set; }

        #endregion

        #region Свойства

        private ObservableCollection<Individual> individuals;
        public ObservableCollection<Individual> Individuals
        {
            get => individuals;
            set => Set(ref individuals, value);
        }

        private ObservableCollection<LegalEntity> legalEntities;
        public ObservableCollection<LegalEntity> LegalEntities
        {
            get => legalEntities;
            set => Set(ref legalEntities, value);
        }


        public DateTime CurrentDate
        {
            get => Bank.Instance.currentDate;
            set => Set(ref Bank.Instance.currentDate, value, nameof(Bank.Instance.CurrentDate));
        }

        public BankAccount CurrentBankAccount { get; set; }

        
        public Client CurrentClient
        {
            get => Bank.Instance.CurrentClient;
            set
            {
                Set(ref Bank.Instance.CurrentClient, value);
                IsVipClient = Bank.Instance.CurrentClient.IsVip ? "Да" : "Нет";
            }
        }

        private string isVipClient;
        public string IsVipClient
        {
            get => isVipClient; 
            set => Set(ref isVipClient, value); 
        }

        
        private int currentIndexClientTypes;
        public int CurrentIndexClientTypes
        {
            get => currentIndexClientTypes;
            set
            {
                Set(ref currentIndexClientTypes, value);
                if (value == 0)
                {
                    IndividualsLvVisibility = Visibility.Visible;
                    LegalEntitiesLvVisibility = Visibility.Hidden;
                }
                else
                {
                    IndividualsLvVisibility = Visibility.Hidden;
                    LegalEntitiesLvVisibility = Visibility.Visible;
                }
            }

        }

        public ObservableCollection<Log> Logs
        {
            get => Bank.Instance.Logs;
            set => Set(ref Bank.Instance.Logs, value);
        }

        private Visibility individualsLvVisibility = Visibility.Visible;
        public Visibility IndividualsLvVisibility
        {
            get => individualsLvVisibility;
            set => Set(ref individualsLvVisibility, value);
        }

        private Visibility legalEntitiesLvVisibility = Visibility.Hidden;
        public Visibility LegalEntitiesLvVisibility
        {
            get => legalEntitiesLvVisibility;
            set => Set(ref legalEntitiesLvVisibility, value);
        }

        private bool buttonNextMonthIsEnable = true;
        public bool ButtonNextMonthIsEnable
        {
            get => buttonNextMonthIsEnable;
            set => Set(ref buttonNextMonthIsEnable, value);
        }

        #endregion


        #region Конструкторы

        public MainViewModel()
        {
            Initialize();
        }

        #endregion


        #region Методы  

        private void Initialize()
        {
            Bank.Instance.LoadClients();

            CloseApplicationCommand = new RelayCommand(o =>
            {
                Application.Current.Shutdown();
            });

            NextDayCommand = new RelayCommand(o =>
            {
                CurrentDate = CurrentDate.AddMonths(1);

                ButtonNextMonthIsEnable = false;
                Bank.Instance.CheckBalanceClients();
                Bank.Instance.SaveData();

                Task.Run(() =>
                {
                    while (!Bank.Instance.FinishedLoaded)
                    {

                    }

                    ButtonNextMonthIsEnable = true;
                });



                MessageBox.Show("Обработка может занять некоторое время!");
            });

            ShowAddClientWindowCommand = new RelayCommand(o =>
            {
                new AddClientView().ShowDialog();
                Bank.Instance.SaveData();
            });

            ShowEditClientWindowCommand = new RelayCommand(o =>
            {
                if (CurrentClient != null)
                {
                    new EditClientView(CurrentClient).ShowDialog();
                    Bank.Instance.SaveData();
                }
            });

            ShowAddBankAccountWindowCommand = new RelayCommand(o =>
            {
                if (CurrentClient != null)
                {
                    var view = new AddBankAccountView(CurrentClient);
                    view.ShowDialog();
                    Bank.Instance.SaveData();
                }
            });

            ShowAddBankCreditWindowCommand = new RelayCommand(o =>
            {
                if (CurrentClient != null && CurrentClient.BankCredits.Count == 0)
                {
                    var view = new AddBankCreditView(CurrentClient);
                    view.ShowDialog();
                    Bank.Instance.SaveData();
                }
                else
                {
                    MessageBox.Show("Нельзя иметь более 1 кредита.", "Ошибка.");
                }
            });

            RemoveClientCommand = new RelayCommand(o =>
            {
                if (CurrentClient != null)
                {
                    Bank.Instance.RemoveClient(CurrentClient);
                    individuals = Bank.Instance.Individuals;
                    legalEntities = Bank.Instance.LegalEntities;
                    Bank.Instance.SaveData();
                }
            });

            PutBankAccountCommand = new RelayCommand(o =>
            {
                if (CurrentBankAccount != null)
                {
                    var view = new PutBankAccountView(CurrentClient, CurrentBankAccount);
                    view.ShowDialog();
                    Bank.Instance.SaveData();
                }
            });

            TransferToBankAccountCommand = new RelayCommand(o =>
            {
                if (CurrentClient != null && CurrentBankAccount != null && CurrentClient.BankAccounts.Count > 1)
                {
                    var view = new TransferToBankAccountView(CurrentClient, CurrentBankAccount);
                    view.ShowDialog();
                    Bank.Instance.SaveData();
                }
            });

            TransferToClientCommand = new RelayCommand(o =>
            {
                var view = new TransferToClientView(CurrentClient);
                view.ShowDialog();
                Bank.Instance.SaveData();
            });

            CloseBankAccountCommand = new RelayCommand(o =>
            {
                if (CurrentClient.BankAccounts.Count > 1)
                {
                    Bank.Instance.RemoveBankAccount(CurrentBankAccount);
                    Bank.Instance.SaveData();
                }
                else
                {
                    MessageBox.Show("Этот банк невозможно покинуть.\nНельзя удалить единственный р/с.",
                        "Предупреждение.");
                }
            });

            Task.Run(() =>
            {
                while (!Bank.Instance.FinishedGenerate) { }

                Individuals = Bank.Instance.Individuals;
                LegalEntities = Bank.Instance.LegalEntities;
            });
        }

        #endregion

    }
}
