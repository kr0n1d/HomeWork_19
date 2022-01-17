using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example_01.Animals;

namespace Example_01.DataSavers
{
    public class DataSaver
    {
        private ISave _saver;


        public DataSaver(ISave saver)
        {
            _saver = saver;
        }

        public void Save(IEnumerable<IAnimal> animals)
        {
            _saver.SaveToFile(animals);
        }

    }
}
