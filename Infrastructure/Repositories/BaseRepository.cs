using System.Data;
using System.Data.SqlClient;
using Dapper;
using static Dapper.SqlMapper;

namespace Infrastructure.Repositories
{
    public class BaseRepository
    {
        protected string _connectionString;

        public BaseRepository(string connectionString) => _connectionString = connectionString;

        protected async Task<int> ExecuteAsync(string query, object param = null, CommandType? commandType = null)
        {
            using IDbConnection con = new SqlConnection(_connectionString);
            con.Open();

            return await con.ExecuteAsync(query, param, commandType: commandType);
        }

        protected async Task<List<T>> QueryAsync<T>(string query, object param = null, CommandType? commandType = null)
        {
            using IDbConnection con = new SqlConnection(_connectionString);
            con.Open();

            return (await con.QueryAsync<T>(query, param, commandType: commandType))?.AsList();
        }

        protected async Task<T> QueryFirstOrDefaultAsync<T>(string query, object param = null, CommandType? commandType = null)
        {
            using IDbConnection con = new SqlConnection(_connectionString);
            con.Open();

            return await con.QueryFirstOrDefaultAsync<T>(query, param, commandType: commandType);
        }

        protected async Task<T> MultipleQueryAsync<T>(string query, Func<GridReader, Task<T>> retornoHandler, object? param = null, CommandType? commandType = null)
        {
            var con = new SqlConnection(_connectionString);
            con.Open();

            using (var retorno = await con.QueryMultipleAsync(query, param, commandType: commandType))
            {
                return await retornoHandler(retorno);
            }
        }
    }
}
