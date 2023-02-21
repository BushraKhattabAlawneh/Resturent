using System;
using System.Collections.Generic;

#nullable disable

namespace Resturent.Models
{
    public partial class Role
    {
        public Role()
        {
            UserLogins = new HashSet<UserLogin>();
        }

        public decimal Id { get; set; }
        public string Rolename { get; set; }

        public virtual ICollection<UserLogin> UserLogins { get; set; }
    }
}
