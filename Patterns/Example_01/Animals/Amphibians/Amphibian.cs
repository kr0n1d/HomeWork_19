using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example_01.Animals.Amphibians
{
    /// <summary>
    /// Класс, описывающий земноводных.
    /// </summary>
    public class Amphibian : Animal
    {
        /// <summary>
        /// Создаем земноводного животного.
        /// </summary>
        /// <param name="kind">Вид земногодного животного.</param>
        /// <param name="color">Цвет земногодного животного.</param>
        /// <param name="length">Длина земногодного животного.</param>
        /// <param name="weight">Вес земногодного животного.</param>
        public Amphibian(string kind, string color, int length, int weight)
            : base("Amphibian", kind, color, length, weight)
        {
        }
    }
}
