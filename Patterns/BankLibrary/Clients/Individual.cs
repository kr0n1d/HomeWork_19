using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BankLibrary.Clients
{
    /// <summary>
    /// Класс, описывающий логику клиента - физического лица.
    /// </summary>
    public class Individual : Client
    {
        //  ===== Конструкторы ====

        /// <summary>
        /// Создаем клиента - физическое лицо.
        /// </summary>
        /// <param name="fullName">Полное имя (ФИО).</param>
        /// <param name="bankAccount">Расчётный счёт.</param>
        /// <param name="isVip">Является ли клиент привилегированным?</param>
        public Individual(string fullName, BankAccount bankAccount, bool isVip)
            : base(fullName, ClientTypes.Individual, bankAccount, isVip)
        {
        }

        /// <summary>
        /// Создаем клиента - физическое лицо.
        /// </summary>
        /// <param name="fullName">Полное имя (ФИО).</param>
        /// <param name="bankAccounts">Расчётные счёта.</param>
        /// <param name="isVip">Является ли клиент привилегированным?</param>
        public Individual(string fullName, ObservableCollection<BankAccount> bankAccounts, bool isVip)
            : base(fullName, ClientTypes.Individual, bankAccounts, isVip)
        {
        }




        //  ===== Методы ====

        protected override void IncreaseAmountWithCapitalization(BankAccount bankAccount)
        {
            var percent = IsVip ? 0.015m : 0.01m;
            bankAccount.Balance += bankAccount.Balance * percent;
        }

        protected override void IncreaseAmountWithoutCapitalization(BankAccount bankAccount)
        {
            var percent = IsVip ? 0.15m : 0.12m;
            bankAccount.Balance += bankAccount.Balance * percent;
        }
    }
}
