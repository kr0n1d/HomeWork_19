using System.Collections.Generic;
using System.IO;
using Example_01.Animals;

namespace Example_01.DataSavers
{
    public class SaverCSV : ISave
    {
        public string FileName { get; set; }

        public SaverCSV(string fileName)
        {
            FileName = fileName;
        }

        public void SaveToFile(IEnumerable<IAnimal> animals)
        {
            var data = "Type;Kind;Color;Length(mm);Weight(gr);\n";

            foreach (var animal in animals)
            {
                data += animal.Type + ";";
                data += animal.Kind + ";";
                data += animal.Color + ";";
                data += animal.Length + ";";
                data += animal.Weight + ";\n";
            }

            File.WriteAllText(FileName, data);
        }
    }
}
