using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockTrader.StockTrader_Model
{
    public partial class User
    {
        public User()
        {
            Stock = new HashSet<Stock>();
            Transaction = new HashSet<Transaction>();
        }

        public int UserId { get; set; }

        [Required(ErrorMessage = "The UserName property is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The UserPassword property is required")]
        public string UserPassword { get; set; }

        [Required(ErrorMessage = "The UserEmail property is required")]
        public string UserEmail { get; set; }

        public double UserAccountBalance { get; set; }

        public virtual ICollection<Stock> Stock { get; set; }

        public virtual ICollection<Transaction> Transaction { get; set; }
    }

}
