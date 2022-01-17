using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

namespace BankLibrary.Clients
{
    /// <summary>
    /// Абстрактный класс, описывающий логику клиента банка.
    /// </summary>
    public abstract class Client : IVip
    {
        //  ===== Свойства ====

        /// <summary>
        /// Id клиента.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Полное имя.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Тип клиента.
        /// </summary>
        public ClientTypes ClientType { get; set; }

        /// <summary>
        /// Является ли клиент привилегированным?
        /// </summary>
        public bool IsVip  { get; set; }

        /// <summary>
        /// Расчётные счета.
        /// </summary>
        public ObservableCollection<BankAccount> BankAccounts { get; set; }

        /// <summary>
        /// Кредиты.
        /// </summary>
        public ObservableCollection<BankCredit> BankCredits { get; set; }

        /// <summary>
        /// Количество р/с
        /// </summary>
        public int CountBankAccounts { get; set; }

        /// <summary>
        /// Количество кредитов
        /// </summary>
        public int CountBankCredits { get; set; }




        //  ===== Конструкторы ====

        public Client() { }

        /// <summary>
        /// Создаем клиента.
        /// </summary>
        /// <param name="fullName">Полное имя.</param>
        /// <param name="clientType">Тип клиента.</param>
        /// <param name="bankAccount">Расчётный счёт.</param>
        /// <param name="isVip">Является ли клиент привилегированным?</param>
        protected Client(string fullName, ClientTypes clientType, BankAccount bankAccount, bool isVip)
        {
            this.FullName = fullName;
            this.ClientType = clientType;
            this.BankAccounts = new ObservableCollection<BankAccount>() { bankAccount };
            this.BankCredits = new ObservableCollection<BankCredit>();
            // this.BankAccounts.Add(bankAccount);
            this.IsVip = isVip;
            this.CountBankAccounts = this.BankAccounts.Count;
            this.CountBankCredits = this.BankCredits.Count;
        }

        /// <summary>
        /// Создаем клиента.
        /// </summary>
        /// <param name="fullName">Полное имя.</param>
        /// <param name="clientType">Тип клиента.</param>
        /// <param name="bankAccounts">Расчётные счёта.</param>
        /// <param name="isVip">Является ли клиент привилегированным?</param>
        protected Client(string fullName, ClientTypes clientType,
            ObservableCollection<BankAccount> bankAccounts, bool isVip)
        {
            this.FullName = fullName;
            this.ClientType = clientType;
            this.BankAccounts = bankAccounts;
            this.BankCredits = new ObservableCollection<BankCredit>();
            this.IsVip = isVip;
            this.CountBankAccounts = this.BankAccounts.Count;
            this.CountBankCredits = this.BankCredits.Count;
        }





        //  ===== Методы ====

        /// <summary>
        /// Добавить рассчётный счёт.
        /// </summary>
        /// <param name="bankAccount">Расчётный счёт.</param>
        public void AddBankAccount(BankAccount bankAccount)
        {
            BankAccounts.Add(bankAccount);
            CountBankAccounts = BankAccounts.Count;
        }

        /// <summary>
        /// Удалить расчётный счёт.
        /// </summary>
        /// <param name="bankAccount"></param>
        public void RemoveBankAccount(BankAccount bankAccount)
        {
            if (BankAccounts.Count > 1)
            {
                var balance = bankAccount.Balance;
                BankAccounts.Remove(bankAccount);
                BankAccounts[0].Balance += balance;
                CountBankAccounts = BankAccounts.Count;
            }
        }

        /// <summary>
        /// Добавить кредит.
        /// </summary>
        /// <param name="bankCredit"></param>
        public void AddBankCredit(BankCredit bankCredit)
        {
            if (CountBankCredits == 1)
                throw new CountCreditsOutOfRangeException();

            BankCredits.Add(bankCredit); 
            
            CountBankCredits = BankCredits.Count;
        }

        /// <summary>
        /// Пополнить баланс.
        /// </summary>
        /// <param name="bankAccount">Рассчётный счёт</param>
        /// <param name="sum">Сумма пополнения.</param>
        public void Put(BankAccount bankAccount, decimal sum)
        {
            bankAccount.Balance += sum;
        }

        /// <summary>
        /// Снять со счёта.
        /// </summary>
        /// <param name="bankAccount">Рассчётный счёт</param>
        /// <param name="sum">Сумма, которую нужно снять со счёта, не превышающую баланс.</param>
        /// <returns>Остаток на счёте.</returns>
        public decimal Withdraw(BankAccount bankAccount, decimal sum)
        {
            if (bankAccount.Balance - sum >= 0)
                return bankAccount.Balance -= sum;
            
            return bankAccount.Balance;
        }

        /// <summary>
        /// Перевести клиенту.
        /// </summary>
        /// <param name="client">Клиент.</param>
        /// <param name="clientBankAccount">Рассчётный счет клиента.</param>
        /// <param name="bankAccount">Рассчётный счёт</param>
        /// <param name="sum">Сумма.</param>
        public void TransferTo(Client client, BankAccount clientBankAccount, BankAccount bankAccount, decimal sum)
        {
            if (bankAccount.Balance - sum >= 0)
            {
                bankAccount.Balance -= sum;
                client.Put(clientBankAccount, sum);
                return;
            }
        }
        
        /// <summary>
        /// Проверка баланса расчётных счетов и кредитов.
        /// </summary>
        /// <param name="currentDate">Текущая дата.</param>
        public void CheckBankAccountsAndCredits(DateTime currentDate)
        {
            foreach (var bankAccount in this.BankAccounts)
            {
                if (bankAccount.Capitalization)
                {
                    this.IncreaseAmountWithCapitalization(bankAccount);
                }
                else
                {
                    if (currentDate.Month == bankAccount.DateOpen.Month)
                    {
                        this.IncreaseAmountWithoutCapitalization(bankAccount);
                    }
                }

            }

            for (int i = 0; i < this.BankCredits.Count; i++)
            {
                BankCredits[i].BankAccount.Balance -= BankCredits[i].MonthlyPayment;
                BankCredits[i].PaidOut += BankCredits[i].MonthlyPayment;
                var min = (int)BankCredits[i].Credit - 5;
                var max = (int)BankCredits[i].Credit + 5;

                if ((int)BankCredits[i].PaidOut >= min  
                    && (int)BankCredits[i].PaidOut <= max)
                {
                    BankCredits.Remove(BankCredits[i]);
                    i--;
                    this.CountBankCredits = this.BankCredits.Count;
                }
            }
        }

        /// <summary>
        /// Увеличение сумма расчётного счета с капитализацией.
        /// </summary>
        /// <param name="bankAccount">Текущий расчётный счёт.</param>
        protected abstract void IncreaseAmountWithCapitalization(BankAccount bankAccount);

        /// <summary>
        /// Увеличение сумма расчётного счета без капитализации.
        /// </summary>
        /// <param name="bankAccount">Текущий расчётный счёт.</param>
        protected abstract void IncreaseAmountWithoutCapitalization(BankAccount bankAccount);

        public override string ToString()
        {
            return this.FullName;
        }
    }
}