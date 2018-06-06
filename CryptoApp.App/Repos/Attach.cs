using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoApp.Repos
{
    public class Attach
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid FileId { get; set; }
        public byte[] Data { get; set; }
        public String Name { get; set; }
    }
}