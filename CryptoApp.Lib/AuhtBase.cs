using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CryptoApp
{
    public abstract class AuthBase
    {
        public String Id { get; set; }
        public String Title { get; set; }
        public AuthTree Parent { get; set; }

        public virtual bool Matches(Regex regex)
        {
            return regex.IsMatch(Title);
        }


        public virtual bool Matches(string find)
        {
            if (Title == null) return String.IsNullOrWhiteSpace(find);
            return Title.ToLowerInvariant().Contains(find.ToLowerInvariant());
        }

        public abstract void SerializeToXml(AuthRoot root,StringBuilder sb, Func<Guid,byte[]> withoutFiles);
    }
}
