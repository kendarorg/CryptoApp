using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoApp.Models
{
    public class UserWithPassword
    {
        public bool IsAdmin { get; set; }
        public Guid Id { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }
        public String PasswordNew { get; set; }
    }
}