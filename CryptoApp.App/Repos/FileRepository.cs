using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CryptoApp.Repos
{
    public class FileRepository : BaseRepository
    {
        public void Initialize()
        {
            try
            {
                ExecuteCommand("CREATE TABLE FILES (ID VARCHAR(40) PRIMARY KEY, USERID VARCHAR(254),LABEL VARCHAR(254),NAME VARCHAR(254), CONTENT BLOB)");
                //ExecuteCommand("ALTER TABLE FILES ADD PRIMARY KEY (ID)", dbConnection);
                ExecuteCommand("CREATE UNIQUE INDEX UX_FILES_USERID_LABEL ON FILES(USERID,NAME);");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public File GetByIdUserAndLabel(Guid id, string name)
        {
            return ExecuteSingle<File>("SELECT ID,NAME,LABEL,USERID,CONTENT FROM FILES WHERE USERID=@USERID AND NAME=@NAME",
                 new Dictionary<String, Object>
                 {
                        {"@USERID",id },
                        {"@NAME",name }
                 });
        }



        public Guid GetIdByIdUserAndLabel(Guid id, string name)
        {
            return ExecuteOrdinal<Guid>("SELECT ID FROM FILES WHERE USERID=@USERID AND NAME=@NAME",
                 new Dictionary<String, Object>
                 {
                        {"@USERID",id },
                        {"@NAME",name }
                 });
        }

        public void Update(File user)
        {
            ExecuteCommand("UPDATE FILES SET CONTENT=@CONTENT,LABEL=@LABEL,NAME=@NAME  WHERE ID=@ID AND USERID=@USERID",
                    new Dictionary<String, Object>
                    {
                        {"@ID",user.Id },
                        {"@CONTENT",user.Content },
                        {"@LABEL",user.Label },
                        {"@NAME",user.Name },
                        {"@USERID",user.UserId }
                    });
        }

        public Guid Add(File user)
        {
            var key = Guid.NewGuid();
            ExecuteCommand("INSERT INTO FILES (ID,USERID,NAME,LABEL,CONTENT) VALUES (@ID,@USERID,@NAME,@LABEL,@CONTENT)",
                new Dictionary<String, Object>
                {
                        {"@ID",key },
                        {"@NAME",user.Name },
                        {"@LABEL",user.Label },
                        {"@CONTENT",user.Content },
                        {"@USERID",user.UserId }
                });
            user.Id = key;
            return key;
        }

        public File GetById(Guid key)
        {

            var user = ExecuteSingle<File>("SELECT * FROM FILES WHERE ID=@ID",
                 new Dictionary<String, Object>
                 {
                        {"@ID",key },
                 });
            return user;
        }

        public IEnumerable<File> GetAll(Guid key)
        {
            foreach (var user in ExecuteList<File>("SELECT ID,NAME,LABEL,USERID FROM FILES"))
            {
                yield return user;
            }
        }

        public IEnumerable<File> GetAllByUser(Guid userId)
        {
            foreach (var user in ExecuteList<File>("SELECT ID,NAME,LABEL,USERID FROM FILES WHERE USERID=@USERID",
                 new Dictionary<String, Object>
                 {
                        {"@USERID",userId }
                 }))
            {
                yield return user;
            }
        }

        public File GetByIdUser(Guid key, Guid userId)
        {
            var user = ExecuteSingle<File>("SELECT * FROM FILES WHERE ID=@ID AND USERID=@USERID",
                 new Dictionary<String, Object>
                 {
                        {"@ID",key },
                        {"@USERID",userId }
                 });
            return user;

        }

        public void Delete(Guid id)
        {
            ExecuteCommand("DELETE FROM FILES WHERE ID=@ID",
                new Dictionary<String, Object>
                {
                        {"@ID",id }
                });
        }
    }
}