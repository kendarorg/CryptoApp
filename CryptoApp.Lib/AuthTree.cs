using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CryptoApp
{
    public class AuthTree : AuthBase
    {
        public AuthTree()
        {
            Children = new List<AuthBase>();
            Id = Guid.NewGuid().ToString();
            IsTree = false;
        }

        public bool IsTree { get; set; }
        public IList<AuthBase> Children { get; set; }



        public IEnumerable<AuthBase> Find(string find)
        {
            if (this.Matches(find))
            {
                yield return this;
            }
            foreach (var item in Children)
            {
                var tree = item as AuthTree;
                if (tree == null && item.Matches(find))
                {
                    yield return item;
                }
                else if (tree != null)
                {
                    foreach (var sub in tree.Find(find))
                    {
                        yield return sub;
                    }
                }
            }
        }


        public IEnumerable<AuthBase> Find(Regex find)
        {
            if (this.Matches(find))
            {
                yield return this;
            }
            foreach (var item in Children)
            {
                var tree = item as AuthTree;
                if (tree == null && item.Matches(find))
                {
                    yield return item;
                }
                else if(tree!=null)
                {
                    foreach (var sub in tree.Find(find))
                    {
                        yield return sub;
                    }
                }
            }
        }

        public override void SerializeToXml(AuthRoot root,StringBuilder sb, Func< Guid, byte[]> files)
        {
            foreach(var item in Children)
            {
                item.SerializeToXml(root,sb,files);
            }
        }

        private String BuildGroup()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Title+"/";
        }
    }

}
