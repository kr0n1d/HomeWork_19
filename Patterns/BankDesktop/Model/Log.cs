namespace BankDesktop.Model
{
    /// <summary>
    /// Класс, описывающий логигу "логов".
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Сообщение лога.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Создаем лог.
        /// </summary>
        /// <param name="message">Сообщение лога.</param>
        public Log(string message)
        {
            Message = message;
        }
    }
}
