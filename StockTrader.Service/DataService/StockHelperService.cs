using System;
using System.Text;
using StockTrader.Service.InfrastructureService;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockTrader.StockTrader_Model;


namespace StockTrader.Service.DataService
{
    public class StockHelperService: ControllerBase, IStockHelperService
    {

        /// <summary>
        /// This is necessary for dependency injection. 
        /// Required for interracting with the appsettings.json file, in order to retrieve the IEX API Token
        /// And any values stored in the appsettings.json file.
        /// </summary>
        #region Class Members and Constructor

        IConfiguration _configuration;
        private string uriEncodedIexAPIUrl;
        private string iexApiToken;
        private readonly StockTraderContext _context;

        public StockHelperService(IConfiguration configuration, StockTraderContext context)
        {
            _configuration = configuration;
            _context = context;
            
        }
        #endregion

        /// <summary>
        /// This is used to retrieve my IEX Token from the appsettings.json file
        /// </summary>
        /// <returns>Method returns a string</returns>
        #region My IEX Token Retriever

        public string GetIexApiTokenFromConfig()
        {
             return _configuration.GetSection("iex").GetSection("iexApiToken").Value;
            
        }
        #endregion

        /// <summary>
        /// Used to get the current price of stock from the IEX API
        /// </summary>
        /// <param name="stockSymbol"></param>
        /// <returns>Method returns a double</returns>
        #region Stock Price Retriever
        public async Task<double> GetStockPrice(string stockSymbol)
        {
            iexApiToken = GetIexApiTokenFromConfig();
            
            uriEncodedIexAPIUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.Encode(new System.Uri($"https://cloud.iexapis.com/stable/stock/{stockSymbol}/quote/latestPrice?token={iexApiToken}"));

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(uriEncodedIexAPIUrl))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<double>(apiResponse);
                }
            }
 
        }
        #endregion

        /// <summary>
        /// Generates a JWT Token that is issued to a user after successful sign-in
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Object of type JwtSecurityToken</returns>
        #region JWT Tocken Generator
        public JwtSecurityToken GenerateAuthToken(User user)
        {

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //Read the string saved in the appsettings folder, convert it to bytes, and then use it as the signing key
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("jwtSecret").GetSection("secret").Value));

            var token = new JwtSecurityToken(
                issuer: "http://kingsleycokei.com",
                audience: "http://decagonhq.com",
                expires: DateTime.UtcNow.AddHours(1),
                claims: claims,
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            //var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return token;

        }
        #endregion

        /// <summary>
        /// This is used to serialize the generated JWT Token, so that it can be sent back to an
        /// authenticated user as part of a response
        /// </summary>
        /// <param name="token">JwtSecurityToken token</param>
        /// <returns>Method returns a string</returns>
        #region JWT Token Serializer
        public string SerializeToken(JwtSecurityToken token)
        {
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        ///<summary>
        ///This method gets the Stock for which a particular stock-purchase/sale transaction is to be recorded
        ///</summary>
        #region Method to get the particular Stock for which this transaction is to be recorded 
        
        public Stock GetStockForTransaction(StockToPurchaseOrSell stockToPurchase, User user)
        {

            //var existingStock = _context.Stocks.Include(u => u.User).FirstOrDefault(u => u.StockSecuritySymbol == stockToPurchase.Symbol);
            ////return existingStock;
            //var stocks = _context.Stocks.Include(s => s.User).Where(s => s.StockSecuritySymbol == stockToPurchase.Symbol);
            //Stock theStock = null;
            //foreach(var stock in stocks)
            //{
            //    if(stock.User.UserEmail==user.UserEmail)
            //    {
            //        theStock = stock;
            //        break;
            //    }
            //}
            return null;
        }
        #endregion

        ///<summary>
        ///An overload of the method that gets the current stock for which a particular stock-purchase/sale is to be recorded
        ///This method simply helps to check if a user already has stock for a particular security
        /// </summary>
        /// <param name="Stock">Stock stock</param>
        /// <param name="Stock">Stock stock</param>
        /// <param name="User">User user</param>
        #region Method To Get A Particular Stock

        public bool StockExists(Stock existingStock, User user)
        {
            //return _context.Stocks.FirstOrDefault(s => s.StockSecuritySymbol == existingStock.StockSecuritySymbol);
            return false;
        }

        #endregion

        ///<summary>
        ///This method gets the current user
        /// </summary>
        #region Method to get current user

        public User GetCurrentUser(string email, StockTraderContext ctext)
        {
            var currentUser = ctext.User.Include(u => u.Stock).FirstOrDefault(u => u.UserEmail == email);
            return currentUser;
        }

        #endregion

    }

}   
