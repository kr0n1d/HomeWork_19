using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Example_01.Animals;

namespace Example_01.DataSavers
{
    public class SaverXML : ISave
    {
        public string FileName { get; set; }

        public SaverXML(string fileName)
        {
            FileName = fileName;
        }

        public void SaveToFile(IEnumerable<IAnimal> animals)
        {
            XElement arrAnimals = new XElement("Animals");

            foreach (var animal in animals)
            {
                XElement xAnimal = new XElement("Animal");
                XAttribute type = new XAttribute("Type", animal.Type);
                XAttribute kind = new XAttribute("Kind", animal.Kind);
                XAttribute color = new XAttribute("Color", animal.Color);
                XAttribute length = new XAttribute("Length", animal.Length);
                XAttribute weight = new XAttribute("Weight", animal.Weight);

                xAnimal.Add(type, kind, color, length, weight);

                arrAnimals.Add(xAnimal);
            }

            using (var fs = File.OpenWrite(FileName))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    arrAnimals.Save(sw);
                }
            }

        }
    }
}
