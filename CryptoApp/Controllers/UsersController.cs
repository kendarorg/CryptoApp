using CryptoApp.Models;
using CryptoApp.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace CryptoApp.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private AttemptRepository _attempts = new AttemptRepository();
        private UserRepository _users = new UserRepository();
        private LoginService _login = new LoginService();

        [Route("")]
        [HttpGet]
        public IEnumerable<User> Get()
        {
            User current = _login.CurrentUser();
            if (!current.IsAdmin)
            {
                var res = _users.GetById(current.Id);
                res.Password = null;
                yield return res;
            }
            else
            {
                foreach(var user in _users.GetAll())
                {
                    user.Password = null;
                    yield return user;
                }
            }
        }

        [Route("{id}")]
        [HttpGet]
        public User Get(String id)
        {
            User current = _login.CurrentUser();
            if(!current.IsAdmin && id != current.Id.ToString())
            {
                throw new Exception();
            }
            var res = _users.GetById(Guid.Parse(id));
            res.Password = null;
            return res;
        }

        [Route("")]
        [HttpPost]
        public String Post(UserWithPassword value)
        {
            if (value.Password != value.PasswordNew)
            {
                throw new Exception("Not matching passwords");
            }
            User current = _login.CurrentUser();
            if (!current.IsAdmin)
            {
                throw new Exception();
            }
            
            _users.Add(new Repos.User
            {
                Id =Guid.NewGuid(),
                IsAdmin = value.IsAdmin,
                Login = value.Login,
                Password = value.Password
            });
            return value.Id.ToString();
        }

        [Route("{id}")]
        [HttpPut]
        public void Put(String id, User value)
        {
            User current = _login.CurrentUser();
            if (!current.IsAdmin && id != current.Id.ToString())
            {
                throw new Exception();
            }
            var user = _users.GetById(Guid.Parse(id));
            if (current.IsAdmin)
            {
                user.IsAdmin = value.IsAdmin;
            }
            user.Login = value.Login;
            _users.Update(user);
        }

        [Route("{id}/password")]
        [HttpPut]
        public void Put(String id, UserWithPassword value)
        {
            if (value.Password != value.PasswordNew || string.IsNullOrWhiteSpace(value.Password)|| string.IsNullOrWhiteSpace(value.PasswordNew))
            {
                throw new Exception("Not matching passwords");
            }
            User current = _login.CurrentUser();
            if (!current.IsAdmin && id != current.Id.ToString())
            {
                throw new Exception();
            }
            var user = _users.GetById(Guid.Parse(id));
            
            _users.UpdatePassword(id,user.Password,value.PasswordNew);
            _attempts.Delete(Guid.Parse(id));
        }

        [Route("{id}")]
        [HttpDelete]
        public void Delete(String id)
        {
            User current = _login.CurrentUser();
            if (!current.IsAdmin || id == current.Id.ToString())
            {
                throw new Exception();
            }
            _users.Delete(Guid.Parse(id));
        }
    }
}