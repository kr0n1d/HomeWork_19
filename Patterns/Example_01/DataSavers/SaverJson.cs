using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example_01.Animals;
using Newtonsoft.Json.Linq;

namespace Example_01.DataSavers
{
    public class SaverJson : ISave
    {
        public string FileName { get; set; }

        public SaverJson(string fileName)
        {
            FileName = fileName;
        }

        public void SaveToFile(IEnumerable<IAnimal> animals)
        {
            JArray arrAnimals = new JArray();

            foreach (var animal in animals)
            {
                JObject obj = new JObject();
                obj["type"] = animal.Type;
                obj["kind"] = animal.Kind;
                obj["color"] = animal.Color;
                obj["length"] = animal.Length;
                obj["weight"] = animal.Weight;

                arrAnimals.Add(obj);
            }

            File.WriteAllText(FileName, arrAnimals.ToString());
        }
    }
}
