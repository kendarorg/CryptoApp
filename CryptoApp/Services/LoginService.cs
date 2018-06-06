using CryptoApp.Repos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CryptoApp.Controllers
{
    public class LoginService
    {
        private AttemptRepository _attempts;
        private UserRepository _user;

        public LoginService()
        {
            _user = new UserRepository();
            _attempts = new AttemptRepository();
        }

        public User CurrentUser()
        {
            HttpContext.Current.Session["TST"] = DateTime.UtcNow.Ticks;
            var user = HttpContext.Current.Session["USER"] as User;
            if(user == null)
            {
                throw new Exception("Not loggedin");
            }
            return user;
        }

        public bool Login(string login, string password)
        {
            var user = _user.GetByLogin(login);
            if (!_attempts.CanLogin(user.Id))
            {
                Logoff();
                return false;
            }
            
            if (_user.Login(login, password))
            {
                
                HttpContext.Current.Session["USER"] = user;
                _attempts.Delete(user.Id);
                return true;
            }
            _attempts.Tentative(user.Id);
            Logoff();
            return false;
        }

        public void Logoff()
        {
            if (HttpContext.Current.Session == null) return;
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}