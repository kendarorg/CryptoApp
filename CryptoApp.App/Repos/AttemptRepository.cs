using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CryptoApp.Repos
{
    public class AttemptRepository : BaseRepository
    {
        private const int MAX_COUNT = 3;
        private TimeSpan WAIT = TimeSpan.FromMinutes(10);
        public void Initialize()
        {
            try
            {
                ExecuteCommand("CREATE TABLE ATTEMPT (ID VARCHAR(40) PRIMARY KEY, COUNT NUMBER,FIRST VARCHAR(50),LAST VARCHAR(50))");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public bool Tentative(Guid userId)
        {
            Attempt attempt = GetById(userId);
            if (attempt == null)
            {
                InitializeAttempt(userId);
                return true;
            }
            if (attempt.Count >= MAX_COUNT)
            {
                if (DateTime.UtcNow < (attempt.Last + WAIT))
                {
                    UpdateLast(attempt.Id, attempt.Last);
                    return false;
                }
                ResetCount(attempt.Id);
            }
            else
            {
                IncrementCount(attempt);
            }
            return true;
        }

        public bool CanLogin(Guid id)
        {
            var attempt = GetById(id);
            if (attempt == null) return true;
            if (attempt.Count >= MAX_COUNT)
            {
                if (DateTime.UtcNow < (attempt.Last + WAIT))
                {
                    UpdateLast(attempt.Id, attempt.Last);
                    return false;
                }
            }
            return true;
        }

        public Attempt GetById(Guid userId)
        {
            return ExecuteSingle<Attempt>("SELECT * FROM ATTEMPT WHERE ID=@ID",
                             new Dictionary<String, Object>
                             {
                        {"@ID",userId },
                             });
        }


        public void Delete(Guid userId)
        {
            ExecuteCommand("DELETE FROM ATTEMPT WHERE ID=@ID",
                             new Dictionary<String, Object>
                             {
                        {"@ID",userId },
                             });
        }

        private void InitializeAttempt(Guid userId)
        {
            ExecuteCommand("INSERT INTO ATTEMPT (ID,COUNT,FIRST,LAST) VALUES (@ID,@COUNT,@FIRST,@LAST)",
                            new Dictionary<String, Object>
                            {
                        {"@ID",userId },
                        {"@COUNT",1 },
                        {"@FIRST",DateTime.UtcNow },
                        {"@LAST",DateTime.UtcNow }
                            });
        }

        private void ResetCount(Guid id)
        {
            ExecuteCommand("UPDATE ATTEMPT SET COUNT=@COUNT,FIRST=@FIRST,LAST=@LAST WHERE ID=@ID",
                            new Dictionary<String, Object>
                            {
                        {"@COUNT",1 },
                        {"@ID",id },
                        {"@FIRST",DateTime.UtcNow },
                        {"@LAST",DateTime.UtcNow }
                            });
        }


        private void UpdateLast(Guid id,DateTime last)
        {
            ExecuteCommand("UPDATE ATTEMPT SET LAST=@LAST WHERE ID=@ID",
                            new Dictionary<String, Object>
                            {
                        {"@LAST",last + WAIT },
                        {"@ID",id }
                            });
        }

        private void IncrementCount(Attempt attempt)
        {
            ExecuteCommand("UPDATE ATTEMPT SET COUNT=@COUNT,LAST=@LAST WHERE ID=@ID",
                            new Dictionary<String, Object>
                            {
                        {"@COUNT",attempt.Count+1 },
                        {"@ID",attempt.Id },
                        {"@LAST",DateTime.UtcNow }
                            });
        }
    }
}