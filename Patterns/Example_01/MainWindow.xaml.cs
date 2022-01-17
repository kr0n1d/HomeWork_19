using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Example_01.Animals;
using Example_01.Animals.Amphibians;
using Example_01.Animals.Birds;
using Example_01.Animals.Mammals;
using Example_01.DataSavers;

namespace Example_01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<IAnimal> animals;
        private DataSaver saver;
        public MainWindow()
        {
            InitializeComponent();

            animals = new ObservableCollection<IAnimal>();

            lvAnimals.ItemsSource = animals;

            animals.Add(new Mammal("Tiger", "Orange Black", 2500, 120_000));
            animals.Add(new Bird("Eagle", "Brown", 1200, 50_000));
            animals.Add(new Amphibian("Salamander", "Black", 120, 350));
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddAnimal(object sender, RoutedEventArgs e)
        {
            if (CheckFields())
            {
                var type = tbType.Text;
                var kind = tbKind.Text;
                var color = tbColor.Text;
                var length = int.Parse(tbLength.Text);
                var weight = int.Parse(tbWeight.Text);

                var animal = AnimalFactory.GetAnimal(type, kind, color, length, weight);

                animals.Add(animal);

                this.DataContext = animal;

                tbType.Clear();
                tbKind.Clear();
                tbColor.Clear();
                tbLength.Clear();
                tbWeight.Clear();
            }
        }

        private bool CheckFields()
        {
            Regex reg = new Regex("[a-zA-Z\\s]");
            if (!reg.IsMatch(tbType.Text) || string.IsNullOrEmpty(tbType.Text))
            {
                MessageBox.Show("В типе должны быть указаны только латинские буквы и пробелы.");
                return false;
            }
            else if (!reg.IsMatch(tbKind.Text) || string.IsNullOrEmpty(tbKind.Text))
            {
                MessageBox.Show("В виде должны быть указаны только латинские буквы и пробелы.");
                return false;
            }
            else if (!reg.IsMatch(tbColor.Text) || string.IsNullOrEmpty(tbColor.Text))
            {
                MessageBox.Show("В цвете должны быть указаны только латинские буквы и пробелы.");
                return false;
            }

            reg = new Regex(@"\d");

            if (!reg.IsMatch(tbLength.Text) || string.IsNullOrEmpty(tbLength.Text))
            {
                MessageBox.Show("В поле Length должны быть указаны только целые числа. (грамм)");
                return false;
            }

            if (int.Parse(tbLength.Text) <= 0)
            {
                MessageBox.Show("Length не может быть меньше нуля.");
                return false;
            }

            if (!reg.IsMatch(tbWeight.Text) || string.IsNullOrEmpty(tbWeight.Text))
            {
                MessageBox.Show("В поле Weight должны быть указаны только целые числа. (грамм)");
                return false;
            }

            if (int.Parse(tbWeight.Text) <= 0)
            {
                MessageBox.Show("Weight не может быть меньше нуля.");
                return false;
            }

            return true;
        }

        private void Animals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DataContext = lvAnimals.SelectedItem as Animal;
        }

        private void RemoveAnimal(object sender, RoutedEventArgs e)
        {
            if (lvAnimals.SelectedItem is Animal animal)
            {
                animals.Remove(animal);
            }
        }

        private void SaveToJSON(object sender, RoutedEventArgs e)
        {
            saver = new DataSaver(new SaverJson("Animals.json"));
            saver.Save(animals);
        }

        private void SaveToXML(object sender, RoutedEventArgs e)
        {
            saver = new DataSaver(new SaverXML("Animals.xml"));
            saver.Save(animals);
        }

        private void SaveToCSV(object sender, RoutedEventArgs e)
        {
            saver = new DataSaver(new SaverCSV("Animals.csv"));
            saver.Save(animals);
        }
    }
}
