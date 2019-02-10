using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;
using Serilog;

namespace GameBattleShip.Tests.Integration.SetUp
{
    [SetUpFixture]
    public class OneTimeSetup
    {
        public string DbHost { get; set; }
        public static OneTimeSetup Default => Instance.Value;
        private static readonly string ExeSuffix = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : string.Empty;
        private static readonly Lazy<OneTimeSetup> Instance = new Lazy<OneTimeSetup>(Create);
        private readonly string _dockerPath;
        private string _sqlContainerName;

        public OneTimeSetup()
        {
            Log.Logger = new LoggerConfiguration()
                .CreateLogger();
        }

        private OneTimeSetup(string path)
        {
            _dockerPath = path;
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var isOnBuildServer = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BUILD_TAG"));
            Default.DbHost = !isOnBuildServer ? "localhost" : $"battleship-postgres-{Environment.GetEnvironmentVariable("BUILD_TAG")}";
            if (!isOnBuildServer)
            {
                await Default.CreateTestEnvironment();
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Default.Stop();
        }

        private async Task CreateTestEnvironment()
        {
            _sqlContainerName = "battleship-postgres-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var sqlPassword = "password";
            var dockerRunArguments = $"run --rm -d --name {_sqlContainerName} -e POSTGRES_PASSWORD={sqlPassword} -p 5432:5432  postgres";
            ProcessHelper.RunProcess(_dockerPath, dockerRunArguments);
            await WaitForDatabaseToBeReady(new ConnectionStringFactoryMock().GetConnectionString());
        }

        private void Stop()
        {
            ProcessHelper.RunProcess(_dockerPath, $"stop {_sqlContainerName}");
        }

        private static OneTimeSetup Create()
        {
            var location = GetDockerLocation();
            return location == null ? null : new OneTimeSetup(location);
        }

        private async Task WaitForDatabaseToBeReady(string connectionString)
        {
            for (var i = 0; i < 5; i++)
            {
                await Task.Delay((i + 1) * 2000);
                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        return;
                    }
                }
                catch (Exception e)
                {
                    Log.Logger.Warning(e, "Waiting for database to be ready...");
                }
            }
        }

        private static string GetDockerLocation()
        {
            foreach (var dir in Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator))
            {
                var candidate = Path.Combine(dir, "docker" + ExeSuffix);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return "/usr/bin/docker";
        }
    }
}
