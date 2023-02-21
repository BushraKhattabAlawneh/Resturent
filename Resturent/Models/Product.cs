using System;
using System.Collections.Generic;

#nullable disable

namespace Resturent.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductCustomers = new HashSet<ProductCustomer>();
        }

        public decimal Id { get; set; }
        public string Namee { get; set; }
        public decimal? Sale { get; set; }
        public decimal? Price { get; set; }
        public decimal? CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<ProductCustomer> ProductCustomers { get; set; }
    }
}
