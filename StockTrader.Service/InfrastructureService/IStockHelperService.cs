using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StockTrader.Service.DataService;
using StockTrader.StockTrader_Model;
using System.IdentityModel.Tokens.Jwt;

namespace StockTrader.Service.InfrastructureService
{
    /// <summary>
    /// This interface defines types that would come in handy in different parts of the StockTrader app
    /// </summary>
    #region Interface Members
    public interface IStockHelperService
    {
        /// <summary>
        /// This is used to retrieve my IEX Token from the appsettings.json file
        /// </summary>
        /// <returns>Method returns a string</returns>
        string GetIexApiTokenFromConfig();

        /// <summary>
        /// Used to get the current price of stock from the IEX API
        /// </summary>
        /// <param name="stockSymbol"></param>
        /// <returns>Method returns a double</returns>
        Task<double> GetStockPrice(string stockSymbol);

        /// <summary>
        /// Generates a JWT Token that is issued to a user after successful sign-in
        /// </summary>
        JwtSecurityToken GenerateAuthToken(User user);

        /// <summary>
        /// This is used to serialize the generated JWT Token, so that it can be sent back to an
        /// authenticated user as part of a response
        /// </summary>
        string SerializeToken(JwtSecurityToken token);

        ///<summary>
        ///This method gets the Stock for which a particular stock-purchase/sale transaction is to be recorded
        ///</summary>
        Stock GetStockForTransaction(StockToPurchaseOrSell stockToPurchase, User user);

        ///<summary>
        ///An overload of the method that gets the current stock for which a particular stock-purchase/sale is to be recorded
        ///This method simply helps to check if a user already has stock for a particular security
        /// </summary>
        bool StockExists(Stock existingStock, User user);

        ///<summary>
        ///This method gets the current user
        /// </summary>
        User GetCurrentUser(string email, StockTraderContext ctext);
    }
    #endregion
}
