using System;

namespace BankLibrary
{
    public class CountCreditsOutOfRangeException : Exception
    {
        public new readonly string Message;

        public CountCreditsOutOfRangeException()
        {
            this.Message = "Ошибка! Количество банковских кредитов не может быть более 1.";
        }
    }
}
