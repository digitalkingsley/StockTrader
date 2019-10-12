using System;
using System.Collections.Generic;
using System.Text;
using StockTrader.StockTrader_Model;
using System.Threading.Tasks;

namespace StockTrader.Service.InfrastructureService
{
    public interface ITransactionService
    {
        //This records a transaction relating to the purchase of more shares of stock for an existing security in an individual's portfolio or a brand new Security
        Task<Transaction> AddTransactionForStockPurchase(StockToPurchaseOrSell stockToSell, User user, StockTraderContext ctx, double stockPrice, string transactionType);

        //This records a transaction relating to the sale of shares of stock of a particular security
        Task<Transaction> AddTransactionForStockSale(StockToPurchaseOrSell stockToPurchase, Stock existingStock, User user, StockTraderContext ctx, string transactionType);

        //This records a transaction relating to a user funding his/her account
        Task<Transaction> AddTransactionForFunding(double amount, User user, StockTraderContext ctx, string transactionType); 

        IEnumerable<Transaction> GetAllTransactionsByUser(User user);

        IEnumerable<Transaction> GetAllTransactionsByUser(User user, DateTime startDate, DateTime endDate);


    }
}
