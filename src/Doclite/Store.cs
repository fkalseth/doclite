using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using SQLitePCL;

namespace Doclite
{
    public class Store : IDisposable
    {
        public static IStoreLogger Log = new DebugStoreLogger();

        public static Store Open(string file)
        {
            Log.Info("Connection to " + file);
            var connection = new SQLiteConnection(file);
            return new Store(connection);
        }
        
        private readonly SQLiteConnection _connection;

        public Store(SQLiteConnection connection)
        {
            _connection = connection;
        }

        public void Save(Document document)
        {
            var table = TableFor(document.GetType());
            Log.Info("Saving {0} into {1}", document.Key, table);

            EnsureTableExists(table);

            var json = JsonConvert.SerializeObject(document);

            Execute(() =>
            {
                using (var statement = _connection.Prepare(String.Format("insert or replace into {0} (key, data, timestamp) values(@key,@data, strftime('%s', 'now'))", table)))
                {
                    statement.Bind(1, document.Key);
                    statement.Bind(2, json);

                    var result = statement.Step();

                    if (result != SQLiteResult.DONE)
                    {
                        Log.Info("* Insert resulted in " + result);
                        throw new Exception();
                    }
                }
            });
        }

        private string TableFor(Type documentType)
        {
            return documentType.Name;
        }

        private void Execute(Action scope, bool transaction = true)
        {
            var watch = new Stopwatch();
            watch.Start();

            if (transaction)
            {
                using (var statement = _connection.Prepare("begin transaction"))
                {
                    statement.Step();
                }
            }

            try
            {
                scope();
                
                if (transaction)
                {
                    using (var statement = _connection.Prepare("commit transaction"))
                    {
                        statement.Step();
                    }
                }

                watch.Stop();
                Log.Info("Transaction completed in {0}ms", watch.ElapsedMilliseconds);
            }
            catch(Exception ex)
            {
                if (transaction)
                {
                    using (var statement = _connection.Prepare("rollback transaction"))
                    {
                        statement.Step();
                    }
                }

                watch.Stop();
                Log.Info("Exception:" + ex.Message);
                Log.Info("Transaction aborted in {0}ms", watch.ElapsedMilliseconds);

                throw;
            }
        }

        public T Get<T>(string key)
            where T : Document
        {
            Log.Info(String.Format("Getting {0} from {1}", key, TableFor(typeof(T))));

            T document = default(T);
            var table = TableFor(typeof(T));

            try
            {
                Execute(() =>
                {
                    using (var statement = _connection.Prepare(String.Format("select key, data, timestamp from {0} where key = @key", table)))
                    {
                        statement.Bind(1, key);

                        var result = statement.Step();

                        if (result != SQLiteResult.ROW)
                        {
                            Log.Info("No results found in table {0} for key {1} (sqlite result: {2})", table, key, result);
                            return;
                        }

                        document = ReadRow<T>(statement);
                    }

                }, transaction: false);

                return document;
            }
            catch (SQLiteException ex)
            {
                if (ex.Message.Contains("no such table"))
                {
                    Log.Info("No results found for key {0} because table {1} did not exist)", key, table);
                    return default(T);
                }

                throw;
            }
        }

        private T ReadRow<T>(ISQLiteStatement statement) where T : Document
        {
            var data = (string) statement["data"];
            var timestamp = (long) statement["timestamp"];
            var key = (string) statement["key"];

            var document = JsonConvert.DeserializeObject<T>(data);
            document.Timestamp = Parse(timestamp);
            document.Key = key;
            
            return document;
        }

        private DateTimeOffset Parse(long timestamp)
        {
            var dt = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
            dt = dt.AddSeconds(timestamp);
            return dt;
        }

        private void EnsureTableExists(string table)
        {
            using (var statement = _connection.Prepare(String.Format("select name from sqlite_master where type='table' and name='{0}'", table)))
            {
                var result = statement.Step();

                if (result != SQLiteResult.ROW)
                {
                    Log.Info("Creating table " + table);

                    var createStatement = _connection.Prepare(String.Format("create table {0}(key text primary key, data text, timestamp integer)", table));
                    var createResult = createStatement.Step();

                    if (createResult != SQLiteResult.DONE) Log.Info("* Create resulted in " + createResult);
                }
            }
        }

        public void Dispose()
        {
            Log.Info("Closing connection.");
            _connection.Dispose();
        }

        public void Delete(string table, string key)
        {
            Log.Info(String.Format("Deleting {0} from {1}", key, table));

            Execute(() =>
            {
                using (var statement = _connection.Prepare(String.Format("delete from {0} where key = @key", table)))
                {
                    statement.Bind(1, key);

                    var result = statement.Step();
                    if (result != SQLiteResult.DONE) Log.Info("* Delete resulted in " + result);
                }
            });
        }

        public IEnumerable<T> GetAll<T>() where T : Document
        {
            Log.Info(String.Format("Getting all from {0}", TableFor(typeof(T))));

            var table = TableFor(typeof(T));
            
            using (var statement = _connection.Prepare(String.Format("select key, data, timestamp from {0}", table)))
            {
                do
                {
                    var result = statement.Step();

                    if (result != SQLiteResult.ROW)
                    {
                        break;
                    }

                    yield return ReadRow<T>(statement);

                } while(true);
            }
        }
    }
}