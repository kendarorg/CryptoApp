using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoApp.Models
{
    public class ItemData:Item
    {
        public String Url { get; set; }
        public String UserName { get; set; }
        public String Notes { get; set; }
        public String Password { get; set; }
        public DateTime ExpireTime { get; set; }
        public String ParentId { get; set; }
        public String HasAttachment {get;set;}
    }
}