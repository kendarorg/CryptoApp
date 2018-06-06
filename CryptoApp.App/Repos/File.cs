using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoApp.Repos
{
    public class File
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public byte[] Content { get; set; }
        public String Name { get; set; }
        public String Label { get; set; }
    }
}