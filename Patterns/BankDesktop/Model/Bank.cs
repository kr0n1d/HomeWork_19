using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BankDesktop.Services;
using BankLibrary;
using BankLibrary.Clients;

namespace BankDesktop.Model
{
    public class Bank
    {
        private static readonly Bank instance = new();
        private DataProvider dataProvider;


        public DateTime currentDate;

        public ObservableCollection<Log> Logs;
        public event Action<Log> AddLog;


        public static Bank Instance => instance;


        public Client CurrentClient;
        public ObservableCollection<Individual> Individuals { get; set; }
        public ObservableCollection<LegalEntity> LegalEntities { get; set; }

        public DateTime CurrentDate { get => currentDate; set => currentDate = value; } 

        public bool FinishedGenerate => dataProvider.FinishedGenerate;
        public bool FinishedLoaded => dataProvider.FinishedLoaded;




        private Bank()
        {
            dataProvider = new DataProvider();

            Individuals = new ObservableCollection<Individual>();
            LegalEntities = new ObservableCollection<LegalEntity>();
            Logs = new ObservableCollection<Log>();
            AddLog = (log) => Logs.Insert(0, log);
            CurrentDate = DateTime.Now;
        }




        public void LoadClients()
        {
            Task.Run(() => dataProvider.LoadClients());
        }

        public void GenerateClients()
        {
            Task.Run(() => dataProvider.GenerateClients());
        }

        public void OnAddLog(string message)
        {
            AddLog?.Invoke(new Log(message));
        }

        /// <summary>
        /// Добавить клиета в список.
        /// </summary>
        /// <param name="client"></param>
        public void AddClient(Client client)
        {
            if (client is Individual individual)
                Individuals.Add(individual);
            else
                LegalEntities.Add(client as LegalEntity);

            var type = client.ClientType == ClientTypes.Individual
                ? "физическое лицо"
                : "юридическое лицо";

            AddClientDb(client);
            OnAddLog($"[{currentDate:dd.MM.yyyy}]: Добавлено {type}: {client.FullName}");
        }

        public void RemoveClient(Client client)
        {
            Client tmpClient;
            if (client is Individual)
            {
                tmpClient = Individuals.First(c => c.Id == client.Id);
                Individuals.Remove(tmpClient as Individual);
            }
            else
            {
                tmpClient = LegalEntities.First(c => c.Id == client.Id);
                LegalEntities.Remove(tmpClient as LegalEntity);
            }
            RemoveClientDb(client);

            var type = client.ClientType == ClientTypes.Individual
                ? "физическое лицо"
                : "юридическое лицо";

            OnAddLog($"[{currentDate:dd.MM.yyyy}]: Удалено {type}: {client.FullName}");
        }

        public void ChangeTypeClient(Client client)
        {
            if (client.ClientType == ClientTypes.Individual)
            {
                var tmp = (Client)LegalEntities
                    .First(c => c.FullName == client.FullName);
                LegalEntities.Remove(tmp as LegalEntity);
                Individuals.Add(tmp as Individual);
            }
            else
            {
                var tmp = (Client)Individuals
                    .First(c => c.FullName == client.FullName);
                Individuals.Remove(tmp as Individual);
                LegalEntities.Add(tmp as LegalEntity);
            }
        }

        public void CheckBalanceClients()
        {
            foreach (var client in Individuals)
            {
                client.CheckBankAccountsAndCredits(currentDate);

                foreach (var bankAccount in client.BankAccounts)
                {
                    UpdateBankAccountDb(bankAccount);
                }

                foreach (var credit in client.BankCredits)
                {
                    UpdateBankCreditDb(credit);
                }
            }
            

            foreach (var client in LegalEntities)
            {
                client.CheckBankAccountsAndCredits(currentDate);

                foreach (var bankAccount in client.BankAccounts)
                {
                    UpdateBankAccountDb(bankAccount);
                }

                foreach (var credit in client.BankCredits)
                {
                    UpdateBankCreditDb(credit);
                }
            }

            OnAddLog($"[{currentDate:dd.MM:yyyy}]: Текущая дата была изменена.");
        }

        public void AddBankAccount(BankAccount bankAccount)
        {
            CurrentClient.AddBankAccount(bankAccount);
            AddBankAccountDb(bankAccount);
            OnAddLog($"[{CurrentDate:dd.MM.yyyy}]: Клиент {CurrentClient.FullName.Trim()} открыл {bankAccount}");
        }

        public void RemoveBankAccount(BankAccount bankAccount)
        {
            CurrentClient.RemoveBankAccount(bankAccount);
            RemoveBankAccountDb(bankAccount);
            OnAddLog($"[{CurrentDate:dd.MM.yyyy}]: Клиент {CurrentClient.FullName.Trim()} закрыл {bankAccount}");
        }

        public ObservableCollection<Client> GetAllClients()
        {
            ObservableCollection<Client> clients = new ObservableCollection<Client>();

            foreach (var client in Individuals)
                clients.Add(client);

            foreach (var client in LegalEntities)
                clients.Add(client);

            return clients;
        }

        public void SaveData()
        {
            Task.Run(() => dataProvider.SaveData());
        }

        public void UpdateClientDb(Client client)
        {
            dataProvider.UpdateClient(client);
        }

        public void RemoveClientDb(Client client)
        {
            dataProvider.RemoveClient(client);
        }

        public void AddClientDb(Client client)
        {
            dataProvider.AddClient(client);
        }

        public void UpdateBankAccountDb(BankAccount bankAccount)
        {
            dataProvider.UpdateBankAccount(bankAccount);
        }

        public void RemoveBankAccountDb(BankAccount bankAccount)
        {
            dataProvider.RemoveBankAccount(bankAccount);
        }

        public void AddBankAccountDb(BankAccount bankAccount)
        {
            dataProvider.AddBankAccount(bankAccount);
        }

        public void UpdateBankCreditDb(BankCredit bankCredit)
        {
            dataProvider.UpdateBankCredit(bankCredit);
        }

        public void RemoveBankCreditDb(BankCredit bankCredit)
        {
            dataProvider.RemoveBankCredit(bankCredit);
        }

        public void AddBankCreditDb(BankCredit bankCredit)
        {
            dataProvider.AddBankCredit(bankCredit);
        }
    }
}
