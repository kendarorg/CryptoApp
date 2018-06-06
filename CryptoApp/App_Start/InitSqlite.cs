using CryptoApp.Repos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace CryptoApp.App_Start
{
    public static class InitSqlite
    {
        public static void Initialize()
        {
            new FileRepository().Initialize();
            new UserRepository().Initialize();
            new AttachRepository().Initialize();
            new AttemptRepository().Initialize();
        }
    }
}