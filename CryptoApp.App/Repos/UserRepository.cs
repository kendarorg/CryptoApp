using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CryptoApp.Repos
{
    public class UserRepository : BaseRepository
    {
        private static User _adminUser;

        public void Initialize()
        {
            SetupGodAdmin();
            try
            {
                ExecuteCommand("CREATE TABLE USERS (ID VARCHAR(40) PRIMARY KEY, LOGIN VARCHAR(254),PASSWORD VARCHAR(254),ISADMIN INT)");
                //ExecuteCommand("ALTER TABLE USERS ADD PRIMARY KEY (ID)", dbConnection);
                ExecuteCommand("CREATE UNIQUE INDEX UX_USERS_LOGIN ON USERS (LOGIN);");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void SetupGodAdmin()
        {
            var admin = ConfigurationManager.AppSettings["Admin"];
            var adminUp = admin.Split(';');
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1pwd = Encoding.UTF8.GetString(Convert.FromBase64String(adminUp[1]));// Encoding.UTF8.GetString(sha1.ComputeHash(Encoding.UTF8.GetBytes(adminUp[1])));
            _adminUser = new User
            {
                Id = Guid.Empty,
                IsAdmin = true,
                Login = adminUp[0],
                Password = sha1pwd
            };
        }

        public void Update(User user)
        {
            if (user.Id == _adminUser.Id) return;
            ExecuteCommand("UPDATE USERS SET ISADMIN=@ISADMIN, LOGIN=@LOGIN,  PASSWORD=@PWD WHERE ID=@ID",
                new Dictionary<String, Object>
                {
                        {"@ID",user.Id },
                        {"@LOGIN",user.Login },
                        {"@ISADMIN",user.IsAdmin },
                        {"@PWD",user.Password }
                });
        }
        public bool UpdatePassword(String userId, String oldPwd, String newPwd)
        {
            if (userId == _adminUser.Login) return false;
            //if (Login(userId, oldPwd))
            
            //{
                var sha1 = new SHA1CryptoServiceProvider();
                var sha1pwd = Encoding.UTF8.GetString(sha1.ComputeHash(Encoding.UTF8.GetBytes(newPwd)));
                ExecuteCommand("UPDATE USERS SET PASSWORD=@PWD WHERE ID=@USERID",
                    new Dictionary<String, Object>
                    {
                        {"@USERID",userId },
                        {"@PWD",sha1pwd },
                    });
                return true;
            /*}
            /*else
            {
                return false;
            }*/
        }

        public Guid Add(User user)
        {
            if (user.Login == _adminUser.Login) return _adminUser.Id;
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1pwd = Encoding.UTF8.GetString(sha1.ComputeHash(Encoding.UTF8.GetBytes(user.Password)));
            var key = Guid.NewGuid();
            ExecuteCommand("INSERT INTO USERS (ID,LOGIN,PASSWORD,ISADMIN) VALUES (@ID,@LOGIN,@PWD,@ISADMIN)",
                new Dictionary<String, Object>
                {
                        {"@ID",key },
                        {"@LOGIN",user.Login },
                        {"@PWD",sha1pwd },
                        {"@ISADMIN",user.IsAdmin }
                });
            user.Id = key;
            return key;
        }

        public void Delete(Guid userId)
        {
            ExecuteCommand("DELETE FROM USERS WHERE  ID=@ID",
                new Dictionary<String, Object>
                {
                        {"@ID",userId }
                });
        }

        public User GetById(Guid key)
        {
            if (key == _adminUser.Id) return _adminUser.CleanClone();
            var user = ExecuteSingle<User>("SELECT * FROM USERS WHERE ID=@ID",
                 new Dictionary<String, Object>
                 {
                        {"@ID",key },
                 });
            if (user != null)
            {
                return user.CleanClone();
            }
            return null;
        }

        public bool Login(String userId, String pwd)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1pwd = Encoding.UTF8.GetString(sha1.ComputeHash(Encoding.UTF8.GetBytes(pwd)));


            if (userId == _adminUser.Login)
            {
                if (_adminUser.Password == sha1pwd)
                {
                    return true;
                }
                return false;
            }
            var user = ExecuteSingle<User>("SELECT * FROM USERS WHERE LOGIN=@USERID",
                 new Dictionary<String, Object>
                 {
                        {"@USERID",userId },
                 });
            if (user != null)
            {
                if (string.IsNullOrWhiteSpace(user.Password)) return true;
                return user.Password == sha1pwd;
            }
            return false;
        }

        public bool IsAdmin(String login)
        {
            return GetByLogin(login).IsAdmin;
        }

        public User GetByLogin(String login)
        {
            if (login == _adminUser.Login)
            {
                return _adminUser.CleanClone();
            }

            var user = ExecuteSingle<User>("SELECT * FROM USERS WHERE LOGIN=@LOGIN",
                 new Dictionary<String, Object>
                 {
                        {"@LOGIN",login },
                 });
            if (user != null)
            {
                return user.CleanClone();
            }
            return null;

        }

        public IEnumerable<User> GetAll()
        {
            yield return _adminUser.CleanClone();
            foreach (var user in ExecuteList<User>("SELECT * FROM USERS"))
            {
                yield return user.CleanClone();
            }
        }
    }
}