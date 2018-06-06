using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CryptoApp.Repos
{
    public class AttachRepository : BaseRepository
    {
        public void Initialize()
        {
            try
            {
                ExecuteCommand("CREATE TABLE ATTACH (ID VARCHAR(40) PRIMARY KEY,FILEID VARCHAR(40), USERID VARCHAR(254),NAME VARCHAR(254), DATA BLOB)");
                //ExecuteCommand("ALTER TABLE ATTACH ADD PRIMARY KEY (ID)", dbConnection);
                ExecuteCommand("CREATE UNIQUE INDEX UX_ATTACH_USERID_LABEL ON ATTACH(USERID,FILEID,ID);");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        public void DeleteFile(Guid fileId){
        	ExecuteSingle<Attach>("DELETE FROM ATTACH WHERE FILEID=@ID",
                 new Dictionary<String, Object>
                 {
                        {"@ID",fileId },
                 });
        }

        public Attach GetByIdUserAndLabel(Guid id, string name)
        {
            return ExecuteSingle<Attach>("SELECT ID,NAME,USERID,DATA FROM ATTACH WHERE USERID=@USERID AND NAME=@NAME",
                 new Dictionary<String, Object>
                 {
                        {"@USERID",id },
                        {"@NAME",name }
                 });
        }

        public void Update(Attach user)
        {
            ExecuteCommand("UPDATE ATTACH SET DATA=@DATA,NAME=@NAME  WHERE ID=@ID AND USERID=@USERID",
                    new Dictionary<String, Object>
                    {
                        {"@ID",user.Id },
                        {"@DATA",user.Data },
                        {"@NAME",user.Name },
                        {"@USERID",user.UserId }
                    });
        }

        public Guid Add(Attach user)
        {
            var key = user.Id;
            ExecuteCommand("INSERT INTO ATTACH (ID,FILEID,USERID,NAME,DATA) VALUES (@ID,@FILEID,@USERID,@NAME,@DATA)",
                new Dictionary<String, Object>
                {
                        {"@ID",key },
                        {"@NAME",user.Name },
                        {"@DATA",user.Data },
                        {"@USERID",user.UserId },
                        {"@FILEID",user.FileId }
                });
            user.Id = key;
            return key;
        }

        public Attach GetById(Guid key)
        {

            var user = ExecuteSingle<Attach>("SELECT * FROM ATTACH WHERE ID=@ID",
                 new Dictionary<String, Object>
                 {
                        {"@ID",key },
                 });
            return user;
        }

        public IEnumerable<Attach> GetAll(Guid key)
        {
            foreach (var user in ExecuteList<Attach>("SELECT ID,NAME,USERID FROM ATTACH"))
            {
                yield return user;
            }
        }

        public IEnumerable<Attach> GetAllByUser(Guid userId,Guid fileId)
        {
            foreach (var user in ExecuteList<Attach>("SELECT ID,NAME,USERID FROM ATTACH WHERE FILEID=@FILEID AND  USERID=@USERID",
                 new Dictionary<String, Object>
                 {
                        {"@USERID",userId },
                        {"@FILEID",fileId }
                 }))
            {
                yield return user;
            }
        }

        public Attach GetByIdUser(Guid attahcId, Guid userId,Guid fileId)
        {
            var user = ExecuteSingle<Attach>("SELECT * FROM ATTACH WHERE FILEID=@FILEID AND  ID=@ID AND USERID=@USERID",
                 new Dictionary<String, Object>
                 {
                        {"@ID",attahcId },
                        {"@USERID",userId },
                        {"@FILEID",fileId }
                 });
            return user;

        }
    }
}