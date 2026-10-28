using System.Collections.Generic;

namespace Game.Primary.GamePlaySystem.Rules
{
    /// <summary>
    /// RNG có seed để tái hiện bug. Mọi nhánh ngẫu nhiên của luật chơi đều đi qua đây.
    /// </summary>
    public class GameRng
    {
        private readonly System.Random random;

        public int Seed { get; }

        public GameRng(int seed)
        {
            Seed = seed;
            random = new System.Random(seed);
        }

        /// <summary> [min, max] bao gồm cả max </summary>
        public int RangeInclusive(int min, int max)
        {
            if (max <= min)
                return min;

            return random.Next(min, max + 1);
        }

        public double NextDouble() => random.NextDouble();

        public bool Chance(double probability) => random.NextDouble() < probability;

        public void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public T Pick<T>(IList<T> list) => list[random.Next(0, list.Count)];
    }
}
