using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockTrader.Model
{
    public partial class Stock
    {
        public int StockId { get; set; }
        public string StockSecurityName { get; set; }
        public string StockSecuritySymbol { get; set; }
        public int StockShareNumber { get; set; }
        public int? StockPurchaseDay { get; set; }
        public int? StockPurchaseMonth { get; set; }
        public int? StockPurchaseYear { get; set; }
        public DateTime StockPurchaseDate { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
    }

    public class StockToPurchaseOrSell
    {
        [Required(ErrorMessage = "The name of the Security is required")]
        public string SecurityName { get; set; }

        [Required(ErrorMessage = "The Security's Symbol is required")]
        public string Symbol { get; set; }

        [Required(ErrorMessage = "The price of this stock is required")]
        public double UnitPrice { get; set; }

        [Required(ErrorMessage = "The Quantity/number of shares to be purchased is required")]
        public int Quantity { get; set; }
    }
}
