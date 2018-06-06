using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace CryptoApp
{
    public class CryptoService
    {
        private AuthRoot root;
        private Dictionary<String, AuthBase> items = new Dictionary<string, AuthBase>(StringComparer.InvariantCultureIgnoreCase);

        public AuthRoot Root
        {
            get
            {
                return root;
            }
        }

        public CryptoService()
        {
        }

        public void RegisterNode(AuthBase node)
        {
            items[node.Id] = node;
        }

        public AuthBase FindById(String id)
        {
            return items[id];
        }

        public IEnumerable<AuthBase> Find(String find)
        {
            if (root == null)
            {
                return new List<AuthBase>();
            }
            return root.Find(find);
        }

        public IEnumerable<AuthBase> Find(Regex find)
        {
            if (root == null)
            {
                return new List<AuthBase>();
            }
            return root.Find(find);
        }
        /*
         * <pwentry>
	<group>WebSites</group>
	<title>CeframeworkDrupal</title>
	<username>edaros</username>
	<url>http://www.ceframework.com/?q=admin</url>
	<password>XXX</password>
	<notes>4+4 :P</notes>
	<uuid>f9717bd74c7c4fbc8de394bffe02b7ab</uuid>
	<image>0</image>
	<creationtime>2999-12-28T23:59:59</creationtime>
	<lastmodtime>2999-12-28T23:59:59</lastmodtime>
	<lastaccesstime>2999-12-28T23:59:59</lastaccesstime>
	<expiretime expires="false">2999-12-28T23:59:59</expiretime>
</pwentry>*/

        public AuthRoot Initialize(String content)
        {
            var start = content.IndexOf("<");
            if (start > 0)
            {
                content = content.Substring(start);
            }
            items.Clear();
            root = new AuthRoot();
            root.Id = Guid.Empty.ToString();
            root.Title = "root";
            items.Add(root.Id, root);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            XmlNodeList nodes = doc.SelectNodes("//pwentry");
            foreach (XmlNode node in nodes)
            {
                String group = LoadNode(node, "group");
                String groupTree = LoadNodeAttr(node, "group", "tree");
                String title = LoadNode(node, "title");
                String username = LoadNode(node, "username");
                String url = LoadNode(node, "url");
                String password = LoadNode(node, "password");
                String uuid = LoadNode(node, "uuid");
                String notes = LoadNode(node, "notes");
                String expiretime = LoadNode(node, "expiretime");
                String expireStr = LoadNodeAttr(node, "expiretime", "expire");
                if (String.IsNullOrWhiteSpace(expireStr))
                {
                    expireStr = "false";
                }

                String files = LoadNode(node, "files");


                bool expire = bool.Parse(expireStr);
                AuthLeaf leaf = new AuthLeaf();

                String attachdesc = LoadNode(node, "attachdesc");
                if (!String.IsNullOrWhiteSpace(attachdesc))
                {
                    leaf.HasAttachment = attachdesc;
                    String encodedString = LoadNode(node, "attachment");
                    var attach = new Attachment
                    {
                        Id = uuid,
                        Data = Convert.FromBase64String(encodedString),
                        Name = attachdesc
                    };
                    root.Attachments.Add(attach);
                }
                else if (!String.IsNullOrWhiteSpace(files))
                {
                    leaf.HasAttachment = files;
                }

                leaf.Id = uuid;
                leaf.Notes = notes;
                leaf.Url = url;
                leaf.UserName = username;
                leaf.Title = title;
                leaf.Password = password;

                items.Add(leaf.Id, leaf);

                leaf.CreationTime = DateTime.Parse(LoadNode(node, "creationtime"));
                leaf.LastAccessTime = DateTime.Parse(LoadNode(node, "lastaccesstime"));
                leaf.LastModifiedTime = DateTime.Parse(LoadNode(node, "lastmodtime"));
                leaf.ExpireTime = DateTime.MaxValue;
                if (!expire)
                {
                    leaf.ExpireTime = DateTime.Parse(expiretime);
                }
                var strs = new List<string>();
                if (groupTree != null && groupTree != String.Empty)
                {
                    var exp = groupTree.Split(new[] { '/', '\\' });
                    foreach (var exps in exp)
                    {
                        strs.Add(exps);
                    }
                }
                if (group != null && group != String.Empty)
                {
                    strs.Add(group);
                }

                AddToRoot(root, strs, leaf);


            }
            return root;
        }

        public String Save(Func<Guid, byte[]> withoutFiles = null)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf - 8\" standalone=\"yes\"?>\n");
            sb.Append("<pwlist>\n");
            foreach (var item in root.Children)
            {
                item.SerializeToXml(root, sb, withoutFiles);
            }
            sb.Append("</pwlist>");
            //SHOULD RELOAD THE HASHMAP
            return sb.ToString();
        }

        public String Empty
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"utf - 8\" standalone=\"yes\"?>\n");
                sb.Append("<pwlist>\n");
                
                sb.Append("</pwlist>");
                //SHOULD RELOAD THE HASHMAP
                return sb.ToString();
            }
        }

        private void AddToRoot(AuthTree root, List<string> strs, AuthLeaf leaf)
        {
            var currentItem = root;
            foreach (var str in strs)
            {
                AuthTree founded = null;
                foreach (var child in currentItem.Children)
                {
                    if (String.Compare(child.Title, str, true) == 0)
                    {
                        founded = child as AuthTree;
                        if (founded != null)
                        {
                            break;
                        }
                    }
                }
                if (founded == null)
                {
                    founded = new AuthTree();
                    founded.Title = str;
                    items.Add(founded.Id, founded);
                    currentItem.Children.Add(founded);
                    founded.Parent = currentItem;
                }
                currentItem = founded;
            }
            currentItem.Children.Add(leaf);
            leaf.Parent = currentItem;
        }

        private string LoadNodeAttr(XmlNode node, string v, String a)
        {

            foreach (var sub in node.ChildNodes)
            {
                XmlElement element = (XmlElement)sub;
                if (element.Name != v) continue;
                return element.GetAttribute(a);
            }
            return String.Empty;
        }

        private string LoadNode(XmlNode node, string v)
        {
            foreach (var sub in node.ChildNodes)
            {
                XmlElement element = (XmlElement)sub;
                if (element.Name != v) continue;
                return element.InnerText;
            }
            return String.Empty;
        }
    }
}
