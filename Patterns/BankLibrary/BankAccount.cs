using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BankLibrary
{
    /// <summary>
    /// Класс, реализующий логику расчётного счёта.
    /// </summary>
    public class BankAccount : INotifyPropertyChanged
    {
        //  ===== Поля ====

        private decimal _balance;
        


        //  ===== Свойства ====

        public int Id { get; set; }

        /// <summary>
        /// Номер счёта.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Сумма на счету.
        /// </summary>
        public decimal Balance
        {
            get => _balance;
            set { _balance = Math.Round(value, 2); OnPropertyChanged(); }
        }

        /// <summary>
        /// Дата отрытия счёта.
        /// </summary>
        public DateTime DateOpen { get; set; }

        /// <summary>
        /// С капитализацией?
        /// </summary>
        public bool Capitalization { get; set; }

        /// <summary>
        /// Id клиента.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Количество раз увеличивалась сумма. Для увеличения дохода на n-ый процент.
        /// </summary>
        public int NumberTimesIncreased { get; set; }



        //  ===== Конструкторы ====

        /// <summary>
        /// Создаем рассчетный счет. (Для БД)
        /// </summary>
        public BankAccount()
        {

        }

        /// <summary>
        /// Создаем рассчетный счёт.
        /// </summary>
        /// <param name="open">Дата открытия.</param>
        /// <param name="balance">Баланс.</param>
        /// <param name="capitalization">С капитализацией?</param>
        public BankAccount(DateTime open, decimal balance, bool capitalization)
        {
            this.Number = Guid.NewGuid().ToString();
            this.Balance = balance;
            this.Capitalization = capitalization;
            this.DateOpen = open;
            this.NumberTimesIncreased = 0;
        }

        /// <summary>
        /// Создаем рассчетный счёт.
        /// </summary>
        /// <param name="number">Номер р/с.</param>
        /// <param name="open">Дата открытия.</param>
        /// <param name="balance">Баланс.</param>
        /// <param name="capitalization">С капитализацией?</param>
        /// <param name="numberTimesIncreased">Кол-во раз увеличивалась сумма. Для увеличения дохода на n-ый процент.</param>
        public BankAccount(string number, DateTime open, decimal balance, bool capitalization, int numberTimesIncreased)
        {
            this.Number = number;
            this.Balance = balance;
            this.Capitalization = capitalization;
            this.DateOpen = open;
            this.NumberTimesIncreased = numberTimesIncreased;
        }



        //  ===== Методы ====

        public override string ToString()
        {
            var arrData = this.Number.Split('-');
            var str = $"р/с ****-{arrData[arrData.Length - 1]}. Баланс: {Balance}";
            return str;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}