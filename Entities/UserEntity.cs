using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class UserEntity
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int CustomerId { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuperAdmin{ get; set; }
        public string Password { get; set; }
    }
}
