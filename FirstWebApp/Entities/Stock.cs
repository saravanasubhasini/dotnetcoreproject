using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Stock
    {
        public string? SymbolValue { get; set; }
        public double CurrentPrice { get; set; }
        public double LowestPrice { get; set; }
        public double HighestPrice { get; set; }
        public double OpenPrice { get; set; }

    }
}
