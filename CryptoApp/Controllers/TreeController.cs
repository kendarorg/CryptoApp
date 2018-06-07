using CryptoApp.Models;
using CryptoApp.Repos;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace CryptoApp.Controllers
{
    [RoutePrefix("api/tree")]
    public class TreeController : ApiController
    {


        [Route("backup")]
        [HttpGet]
        public HttpResponseMessage Dowload()
        {
            var user = _userSvc.CurrentUser();
            var cry = (CryptoService)HttpContext.Current.Session["DATA"];
            var file = (String)HttpContext.Current.Session["FILE"];
            var key = HttpContext.Current.Session["FILEKEY"].ToString();


            /* cry.Root


             var user = _userSvc.CurrentUser();
             Attach file = _attach.GetById(Guid.Parse(itemId));
             if (file != null && file.UserId != user.Id) throw new Exception();

             var key = HttpContext.Current.Session["FILEKEY"].ToString();
             var data = _crypt.DecryptBytes(file.Data, key);

             */

            String backup = cry.Save((guid) =>
            {
                try
                {
                    var res = _attach.GetById(guid);
                    if (res == null) return null;
                    return _crypt.DecryptBytes(res.Data, key);
                }
                catch
                {
                    return null;
                }
            });


            var bytes = Encoding.UTF8.GetBytes(backup);
            var ms = new MemoryStream();
            var fileName = Path.GetFileNameWithoutExtension(file);
            using (ZipFile zip = new ZipFile())
            {
                zip.Password = key;
                zip.AddEntry(fileName + ".xml", bytes);
                zip.Save(ms);
            }
            var result = new HttpResponseMessage
            {
                Content = new ByteArrayContent(ms.ToArray())
            };

            var nowString = DateTime.Now.ToString("MMddyyyyHHmm");

            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = nowString + "." + fileName + ".zip"
            };

            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");
            return result;
        }

        private LoginService _userSvc;
        private UserRepository _users;
        private FileRepository _files;
        private StringCipher _crypt;
        private AttachRepository _attach;

        public TreeController()
        {
            _attach = new AttachRepository();
            _files = new FileRepository();
            _userSvc = new LoginService();
            _users = new UserRepository();
            _crypt = new StringCipher();
        }
        private void Save()
        {
            var user = _userSvc.CurrentUser();
            var file = _files.GetByIdUserAndLabel(user.Id, HttpContext.Current.Session["FILE"].ToString());
            var data = HttpContext.Current.Session["DATA"] as CryptoService;
            var dataSaved = data.Save();
            var cipher = new StringCipher();
            var encrypted = cipher.Encrypt(dataSaved,
                HttpContext.Current.Session["FILEKEY"].ToString());

            file.Content = Encoding.UTF8.GetBytes(encrypted);
            _files.Update(file);
        }

        [Route("{id}/children")]
        [HttpGet]
        public IEnumerable<Item> LoadChildren(String id)
        {
            var user = _userSvc.CurrentUser();
            var cry = (CryptoService)HttpContext.Current.Session["DATA"];
            AuthTree item = cry.FindById(id) as AuthTree;
            if (item == null) yield break;
            foreach (var sub in item.Children)
            {
                yield return new Item
                {
                    Id = sub.Id,
                    IsFolder = sub is AuthTree,
                    Title = sub.Title
                };
            }
        }

        [Route("{id}")]
        [HttpGet]
        public Item GetData(String id)
        {
            var user = _userSvc.CurrentUser();
            var cry = (CryptoService)HttpContext.Current.Session["DATA"];
            AuthBase item = null;
            if (id == "rootitem")
            {
                item = cry.Root;
            }
            else
            {
                item = cry.FindById(id);
            }
            if (item == null) return null;
            return new Item
            {
                IsFolder = item is AuthTree,
                Id = item.Id,
                Title = item.Title
            };
        }

        [Route("search/{pattern}")]
        [HttpGet]
        public IEnumerable<ItemData> Search(String pattern)
        {
            var user = _userSvc.CurrentUser();
            var cry = (CryptoService)HttpContext.Current.Session["DATA"];
            foreach (var item in cry.Find(pattern))
            {
                var iteml = item as AuthLeaf;
                if (iteml == null) continue;

                yield return new ItemData
                {
                    ExpireTime = iteml.ExpireTime,
                    IsFolder = false,
                    Id = iteml.Id,
                    Notes = iteml.Notes,
                    Title = iteml.Title,
                    Url = iteml.Url,
                    UserName = iteml.UserName,
                    HasAttachment = iteml.HasAttachment
                };
            }
        }

        [Route("{id}/fullData")]
        [HttpGet]
        public ItemData GetFullData(String id)
        {
            var user = _userSvc.CurrentUser();
            var cry = (CryptoService)HttpContext.Current.Session["DATA"];

            var item = cry.FindById(id);
            AuthLeaf iteml = item as AuthLeaf;
            AuthTree itemt = item as AuthTree;
            if (iteml != null)
            {
                return new ItemData
                {
                    ExpireTime = iteml.ExpireTime,
                    IsFolder = false,
                    Id = iteml.Id,
                    Notes = iteml.Notes,
                    Title = iteml.Title,
                    Url = iteml.Url,
                    UserName = iteml.UserName,
                    HasAttachment = iteml.HasAttachment
                };
            }
            else if (itemt != null)
            {
                return new ItemData
                {
                    IsFolder = true,
                    Id = itemt.Id,
                    Title = itemt.Title
                };
            }
            return null;
        }

        [Route("{id}/password")]
        [HttpGet]
        public String GetPassword(String id)
        {
            var user = _userSvc.CurrentUser();
            var cry = (CryptoService)HttpContext.Current.Session["DATA"];
            AuthLeaf item = cry.FindById(id) as AuthLeaf;
            if (item == null) return String.Empty;
            item.LastAccessTime = DateTime.UtcNow;
            Save();
            return item.Password;
        }


        [Route("{id}/password")]
        [HttpPost]
        public void ChangePassword(PasswordChangeModel changeModel, String id)
        {
            var user = _userSvc.CurrentUser();
            if (changeModel.New != changeModel.NewRepeat) throw new Exception("Not matching passwords");
            var cry = (CryptoService)HttpContext.Current.Session["DATA"];
            var file = (String)HttpContext.Current.Session["FILE"];
            AuthLeaf item = cry.FindById(id) as AuthLeaf;
            //if (item.Password != changeModel.Old) throw new Exception("Not matching passwords");
            item.Password = changeModel.New;
            item.LastModifiedTime = DateTime.UtcNow;
            Save();
        }


        [Route("{id}/move/{newParent}")]
        [HttpPut]
        public void ChangeParent(String id, String newParent)
        {
            var user = _userSvc.CurrentUser();
            var cry = (CryptoService)HttpContext.Current.Session["DATA"];
            var file = (String)HttpContext.Current.Session["FILE"];
            AuthBase toUpdate = cry.FindById(id);
            var parentItem = cry.FindById(newParent) as AuthTree;
            if (toUpdate is AuthLeaf && parentItem == cry.Root)
            {
                throw new Exception("Cannot move leaves to root!");
            }

            toUpdate.Parent.Children.Remove(toUpdate);
            parentItem.Children.Add(toUpdate);
            toUpdate.Parent = parentItem;

            Save();
        }

        [Route("{id}")]
        [HttpDelete]
        public String DeleteItem(String id)
        {
            var user = _userSvc.CurrentUser();
            var cry = (CryptoService)HttpContext.Current.Session["DATA"];
            var file = (String)HttpContext.Current.Session["FILE"];
            AuthBase ida = cry.FindById(id) as AuthBase;
            ida.Parent.Children.Remove(ida);

            Save();
            return "ok";
        }


        [Route("")]
        [HttpPost]
        public void CreateItem(ItemData item)
        {
            var user = _userSvc.CurrentUser();
            var cry = (CryptoService)HttpContext.Current.Session["DATA"];
            var file = (String)HttpContext.Current.Session["FILE"];
            var parentId = item.ParentId;
            AuthTree ida = cry.FindById(parentId) as AuthTree;
            if (item.IsFolder)
            {
                var newNode = new AuthTree
                {
                    Title = item.Title,
                    Parent = ida
                };
                ida.Children.Add(newNode);
                cry.RegisterNode(newNode);
            }
            else
            {
                var newNode = new AuthLeaf
                {
                    Notes = item.Notes,
                    ExpireTime = item.ExpireTime,
                    Title = item.Title,
                    Url = item.Url,
                    UserName = item.UserName,
                    Password = item.Password,
                    Parent = ida,
                    HasAttachment = item.HasAttachment
                };
                ida.Children.Add(newNode);
                cry.RegisterNode(newNode);
            }
            Save();
        }

        [Route("")]
        [HttpPut]
        public String UpdateItem(ItemData item)
        {
            var user = _userSvc.CurrentUser();
            var cry = (CryptoService)HttpContext.Current.Session["DATA"];
            var file = (String)HttpContext.Current.Session["FILE"];
            var toUpdate = cry.FindById(item.Id);
            var parentId = item.ParentId;
            if (String.IsNullOrWhiteSpace(parentId))
            {
                parentId = toUpdate.Parent.Id;
            }
            var parentItem = cry.FindById(parentId) as AuthTree;
            AuthTree toUpdatet = toUpdate as AuthTree;
            AuthLeaf toUpdatel = toUpdate as AuthLeaf;

            if (parentItem == cry.Root && toUpdatet == null)
            {
                throw new Exception("ONLY FOLDERS CAN BE ADDED TO ROOT!!");
            }
            if (parentItem.Id != toUpdate.Parent.Id)
            {
                toUpdate.Parent.Children.Remove(toUpdate);
                parentItem.Children.Add(toUpdate);
                toUpdate.Parent = parentItem;
            }
            if (toUpdatet != null)
            {
                toUpdatet.Title = item.Title;
            }
            else if (toUpdatel != null)
            {
                toUpdatel.Notes = item.Notes;
                toUpdatel.UserName = item.UserName;
                toUpdatel.Url = item.Url;
                toUpdatel.Title = item.Title;
                toUpdatel.LastModifiedTime = DateTime.UtcNow;
                toUpdatel.HasAttachment = item.HasAttachment;
            }
            Save();
            return item.Id;
        }

    }
}
