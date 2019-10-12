using System;
using System.Collections.Generic;
using System.Text;

namespace StockTrader.Service.InfrastructureService
{
    public interface IStockHelper
    {
        int GetStockPrice(string symbol);
    }
}
