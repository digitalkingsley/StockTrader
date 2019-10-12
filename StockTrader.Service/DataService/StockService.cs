using System;
using System.Collections.Generic;
using System.Text;
using StockTrader.Service.InfrastructureService;
using StockTrader.StockTrader_Model;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StockTrader.Service.DataService
{
    public class StockService:IStockService
    {
        #region Member Fields

        internal readonly StockTraderContext _context;
        private ITransactionService _transactionSevice;
        private IStockHelperService _stockHelperService;

        #endregion

        #region Class Constructor

        public StockService(StockTraderContext context, IStockHelperService stockHelperService, ITransactionService transactionService)
        {
            _context = context;
            _transactionSevice = transactionService;
            _stockHelperService = stockHelperService;
        }

        #endregion

        #region Method To Purchase Shares Of Stock
        public async Task<string> AddStockShares(StockToPurchaseOrSell stock, User user)
        {
            double stockPrice;
            var authenticatedUser = _stockHelperService.GetCurrentUser(user.UserEmail, _context);
            try
            {
                stockPrice = await _stockHelperService.GetStockPrice(stock.Symbol);
            }
            catch (Exception ex)
            {
                return "Could Not Retrieve Stock Price: " + ex.Message;
            }
            if (authenticatedUser.UserAccountBalance < stockPrice)
            {
                return "Insufficient Balance";
            }          
            var theStock = _context.Stock.Include(s => s.User).Where(s => s.User.UserEmail == user.UserEmail && s.StockSecuritySymbol == stock.Symbol).FirstOrDefault();
            if (theStock == null)
            {
                var newStock = new Stock
                {
                    StockSecurityName = stock.SecurityName,
                    StockSecuritySymbol = stock.Symbol,
                    StockShareNumber = stock.Quantity,
                    StockPurchaseDay = DateTime.Now.Day,
                    StockPurchaseMonth = DateTime.Now.Month,
                    StockPurchaseYear = DateTime.Now.Year,
                    StockPurchaseDate = DateTime.Now,
                    UserId = user.UserId
                };

                double amountSpent = stock.Quantity * stockPrice;
                _context.Stock.Add(newStock);
                authenticatedUser.Stock.Add(newStock);
                authenticatedUser.UserAccountBalance -= amountSpent;
                //Register this purchase Transaction
                Transaction transaction = await _transactionSevice.AddTransactionForStockPurchase(stock, user, _context, stockPrice, "purchase");
                if (transaction != null)
                {
                    await _context.SaveChangesAsync();
                    return "Success";
                }
                else
                {
                    return "Purchase was successful, but Transaction could not be saved. Operation rolled back...";
                }
            }
            else
            {
                double amountSpent = stock.Quantity * stockPrice;
                authenticatedUser.UserAccountBalance -= amountSpent;
                theStock.StockShareNumber += stock.Quantity;
                Transaction transaction = await _transactionSevice.AddTransactionForStockPurchase(stock, user, _context, stockPrice, "purchase");
                if (transaction != null)
                {
                    await _context.SaveChangesAsync();
                    return "Success";
                }
                else
                {
                    return "Purchase was successful, but Transaction could not be saved. Operation rolled back...";
                }                
            }
        }
        #endregion

        #region Method To List All Stocks Purchased By User
        public IEnumerable<Stock> GetStocksByUser(User user)
        {
            var theStocks = _context.Stock.Where(u => u.UserId == user.UserId).ToList<Stock>();
           
            if(theStocks != null)
            {
                return theStocks;
            }
            else
            {
                throw new Exception("Stocks could not be retrieved for user");
            }
           
        }
        #endregion

        #region Method To Sell User Stocks
        public async Task<string> RemoveStockShares(StockToPurchaseOrSell stock, User user, StockTraderContext ctext)
        {
            var theStock = ctext.Stock.Include(u => u.User).Where(u => u.User.UserEmail == user.UserEmail && u.StockSecuritySymbol == stock.Symbol).FirstOrDefault();
            if(string.IsNullOrEmpty(stock.UnitPrice.ToString()))
            {
                throw new Exception("No UnitPrice specified for the Stock to sell");
            }

            if (theStock != null)
            {
                if (theStock.StockShareNumber >= stock.Quantity)
                {
                    //double saleProfit = stock.Quantity * stock.UnitPrice;
                    theStock.StockShareNumber -= stock.Quantity;
                    user.UserAccountBalance += (stock.Quantity * stock.UnitPrice);
                    
                    var transaction = await _transactionSevice.AddTransactionForStockSale(stock, theStock, user, ctext, "sale");
                    if (transaction != null)
                    {
                        await ctext.SaveChangesAsync();
                        return "Success";
                    }
                    else
                    {
                        return "Sale of Stock was successful, but Transaction could not be saved. Operation rolled back...";
                    }
                }
                else
                {
                    throw new Exception("Insufficient Stock Shares");
                }
            }
            else
            {
                throw new Exception("User Has No Stocks For This Security");
            }         
        }
        #endregion

        #region Method To Get Latest Price Of Stock From IEX
        public async Task<double> GetStockPrice(string stockSymbol)
        {
            var stockPrice = await _stockHelperService.GetStockPrice(stockSymbol);
            return stockPrice;
        }
        #endregion
    }
}
