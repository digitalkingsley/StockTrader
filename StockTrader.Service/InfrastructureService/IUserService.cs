using System;
using System.Collections.Generic;
using System.Text;
using StockTrader.StockTrader_Model;
using System.Threading.Tasks;

namespace StockTrader.Service.InfrastructureService
{
  public interface IUserService
    {
        Task<string> Register(User user, StockTraderContext context);

        string Authenticate(User user, StockTraderContext context);

        Task<string> FundAccount(double amount, string userEmail, StockTraderContext context);
    }
}
