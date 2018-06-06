using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApp
{
    public class AuthRoot :AuthTree
    {
        public AuthRoot()
        {
            Attachments = new List<Attachment>();
        }
        public List<Attachment> Attachments { get; set; }
    }
}
