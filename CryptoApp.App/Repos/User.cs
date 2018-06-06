using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoApp.Repos
{
    public class User
    {
        public bool IsAdmin { get; set; }
        public Guid Id { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }
        public DateTime LastAccess { get; set; }

        public User CleanClone()
        {
            return new User
            {
                IsAdmin = IsAdmin,
                Id = Id,
                Login = Login,
                Password = string.Empty,
                LastAccess = LastAccess
            };
        }
    }
}