using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoApp.Repos
{
    public class Attempt
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
        public DateTime First { get; set; }
        public DateTime Last { get; set; }

    }
}