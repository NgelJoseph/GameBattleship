using Microsoft.Extensions.Configuration;

namespace GameBattleShip.Data
{
    public interface IConnectionStringFactory
    {
        string GetConnectionString();
    }

    public class ConnectionStringFactory : IConnectionStringFactory
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            var connectionString = _configuration["Database:ConnectionString"].Replace("battleship", "postgres") + $"Password={_configuration["Database:MasterPassword"]}";
            return connectionString;
        }
    }
}
