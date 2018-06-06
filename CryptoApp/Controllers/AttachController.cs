using CryptoApp.Models;
using CryptoApp.Repos;
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
    [RoutePrefix("api/attach")]
    public class AttachController : ApiController
    {
        private LoginService _userSvc;
        private UserRepository _users;
        private FileRepository _files;
        private StringCipher _crypt;
        private AttachRepository _attach;

        public AttachController()
        {
            _userSvc = new LoginService();
            _users = new UserRepository();
            _files = new FileRepository();
            _crypt = new StringCipher();
            _attach = new AttachRepository();
        }

        [Route("{itemId}")]
        [HttpGet]
        public HttpResponseMessage Get(String itemId)
        {
            var user = _userSvc.CurrentUser();
            Attach file = _attach.GetById(Guid.Parse(itemId));
            if (file != null && file.UserId != user.Id) throw new Exception();

            var key = HttpContext.Current.Session["FILEKEY"].ToString();
            var data = _crypt.DecryptBytes(file.Data, key);


            var result = new HttpResponseMessage
            {
                Content = new ByteArrayContent(data)
            };
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = file.Name
            };
            var mime =MimeTypes.GetMimeType(file.Name);
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mime);
            return result;
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

        [Route("{itemId}")]
        [HttpPost]
        public Guid Upload(String itemId)
        {
            var user = _userSvc.CurrentUser();
            Attach file = _attach.GetById(Guid.Parse(itemId));
            if (file!=null && file.UserId != user.Id) throw new Exception();
            if (HttpContext.Current.Request.Files["file"] == null)
            {
                throw new Exception();
            }
            var encriptionKey = HttpContext.Current.Session["FILEKEY"].ToString();
            using (var fileUploadStream = HttpContext.Current.Request.Files["file"].InputStream)
            {
                using (var ms = new MemoryStream())
                {
                    fileUploadStream.CopyTo(ms);
                    var data = ms.ToArray();

                    var strData = Encoding.UTF8.GetString(data);
                    
                    
                    var cry = (CryptoService)HttpContext.Current.Session["DATA"];
                    AuthLeaf ida = cry.FindById(itemId) as AuthLeaf;
                    ida.HasAttachment = HttpContext.Current.Request.Files["file"].FileName;

                    var id =_files.GetIdByIdUserAndLabel(user.Id, HttpContext.Current.Session["FILE"].ToString());

                    var encrypted = _crypt.Encrypt(data, encriptionKey);
                    if (file == null)
                    {
                        file = new Attach
                        {
                            Id = Guid.Parse(itemId),
                            Data = encrypted,
                            FileId = id,
                            Name = HttpContext.Current.Request.Files["file"].FileName,
                            UserId = user.Id
                        };
                        _attach.Add(file);
                    }
                    else
                    {
                        file.Data = encrypted;
                        file.Name = HttpContext.Current.Request.Files["file"].FileName;
                        _attach.Update(file);
                    }
                    Save();
                    

                    return file.Id;
                }

            }

        }
    }
}
