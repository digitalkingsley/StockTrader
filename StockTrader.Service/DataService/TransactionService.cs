using System;
using System.Collections.Generic;
using System.Text;
using StockTrader.Service.InfrastructureService;
using StockTrader.StockTrader_Model;
using System.Linq;
using StockTrader.Service.DataService;
using System.Threading.Tasks;

namespace StockTrader.Service.DataService
{
    public class TransactionService : ITransactionService
    {
        private StockTraderContext _context;
 
        private  string [] TransactionTypes = {"purchase","sale", "funding"};

        private string _transactionDescription;

        private StockHelperService _stockHelperService;

        public TransactionService(StockTraderContext context, StockHelperService stockTraderHelperService)
        {
            _context = context;
            _stockHelperService = stockTraderHelperService;
        }

        //This records a transaction relating to either of the addition of a brand new security to the user's portfolio
        //or the purchase of more shares of stock for an existing security.
        public async Task<Transaction> AddTransactionForStockPurchase(StockToPurchaseOrSell stock, User user, StockTraderContext _ctx, double stockPrice, string transactionType)
        {

            double stockTotalPrice = stock.Quantity * stockPrice;
                
            if (transactionType.ToLower() == TransactionTypes[0])
            {
                _transactionDescription = $"Purchase of {stock.Quantity} share(s) of {stock.SecurityName}'s Stock at a total cost of {stockTotalPrice}";
            }
            else
            {
                //This would throw an exception when another type of transaction
                //other than a purchase is attempted on this method. This method has two overloads
                //and the appropriate overload needs to be called for a particular transaction - "purchase", "sale", or "funding"
                throw new Exception("Invalid Transaction");
            }

            var transaction = new Transaction
            {
                TransactionType = transactionType,
                TransactionDescription = _transactionDescription,
                TransactionDay = DateTime.Today.Day,
                TransactionMonth = DateTime.Today.Month,
                TransactionYear = DateTime.Today.Year,
                TransactionDate = DateTime.Now,
                UserId=user.UserId
            };

            await _ctx.Transaction.AddAsync(transaction);
            user.Transaction.Add(transaction);
            return transaction;

        }

        //This records a transaction that involves the sale of shares of stock for a particular security
        public async Task<Transaction> AddTransactionForStockSale(StockToPurchaseOrSell stockToSell, Stock existingStock, User user, StockTraderContext _ctx, string transactionType)
        {
            double saleTotalPrice = stockToSell.UnitPrice * stockToSell.Quantity;

            if (transactionType.ToLower() == TransactionTypes[1])
            {
                _transactionDescription = $"Sale of {stockToSell.Quantity} share(s) of {stockToSell.SecurityName}'s Stock at a total price of {saleTotalPrice}";
            }
            else
            {
                throw new Exception("Invalid Transaction");
            }

            //Now register a Transaction against the new Stock Sale
            var transaction = new Transaction
            {
                TransactionType = transactionType,
                TransactionDescription = _transactionDescription,
                TransactionDay = DateTime.Today.Day,
                TransactionMonth = DateTime.Today.Month,
                TransactionYear = DateTime.Today.Year,
                TransactionDate = DateTime.Now,
                UserId=user.UserId
            };

            await _ctx.Transaction.AddAsync(transaction);
            user.Transaction.Add(transaction);
            return transaction;
        }

        //This records a transaction that involves a user funding his/her account
        public async Task<Transaction> AddTransactionForFunding(double amount, User user, StockTraderContext _ctx, string transactionType)
        {

            if (transactionType.ToLower() == TransactionTypes[2])
            {
                _transactionDescription = $"Account funded with {amount}";
            }
            else
            {
                throw new Exception("Invalid Transaction");
            }

            //Now register a Transaction against this account funding operation
            var transaction = new Transaction
            {
                TransactionType = transactionType,
                TransactionDescription = _transactionDescription,
                TransactionDay = DateTime.Today.Day,
                TransactionMonth = DateTime.Today.Month,
                TransactionYear = DateTime.Today.Year,
                TransactionDate = DateTime.Now,
                UserId=user.UserId
            };

            _ctx.Transaction.Add(transaction);
            user.Transaction.Add(transaction);
            await _ctx.SaveChangesAsync();
            return transaction;
        }

        //This gets all transactions associated with a user
        public IEnumerable<Transaction> GetAllTransactionsByUser(User user)
        {
            return _context.Transaction.Where(u => u.UserId == user.UserId).ToList<Transaction>();
            
        }

        //This retrieves all transactions associated with a user within a particular period.
        public IEnumerable<Transaction> GetAllTransactionsByUser(User user, DateTime startDate, DateTime endDate)
        {
            return _context.Transaction.Where(u => u.UserId == user.UserId && u.TransactionDate >= startDate && u.TransactionDate <= endDate).ToList<Transaction>();
         
        }


    }
}
