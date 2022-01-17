using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example_01.Animals.Birds
{
    /// <summary>
    /// Класс, описывающий птиц.
    /// </summary>
    public class Bird : Animal
    {
        /// <summary>
        /// Создаем птицу.
        /// </summary>
        /// <param name="kind">Вид птицы.</param>
        /// <param name="color">Цвет птицы.</param>
        /// <param name="length">Длина птицы.</param>
        /// <param name="weight">Вес птицы.</param>
        public Bird(string kind, string color, int length, int weight)
            : base("Bird", kind, color, length, weight)
        {
        }

    }
}
