using CryptoApp.Models;
using CryptoApp.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

namespace CryptoApp.Controllers
{
    [RoutePrefix("api/user")]
    public class LoginController:ApiController
    {
        private LoginService _users;

        public LoginController()
        {
            _users = new LoginService();
        }

        [Route("enc/{toEnc}")]
        [HttpGet]
        public HttpResponseMessage Enc(String toEnc)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1pwd = Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(toEnc)));
            var resp = new HttpResponseMessage();
            resp.Content = new StringContent(sha1pwd);
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            return resp;
        }

        [Route("login")]
        [HttpPost]
        public void Login(LoginModel login)
        {
            if (!_users.Login(login.Login, login.Password))
            {
                throw new Exception("Invalid user");
            }
        }


        [Route("logoff")]
        [HttpGet]
        public void Logoff()
        {
            _users.Logoff();
        }
    }
}