using System;
using System.Collections.Generic;
using System.IO;
using GameBattleShip.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace GameBattleShip.Tests.Integration.SetUp
{
    public class ServerFixture : IDisposable
    {
        public string Url { get; private set; }
        public static string SqlConnectionString { get; private set; }

        private readonly string _databaseName = $"BattleShipDb{Guid.NewGuid().ToString().Substring(0, 8)}";

        public SqlData SqlClient;

        private IWebHost _server;
        public ServerFixture()
        {
            StartServer();
        }

        private void StartServer()
        {
            SqlConnectionString = $"User ID=battleship;Host=localhost;Port=5432;Database={_databaseName};Pooling=true;";
            Log.Logger.Information("Connection String: {cs}", SqlConnectionString);
            Url = $"http://localhost:{PortHelper.GetAvailablePort()}";
            _server = StartMainHost(SqlConnectionString);
        }

        private IWebHost StartMainHost(string sqlConnectionString)
        {
            var host = WebHost.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, configuration) =>
                {
                    var configurationOverrides = new Dictionary<string, string>
                    {
                        ["Database:ConnectionString"] = sqlConnectionString,
                        ["Database:MasterPassword"] = "password",
                        ["Database:ApiPassword"] = "api_password"
                    };
                    configuration.SetBasePath(Directory.GetCurrentDirectory())
                        .AddInMemoryCollection(configurationOverrides);
                })
                .UseKestrel()
                .UseUrls(Url)
                .UseStartup<Startup>()
                .Build();

            SqlClient = new SqlData(new ConnectionStringFactoryMock(), Log.Logger);
            host.Start();
            return host;
        }

        private void StopHost(IWebHost host)
        {
            host?.StopAsync(TimeSpan.FromSeconds(5)).ContinueWith(t => host?.Dispose());
        }


        public void Dispose()
        {
            StopHost(_server);
            _server = null;
        }
    }
}
