using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BankLibrary.Clients
{
    /// <summary>
    /// Класс, описывающий логику клиента - юридического лица.
    /// </summary>
    public class LegalEntity : Client
    {
        //  ===== Конструкторы ====

        /// <summary>
        /// Создаем клиента - юридическое лицо.
        /// </summary>
        /// <param name="fullName">Полное имя (ФИО).</param>
        /// <param name="bankAccount">Расчётный счёт.</param>
        /// <param name="isVip">Является ли клиент привилегированным?</param>
        public LegalEntity(string fullName, BankAccount bankAccount, bool isVip)
            : base(fullName, ClientTypes.LegalEntity, bankAccount, isVip)
        {
        }

        /// <summary>
        /// Создаем клиента - юридическое лицо.
        /// </summary>
        /// <param name="fullName">Полное имя (ФИО).</param>
        /// <param name="bankAccounts">Расчётные счёта.</param>
        /// <param name="isVip">Является ли клиент привилегированным?</param>
        public LegalEntity(string fullName, ObservableCollection<BankAccount> bankAccounts, bool isVip)
            : base(fullName, ClientTypes.LegalEntity, bankAccounts, isVip)
        {
        }



        //  ===== Методы ====

        protected override void IncreaseAmountWithCapitalization(BankAccount bankAccount)
        {
            var percent = IsVip ? 0.025m : 0.02m;
            bankAccount.Balance += bankAccount.Balance * percent;
        }

        protected override void IncreaseAmountWithoutCapitalization(BankAccount bankAccount)
        {
            var percent = IsVip ? 0.25m : 0.2m;
            bankAccount.Balance += bankAccount.Balance * percent;
        }
    }
}
