using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autojenzi.src.Addin.Services
{
    public class WallProperties
    {
        public string Properties { get; set; }
        public string Unit { get; set; }
        public double Value { get; set; }

        public WallProperties(string properties, string unit, double value)
        {
            Properties = properties; Unit = unit; Value = value;
        }



    }
}
