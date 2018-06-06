using CryptoApp.Models;
using CryptoApp.Repos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace CryptoApp.Controllers
{
    [RoutePrefix("api/files")]
    public class FileController:ApiController
    {
        private LoginService _userSvc;
        private UserRepository _users;
        private FileRepository _files;
        private AttachRepository _attach;
        private StringCipher _crypt;

        public FileController()
        {
            _userSvc = new LoginService();
            _users = new UserRepository();
            _files = new FileRepository();
            _crypt = new StringCipher();
            _attach = new AttachRepository();
        }

        [Route("create/new")]
        [HttpPost]
        public Guid CreateNew(LoadFileModel model)
        {
            if (!model.FileName.ToLowerInvariant().EndsWith(".xml"))
            {
                model.FileName += ".xml";
            }
            
            var user = _userSvc.CurrentUser();
            Repos.File file = _files.GetByIdUserAndLabel(user.Id, model.FileName);
            var encriptionKey = model.FileKey;

            var crys = new CryptoService();
            var empty = crys.Empty;
            var root = crys.Initialize(empty);

            var strData = crys.Save();
           var encrypted = Encoding.UTF8.GetBytes(_crypt.Encrypt(strData, encriptionKey));
            if (file == null)
            {
                file = new Repos.File
                {
                    Id = Guid.NewGuid(),
                    Label = model.FileLabel,
                    Name = model.FileName,
                    UserId = user.Id,
                    Content = encrypted
                };
                _files.Add(file);
            }
            else
            {
                file.Content = encrypted;
                _files.Update(file);
            }
            _attach.DeleteFile(file.Id);
            foreach (var singleFile in root.Attachments)
            {
                //_attach.DeleteFile(Guid.Parse(singleFile.Id));
                var toAdd = new Attach
                {
                    Id = Guid.Parse(singleFile.Id),
                    UserId = user.Id,
                    FileId = file.Id,
                    Name = file.Name,
                    Data = singleFile.Data
                };
                _attach.Add(toAdd);
            }
            return file.Id;
        }


        [Route("load")]
        [HttpPost]
        public Item Load(LoadFileModel loadFile)
        {
            if (string.IsNullOrEmpty(loadFile.FileKey)) throw new Exception();
            var user = _userSvc.CurrentUser();
            var file = _files.GetByIdUserAndLabel(user.Id, loadFile.FileName);
            var strContent = Encoding.UTF8.GetString(file.Content);
            var cipher = new StringCipher();
            var decrypted = cipher.Decrypt(strContent,
                loadFile.FileKey).Trim();
            var ser = new CryptoService();
            if (decrypted.IndexOf("<") == 1)
            {
                decrypted = decrypted.Substring(1);
            }
            var root = ser.Initialize(decrypted);
            HttpContext.Current.Session["DATA"] = ser;
            HttpContext.Current.Session["FILE"] = loadFile.FileName;
            HttpContext.Current.Session["FILEKEY"] = loadFile.FileKey;
            return new Item
            {
                Id = root.Id,
                IsFolder = true,
                Title = root.Title
            };
        }

        [Route("save")]
        [HttpPost]
        public void Save()
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

        

        [Route("{fileLabel}")]
        [HttpPost]
        public Guid Upload(String fileLabel)
        {
            var user = _userSvc.CurrentUser();
            Repos.File file = _files.GetByIdUserAndLabel(user.Id, HttpContext.Current.Request.Files["file"].FileName);
            var encriptionKey = HttpContext.Current.Request.Params["encryptionKey"];
            using (var fileUploadStream = HttpContext.Current.Request.Files["file"].InputStream)
            {
                using (var ms = new MemoryStream())
                {
                    fileUploadStream.CopyTo(ms);
                    var data = ms.ToArray();

                    var strData = Encoding.UTF8.GetString(data);
                    var crys = new CryptoService();
                    var root = crys.Initialize(strData);
                    
                    strData = crys.Save();
                    
                    var encrypted = Encoding.UTF8.GetBytes(_crypt.Encrypt(strData, encriptionKey));
                    if (file == null)
                    {
                        file = new Repos.File
                        {
                            Id = Guid.NewGuid(),
                            Label = fileLabel,
                            Name = HttpContext.Current.Request.Files["file"].FileName,
                            UserId = user.Id,
                            Content = encrypted
                        };
                        _files.Add(file);
                    }
                    else
                    {
                        file.Content = encrypted;
                        _files.Update(file);
                    }
                    _attach.DeleteFile(file.Id);
                    foreach(var singleFile in root.Attachments)
                    {
                        //_attach.DeleteFile(Guid.Parse(singleFile.Id));
                        var toAdd= new Attach{
                    		Id= Guid.Parse(singleFile.Id),
                    		UserId=user.Id,
                    		FileId=file.Id,
                    		Name=file.Name,
                    		Data= singleFile.Data
                    	};
                    	_attach.Add(toAdd);
                    }
                    return file.Id;
                }
                 
            }

        }

        [Route("")]
        [HttpGet]
        public IEnumerable<FileItem> GetFiles()
        {
            var user = _userSvc.CurrentUser();
            foreach(var item in _files.GetAllByUser(user.Id))
            {
                yield return new FileItem
                {
                    FileLabel = item.Label,
                    FileName = item.Name
                };
            }
            
        }



        [Route("{fileName}")]
        [HttpDelete]
        public String Delete(String fileName)
        {
            var user = _userSvc.CurrentUser();
            var file = _files.GetByIdUserAndLabel(user.Id, fileName);
            
            _files.Delete(file.Id);
            _attach.DeleteFile(file.Id);
            return "OK";
        }
    }
}