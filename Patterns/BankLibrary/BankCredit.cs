using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BankLibrary
{
    /// <summary>
    /// Класс, реализующий логику кредита.
    /// </summary>
    public class BankCredit : INotifyPropertyChanged
    {
        //  ==== Поля ====

        private decimal _paidOut;


        //  ===== Свойства ====

        public int Id { get; set; }

        /// <summary>
        /// Номер счета.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Долг.
        /// </summary>
        public decimal Credit { get; set; }

        /// <summary>
        /// Выплачено.
        /// </summary>
        public decimal PaidOut
        {
            get => _paidOut;
            set { _paidOut = Math.Round(value, 2); OnPropertyChanged(); }
        }

        /// <summary>
        /// Дата открытия.
        /// </summary>
        public DateTime DateOpen { get; set; }

        /// <summary>
        /// Срок.
        /// </summary>
        public int Term { get; set; }

        /// <summary>
        /// Ежемесячная плата.
        /// </summary>
        public decimal MonthlyPayment { get; set; }

        /// <summary>
        /// Расчётный счёт, с которого будет сниматься сумма за кредит.
        /// </summary>
        public BankAccount BankAccount { get; set; }

        /// <summary>
        /// Id клиента.
        /// </summary>
        public int ClientId { get; set; }


        //  ===== Конструкторы ====

        public BankCredit()
        {

        }

        /// <summary>
        /// Выдаем кредит.
        /// </summary>
        /// <param name="credit">Сумма долга.</param>
        /// <param name="dateOpen">Дата открытия кредита.</param>
        /// <param name="term">Срок кредита.</param>
        /// <param name="bankAccount">Расчётный счёт, для снятия денег за кредит.</param>
        /// <param name="sum">Сумма пополнения расчётного счета.</param>
        public BankCredit(decimal credit, DateTime dateOpen, int term, 
            BankAccount bankAccount, decimal sum)
        {
            this.Number = Guid.NewGuid().ToString();
            this.DateOpen = dateOpen;
            this.Term = term;
            this.Credit = Math.Round(credit, 2);
            this.PaidOut = 0;
            this.MonthlyPayment = credit / (term * 12);
            this.BankAccount = bankAccount;
            this.BankAccount.Balance += sum;
        }

        /// <summary>
        /// Выдаем кредит.
        /// </summary>
        /// <param name="credit">Сумма долга.</param>
        /// <param name="dateOpen">Дата открытия кредита.</param>
        /// <param name="term">Срок кредита.</param>
        /// <param name="bankAccount">Расчётный счёт, для снятия денег за кредит.</param>
        public BankCredit(string number, decimal credit, DateTime dateOpen, 
        int term, decimal paidOut, BankAccount bankAccount)
        {
            this.Number = number;
            this.DateOpen = dateOpen;
            this.Term = term;
            this.Credit = Math.Round(credit, 2);
            this.PaidOut = paidOut;
            this.MonthlyPayment = credit / (term * 12);
            this.BankAccount = bankAccount;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}