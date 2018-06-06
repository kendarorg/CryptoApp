using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoApp.Models
{
    public class Item
    {
        public String Id { get; set; }
        public String Title { get; set; }
        public bool IsFolder { get; set; }
    }
}