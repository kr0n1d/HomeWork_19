using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example_01.Animals
{
    public interface IAnimal
    {
        string Type { get; }
        string Kind { get; }
        string Color { get; }
        int Length { get; set; }
        double Weight { get; set; }
    }
}
