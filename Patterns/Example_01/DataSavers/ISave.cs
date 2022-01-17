using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example_01.Animals;

namespace Example_01.DataSavers
{
    public interface ISave
    {
        void SaveToFile(IEnumerable<IAnimal> animals);
    }
}
