using System;
using System.Collections.Generic;

namespace GameCore
{
    public static class RandomUtils
    {
        public static Random rand = new();
        public static float D100F() => (float)(rand.NextDouble() * 100);
        public static float NextFloat() => (float)rand.NextDouble();
        public static T Sample<T>(List<T> list) => list[rand.Next(list.Count)];
        public static int RandomRound(float x)
        {
            var f = (int)Math.Floor(x);
            return f + (x - f >= NextFloat() ? 1 : 0);
        }
    }
}