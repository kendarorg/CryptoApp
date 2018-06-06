using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CryptoApp
{
    public class AuthLeaf: AuthBase
    {
        public AuthLeaf()
        {
            Id = Guid.NewGuid().ToString();
            ExpireTime = DateTime.MaxValue;
            LastAccessTime = DateTime.UtcNow;
            LastModifiedTime = DateTime.UtcNow;
            CreationTime = DateTime.UtcNow;
        }
        
		public String Url { get; set; }
		public String UserName { get; set; }
		public String Password { get; set; }
		public String Notes { get; set; }
        public DateTime ExpireTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public DateTime CreationTime { get; set; }
        public String HasAttachment { get; set; }

        public override bool Matches(Regex regex)
        {
            return base.Matches(regex)||
				regex.IsMatch(Url) ||
                regex.IsMatch(UserName) ||
                regex.IsMatch(Notes);
        }


        public override bool Matches(String regex)
        {
            regex = regex.ToLowerInvariant();
            return base.Matches(regex) ||
                Url.ToLowerInvariant().Contains(regex) ||
                UserName.ToLowerInvariant().Contains(regex) ||
                Notes.ToLowerInvariant().Contains(regex);
        }

        public override string ToString()
        {
            return Title;
        }

        public override void SerializeToXml(AuthRoot root,StringBuilder sb, Func< Guid, byte[]> withoutFiles)
        {
            sb.Append("<pwentry>\n");
            var groupTreePath = BuildGroup();
            var group = Parent.Title;
            if (!String.IsNullOrWhiteSpace(groupTreePath))
            {
                sb.AppendFormat("<group tree=\"{0}\">{1}</group>\n",groupTreePath,group);
            }
            else
            {
                sb.AppendFormat("<group>{0}</group>\n", group);
            }
            sb.AppendFormat("<title>{0}</title>\n", WebUtility.HtmlEncode(this.Title));
            sb.AppendFormat("<username>{0}</username>\n", WebUtility.HtmlEncode(this.UserName));
            sb.AppendFormat("<url>{0}</url>\n", WebUtility.HtmlEncode(this.Url));
             sb.AppendFormat("<password>{0}</password>\n", WebUtility.HtmlEncode(this.Password));
            sb.AppendFormat("<notes>{0}</notes>\n", WebUtility.HtmlEncode(this.Notes));
            sb.AppendFormat("<uuid>{0}</uuid>\n", WebUtility.HtmlEncode(this.Id.ToString()));
            sb.AppendFormat("<image>{0}</image>\n", WebUtility.HtmlEncode("0"));
            sb.AppendFormat("<creationtime>{0}</creationtime>\n", (this.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss")));
            sb.AppendFormat("<lastmodtime>{0}</lastmodtime>\n", (this.LastModifiedTime.ToString("yyyy-MM-ddTHH:mm:ss")));
            sb.AppendFormat("<lastaccesstime>{0}</lastaccesstime>\n", (this.LastAccessTime.ToString("yyyy-MM-ddTHH:mm:ss")));
            sb.AppendFormat("<expiretime expires=\"true\">{0}</expiretime>\n", (this.ExpireTime.ToString("yyyy-MM-ddTHH:mm:ss")));
            if(withoutFiles==null && !string.IsNullOrWhiteSpace(this.HasAttachment)){
            	sb.AppendFormat("<files>{0}</files>\n", WebUtility.HtmlEncode(this.HasAttachment));
            }else if(!string.IsNullOrWhiteSpace(this.HasAttachment)){

                var data = withoutFiles(Guid.Parse(this.Id));

                
            	if(data!=null){
            		sb.AppendFormat("<attachdesc>{0}</attachdesc>\n", WebUtility.HtmlEncode(HasAttachment));
            		var encodedData = Convert.ToBase64String(data);
            		sb.AppendFormat("<attachment>{0}</attachment>\n", WebUtility.HtmlEncode(encodedData));
            			
            	}
            	
            }
            sb.Append("</pwentry>\n");
        }

        private String BuildGroup()
        {
            var path = new List<String>();
            AuthTree current = this.Parent;
            path.Add(current.Title);
            while (current.Parent != null)
            {
                current = current.Parent;
                if (current == null)
                {
                    break;
                }
                path.Add(current.Title);
            }
            //Remove root
            path.RemoveAt(path.Count - 1);
            path.Reverse();
            //Remove last child (used directly from parent)
            path.RemoveAt(path.Count - 1);
            return String.Join("/", path);
        }
    }
}
