using System;
using System.Collections.Generic;
using System.Text;
using StockTrader.StockTrader_Model;
using System.Threading.Tasks;

namespace StockTrader.Service.InfrastructureService
{
  public interface IUserService
    {
        Task Register(User user);

        string Authenticate(User user);

        Task<string> FundAccount(double amount, string userEmail);
    }
}
