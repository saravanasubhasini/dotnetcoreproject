using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IStockService
    {
        Task<Dictionary<string, object>?> StockServiceResquestAndResponse(string StockSymbol);
    }
}
