using System;

namespace GameBattleShip.Helpers
{
    public class RandomGenerator : IRandomGenerator
    {
        public int Random(int startIndex, int endIndex)
        {
            var random = new Random();
            return random.Next(startIndex, endIndex);
        }
    }

    public interface IRandomGenerator
    {
        int Random(int startIndex, int endIndex);
    }
}
