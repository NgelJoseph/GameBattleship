using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Serilog;

namespace GameBattleShip.Data
{
    public interface IData
    {
        Task<bool> IsHealthy();
        Task<List<T>> QueryAsyncList<T>(string sql, object parameters = null, int? commandTimeout = null);
        Task<T> QueryAsync<T>(string sql, object parameters = null, int? commandTimeout = null);
        Task<int> ExecuteAsync(string sql, object parameters = null, int? commandTimeout = null);
    }

    public class SqlData : IData
    {
        private readonly ILogger _logger;
        private readonly IConnectionStringFactory _connectionStringFactory;

        public SqlData(IConnectionStringFactory connectionStringFactory, ILogger logger)
        {
            _connectionStringFactory = connectionStringFactory;
            _logger = logger;
        }

        public async Task<bool> IsHealthy()
        {
            try
            {
                await QueryAsyncList<string>("SELECT 1");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Database unhealthy");
                return false;
            }
            return true;
        }

        public async Task<List<T>> QueryAsyncList<T>(string sql, object parameters = null, int? commandTimeout = null)
        {
            try
            {
                using (var connection = NewConnectionAsync())
                {
                    var queryAsync = await connection.QueryAsync<T>(sql, parameters, commandTimeout: commandTimeout);
                    return queryAsync.ToList();
                }
            }
            catch (Exception e)
            {
                LogDatabaseError(e, sql, parameters);
                throw;
            }
        }

        public async Task<T> QueryAsync<T>(string sql, object parameters = null, int? commandTimeout = null)
        {
            try
            {
                using (var connection = NewConnectionAsync())
                {
                    var queryAsync = await connection.QueryFirstAsync<T>(sql, parameters, commandTimeout: commandTimeout);
                    return queryAsync;
                }
            }
            catch (Exception e)
            {
                LogDatabaseError(e, sql, parameters);
                throw;
            }
        }

        public async Task<int> ExecuteAsync(string sql, object parameters = null, int? commandTimeout = null)
        {
            try
            {
                using (var connection = NewConnectionAsync())
                {
                    var result = await connection.ExecuteAsync(sql, parameters, commandTimeout: commandTimeout);
                    return result;
                }
            }
            catch (Exception e)
            {
                LogDatabaseError(e, sql, parameters);
                throw;
            }
        }

        private void LogDatabaseError(Exception exception, string sql, object parameters)
        {
            _logger.Error(exception, "Database Error {Sql} {@Params}", sql, parameters);
        }

        private NpgsqlConnection NewConnectionAsync()
        {
            var connectionString = _connectionStringFactory.GetConnectionString();
            _logger.Debug($"Hitting connection string {connectionString}");

            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
