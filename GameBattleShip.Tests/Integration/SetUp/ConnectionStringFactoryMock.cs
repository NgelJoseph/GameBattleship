using GameBattleShip.Data;

namespace GameBattleShip.Tests.Integration.SetUp
{
    public class ConnectionStringFactoryMock : IConnectionStringFactory
    {
        public string GetConnectionString()
        {
            return ServerFixture.SqlConnectionString.Replace("battleship", "postgres") + "Password=password";
        }
    }
}
