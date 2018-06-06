using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoApp.Models
{
    public class PasswordChangeModel
    {
        public String Old { get; set; }
        public String New { get; set; }
        public String NewRepeat { get; set; }
    }
}