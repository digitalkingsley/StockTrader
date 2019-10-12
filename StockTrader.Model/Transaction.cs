using System;
using System.Collections.Generic;

namespace StockTrader.StockTrader_Model
{
    /// <summary>
    /// This model represents a Stock Entity and is generated from the database
    /// using the Scaffold-DbContext Command.
    /// </summary>
    public partial class Transaction
    {
        public int TransactionId { get; set; }

        public string TransactionType { get; set; }

        public string TransactionDescription { get; set; }

        public int? TransactionDay { get; set; }

        public int? TransactionMonth { get; set; }

        public int? TransactionYear { get; set; }

        public DateTime? TransactionDate { get; set; }

        public int? StockId { get; set; }

        public int? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
