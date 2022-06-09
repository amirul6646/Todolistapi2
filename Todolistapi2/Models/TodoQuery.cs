using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace Todolistapi2.Models
{
    public class TodoQuery
    {
        public AppDb Db { get; }

        public TodoQuery(AppDb db)
        {
            Db = db;
        }

        public async Task<Todo> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `id`, `Task`, `Status` FROM `todos` WHERE `id` = @id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<List<Todo>> LatestPostsAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `id`, `Task`, `Status` FROM `todos`";
            return await ReadAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task DeleteAllAsync()
        {
            using var txn = await Db.Connection.BeginTransactionAsync();
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `todos`";
            await cmd.ExecuteNonQueryAsync();
            await txn.CommitAsync();
        }

        private async Task<List<Todo>> ReadAllAsync(DbDataReader reader)
        {
            var posts = new List<Todo>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new Todo(Db)
                    {
                        Id = reader.GetInt32(0),
                        Task = reader.GetString(1),
                        Status = reader.GetInt32(2),
                    };
                    posts.Add(post);
                }
            }
            return posts;
        }
    }
}
