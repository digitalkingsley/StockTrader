using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockTrader.Service.InfrastructureService;
using StockTrader.Service.DataService;
using Microsoft.Extensions.Configuration;
using StockTrader.StockTrader_Model;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockTrader.Controllers
{
 
    [Route("api/v1/[controller]")]
    public class StocksController : ControllerBase
    {
        private IStockService _stockService;
        private IStockHelperService _stockHelperService;
        private StockTraderContext _context;


        public StocksController(IStockService stockService, IStockHelperService stockHelperService, StockTraderContext context)
        {
            _stockService = stockService;
            _stockHelperService = stockHelperService;
            _context = context;
        }

        /// <summary>
        /// This EndPoint allows an authenticated user to get the latest price of stock for a particular security
        /// </summary>
        /// <param name="stockSymbol"></param>
        /// <returns>End Point returns a Json value representing the latest price of stock</returns>   
        #region End Point For Getting The Current Price Of Stock From IEX
        [Authorize]
        [HttpGet("price")]
        public async Task<IActionResult> GetStockPrice([FromBody] string stockSymbol)
        {
            if (string.IsNullOrEmpty(stockSymbol))
            {
                return BadRequest("No stock symbol supplied");
            }
            else
            {
                var latestPrice = await _stockService.GetStockPrice(stockSymbol);
                return Ok(latestPrice);
            }
        }
        #endregion

        /// <summary>
        /// This End Point allows an authenticated user to purchase stock 
        /// </summary>
        /// <param name="stock"></param>
        /// <returns>EndPoint returns a Transaction Object if successful</returns>
        #region EndPoint To Allow For Stock Purchase
        [Authorize]
        [HttpPost("buy")]
        public async Task<IActionResult> BuyStock([FromBody]StockToPurchaseOrSell stock)
        {
            try
            {
                string eMail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var currentUser = _stockHelperService.GetCurrentUser(eMail, _context);

                if (currentUser == null)
                {
                    return NotFound("User Not Found");
                }

                string report = await _stockService.AddStockShares(stock, currentUser);

                if (report == "Success")
                {
                    return Ok("Success");
                }
                else
                {
                    return Content("Stock Purchase Unsuccessful");
                }

            }catch(Exception ex)
            {
                return Content(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// EndPoint allows a user fetch all stocks that he/she has purchased
        /// </summary>
        /// <returns>EndPoint returns a response</returns>
        #region EndPoint To List All Stocks Purchased
        [Authorize]
        [HttpGet("all")]
        public IActionResult GetAllStocksPurchasedByUser()
        {
            try
            {
                //get current user
                string eMail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUser = _stockHelperService.GetCurrentUser(eMail, _context);

                var stocksPurchased = _stockService.GetStocksByUser(currentUser);
                if (stocksPurchased != null)
                {
                    return Ok(stocksPurchased);
                }
                else
                {
                    return Ok("No Stocks found for user");
                }
            }
            catch(Exception ex)
            {
                return Content(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// EndPoint allows a user to sell shares of stock they own
        /// </summary>
        /// <param name="stock"></param>
        /// <returns>EndPoint returns a Transaction Object if transaction is successful, otherwise, it </returns>
        #region Method To Allow For Stock Sale
        [Authorize]
        [HttpPost("sell")]
        public async Task<IActionResult> SellStocks([FromBody]StockToPurchaseOrSell stock)
        {
            if(stock.UnitPrice==0 || string.IsNullOrEmpty(stock.Symbol.ToString()))
            {
                return BadRequest("A required field is missing");
            }
                try
                {
                    string eMail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var currentUser = _stockHelperService.GetCurrentUser(eMail, _context);

                    if (currentUser != null)
                    {
                        string report = await _stockService.RemoveStockShares(stock, currentUser, _context);
                        if (report == "Success")
                        {
                            return Ok("Success");
                        }
                        else
                        {
                            return Content("Stock Sale Could Not Be Completed Successfully");
                        }
                    }
                    else
                    {
                        return NotFound("User Not Found");
                    }

                }
                catch (Exception ex) { return Content(ex.Message); }
         
        }
        #endregion
    }
}
