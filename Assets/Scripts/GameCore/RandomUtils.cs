using System;
using System.Collections.Generic;

namespace GameCore
{
    public static class RandomUtils
    {
        public static Random rand = new();
        public static float D100F() => (float)(rand.NextDouble() * 100);
        public static float NextFloat() => (float)rand.NextDouble();
        public static int Next(int n) => rand.Next(n);
        public static int D6() => Next(6) + 1;
        public static int D10() => Next(10) + 1;
        public static int D20() => Next(20) + 1;
        public static T Sample<T>(List<T> list) => list[rand.Next(list.Count)];
        public static int RandomRound(float x)
        {
            var f = (int)Math.Floor(x);
            return f + (x - f >= NextFloat() ? 1 : 0);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}