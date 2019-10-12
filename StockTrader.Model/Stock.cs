using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockTrader.StockTrader_Model
{
    /// <summary>
    /// This model represents a Stock Entity and is generated from the database
    /// using the Scaffold-DbContext Command.
    /// </summary>
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

    /// <summary>
    /// Every request-body for either the purchase or sale of shares of stock must be an object of this type
    /// </summary>
    public class StockToPurchaseOrSell
    {
        public string SecurityName { get; set; }

        public string Symbol { get; set; }

        public double UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}
