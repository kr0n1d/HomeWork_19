using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example_01.Animals.Mammals
{
    /// <summary>
    /// Класс, описывающий млекомитающих.
    /// </summary>
    public class Mammal : Animal
    {
        /// <summary>
        /// Создаем млекопитающего животного.
        /// </summary>
        /// <param name="kind">Вид животного.</param>
        /// <param name="color">Цвет животного.</param>
        /// <param name="length">Длина животного.</param>
        /// <param name="weight">Вес животного.</param>
        public Mammal(string kind, string color, int length, int weight)
            : base("Mammal", kind, color, length, weight)
        {
        }
    }
}
