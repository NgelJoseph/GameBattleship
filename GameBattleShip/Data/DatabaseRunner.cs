using System;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace GameBattleShip.Data
{
    public class DatabaseRunner
    {
        public static async Task RunAsync(IConfiguration configuration)
        {
            try
            {
                var thisType = typeof(DatabaseRunner);

                var masterPassword = configuration["Database:MasterPassword"];
                var apiPassword = configuration["Database:ApiPassword"];

                var connectionString = configuration["Database:ConnectionString"].Replace("battleship", "postgres") + $"Password={masterPassword}";
                EnsureDatabase.For.PostgresqlDatabase(connectionString);

                var upgrader =
                    DeployChanges.To
                    .PostgresqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => !s.StartsWith($"{thisType.Namespace}.Scripts.Always_"))
                    .WithVariable("body", "$body$")
                    .WithVariable("apiPassword", apiPassword)
                    .LogToConsole()
                    .Build();

                var result = upgrader.PerformUpgrade();

                if (!result.Successful)
                {
                    throw result.Error;
                }
                Log.Logger.Information("Database upgrade successful!");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Failed to update database!!!");
            }
        }
    }
}
