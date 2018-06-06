using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using TB.ComponentModel;

namespace CryptoApp.Repos
{
    public class BaseRepository
    {
        private string _sqLiteFile;
        private string _sqLiteDs;
        private static ConcurrentDictionary<String, Dictionary<String, PropertyInfo>> _map =
            new ConcurrentDictionary<String, Dictionary<string, PropertyInfo>>(StringComparer.InvariantCultureIgnoreCase);

        protected BaseRepository()
        {
            _sqLiteFile = ConfigurationManager.AppSettings["SqLiteFile"];
            if (!Path.IsPathRooted(_sqLiteFile))
            {
                var codebase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codebase);
                var path = Uri.UnescapeDataString(uri.Path);
                var dir = Path.GetDirectoryName(path);
                if (dir.EndsWith(Path.DirectorySeparatorChar + "bin"))
                {
                    dir = Path.GetDirectoryName(dir);
                }
                _sqLiteFile = Path.Combine(dir, _sqLiteFile);

            }
            _sqLiteDs = ConfigurationManager.AppSettings["SqLiteDs"]
                .Replace("@SqLiteFile",_sqLiteFile);
        }

        protected void ExecuteCommand(String sql,  IDictionary<String, Object> pars = null)
        {
            SQLiteConnection c = null;
            try
            {
                c = Connect();
                var command = new SQLiteCommand(sql, c);
                ParseParameters(pars, command);
                command.ExecuteNonQuery();
            }
            finally
            {
                if (c != null) c.Close();
            }
        }



        protected IEnumerable<T> ExecuteList<T>(String sql, IDictionary<String, Object> pars = null) where T : class, new()
        {
            SQLiteConnection c = null;
            SQLiteDataReader reader = null;
            try
            {
                c = Connect();
                SQLiteCommand command = new SQLiteCommand(sql, c);
                ParseParameters(pars, command);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var t = new T();
                    Fill(t, typeof(T).Name, reader);
                    yield return t;
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (c != null) c.Close();
            }
        }

        protected T ExecuteSingle<T>(String sql, IDictionary<String, Object> pars = null) where T : class, new()
        {
            return ExecuteList<T>(sql, pars).FirstOrDefault();
        }

        protected T ExecuteOrdinal<T>(String sql, IDictionary<String, Object> pars = null)
        {
            SQLiteConnection c = null;
            SQLiteDataReader reader = null;
            try
            {
                c = Connect();

                SQLiteCommand command = new SQLiteCommand(sql, c);
                ParseParameters(pars, command);
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var value = UniversalTypeConverter.Convert(reader[0], typeof(T));
                    return (T)value;
                }
                return default(T);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (c != null) c.Close();
            }
        }

        private static void ParseParameters(IDictionary<string, object> pars, SQLiteCommand command)
        {
            if (pars != null)
            {
                foreach (var par in pars)
                {
                    if (par.Value != null && par.Value.GetType() == typeof(bool))
                    {
                        command.Parameters.AddWithValue(par.Key, ((bool)par.Value) ? 1 : 0);
                    }
                    else if (par.Value != null && par.Value.GetType() == typeof(Guid))
                    {
                        command.Parameters.AddWithValue(par.Key, par.Value.ToString());
                    }
                    else if (par.Value != null && par.Value.GetType() == typeof(byte[]))
                    {
                        command.Parameters.Add(par.Key, DbType.Binary, 20).Value= par.Value;
                    }
                    else if (par.Value != null && par.Value.GetType() == typeof(DateTime))
                    {
                        command.Parameters.AddWithValue(par.Key,
                            ((DateTime)par.Value).ToString("o", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        command.Parameters.AddWithValue(par.Key, par.Value);
                    }
                }
            }
        }

        protected void Fill<T>(T t, string name, SQLiteDataReader reader) where T : class, new()
        {
            var setters = _map.GetOrAdd(typeof(T).Name, key => CreateTypeDictionary(key, typeof(T)));

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var colName = reader.GetName(i);
                if (setters.ContainsKey(colName))
                {
                    Object value = null;
                    value = GetValue(reader, setters, i, colName);
                    setters[colName].GetSetMethod().Invoke(t, new object[] { value });
                }
            }
        }

        private static object GetValue(SQLiteDataReader reader, Dictionary<string, PropertyInfo> setters, int i, string colName)
        {
            if (setters[colName].PropertyType == typeof(bool))
            {
                var result = (int)UniversalTypeConverter.Convert(reader[i], typeof(int));
                return result == 1;
            }
            else if (setters[colName].PropertyType == typeof(byte[]))
            {
                return GetBytes(reader, i);
            }
            else if (setters[colName].PropertyType == typeof(Guid))
            {
                Guid outguid;
                if(reader[i]==null || !Guid.TryParse(reader[i].ToString(),out outguid))
                {
                    return Guid.Empty;
                }
                return outguid;
            }
            else if (setters[colName].PropertyType == typeof(DateTime))
            {
                DateTime d = DateTime.UtcNow;
                Object content = reader[i];
                if (content == null)
                {
                    return d;
                }
                if(!DateTime.TryParseExact (content.ToString(),
                    @"yyyy-MM-dd\THH:mm:ss.fff\Z", 
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out d))
                {
                    return DateTime.UtcNow;
                }
                return d;
            }
            else
            {
                return UniversalTypeConverter.Convert(reader[i], setters[colName].PropertyType);
            }
        }

        private static byte[] GetBytes(SQLiteDataReader reader,int fieldIndex)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (var stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(fieldIndex, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }

        private static Dictionary<string, PropertyInfo> CreateTypeDictionary(string key, Type type)
        {
            var result = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var property in type.GetProperties())
            {
                result[property.Name] = property;
            }
            return result;
        }

        private SQLiteConnection Connect()
        {
            var dbConnection = new SQLiteConnection(_sqLiteDs,true);
            dbConnection.Open();
            return dbConnection;
        }
    }
}