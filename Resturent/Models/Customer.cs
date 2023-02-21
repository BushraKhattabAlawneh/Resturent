using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Resturent.Models
{
    public partial class Customer
    {
        public Customer()
        {
            ProductCustomers = new HashSet<ProductCustomer>();
            UserLogins = new HashSet<UserLogin>();
        }

        public decimal Id { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public virtual ICollection<ProductCustomer> ProductCustomers { get; set; }
        public virtual ICollection<UserLogin> UserLogins { get; set; }
    }
}
