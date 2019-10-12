using System;
using System.Collections.Generic;
using System.Text;
using StockTrader.StockTrader_Model;
using System.Threading.Tasks;
using StockTrader.Service.DataService;

namespace StockTrader.Service.InfrastructureService
{
   public interface IStockService
    {         
        Task<string> AddStockShares(StockToPurchaseOrSell stock, User user);

        Task<string> RemoveStockShares(StockToPurchaseOrSell stock, User user, StockTraderContext ctext);
             
        IEnumerable<Stock> GetStocksByUser(User user);

        Task<double> GetStockPrice(string _stockSymbol);
    }
}
