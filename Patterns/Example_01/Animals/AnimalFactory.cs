using Example_01.Animals.Amphibians;
using Example_01.Animals.Birds;
using Example_01.Animals.Empty;
using Example_01.Animals.Mammals;

namespace Example_01.Animals
{
    /// <summary>
    /// Класс, описывающий паттерн Фабрика.
    /// </summary>
    public static class AnimalFactory
    {
        /// <summary>
        /// Получить животное.
        /// </summary>
        /// <param name="type">Тип животного.</param>
        /// <param name="kind">Вид животного.</param>
        /// <param name="color">Цвет животного.</param>
        /// <param name="length">Длина животного.</param>
        /// <param name="weight">Вес животного.</param>
        /// <returns></returns>
        public static IAnimal GetAnimal(string type, string kind,
            string color, int length, int weight)
        {
            switch (type)
            {
                case "Mammal":
                    return new Mammal(kind, color, length, weight);
                case "Bird":
                    return new Bird(kind, color, length, weight);
                case "Amphibian":
                    return new Amphibian(kind, color, length, weight);
                default:
                    return new EmptyType();
            }
        }
    }
}
