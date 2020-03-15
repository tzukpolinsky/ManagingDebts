using System;
using System.Collections.Generic;

namespace ManagingDebts
{
    public partial class Users
    {
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public int CustomerId { get; set; }
        public bool UserIsActive { get; set; }
        public string UserPassword { get; set; }
        public bool UserIsSuperAdmin { get; set; }

        public virtual Customers Customer { get; set; }
    }
}
