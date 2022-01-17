using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example_01.Animals
{
    /// <summary>
    /// Класс описывающий животных.
    /// </summary>
    public abstract class Animal : IAnimal
    {
        /// <summary>
        /// Создаем животного.
        /// </summary>
        /// <param name="type">Тип животного.</param>
        /// <param name="kind">Вид животного.</param>
        /// <param name="color">Цвет животного.</param>
        /// <param name="length">Длина животного.</param>
        /// <param name="weight">Вес животного.</param>
        public Animal(string type, string kind, string color, int length, int weight)
        {
            Type = type;
            Kind = kind;
            Color = color;
            Length = length;
            Weight = weight;
        }

        /// <summary>
        /// Тип животного.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Вид животного.
        /// </summary>
        public string Kind { get; }

        /// <summary>
        /// Цвет животного.
        /// </summary>
        public string Color { get; }

        /// <summary>
        /// Длина животного.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Вес животного.
        /// </summary>
        public double Weight { get; set; }
    }
}
