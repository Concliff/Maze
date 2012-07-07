using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    public static class Random
    {
        private static System.Random random;

        public static void Initialize()
        {
            random = new System.Random();
        }
        /// <summary>
        /// Returns positive integer value
        /// </summary>
        /// <param name="?"></param>
        public static int Int(int maxValue)
        {
            return random.Next(maxValue);
        }

        public static int Int(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }
    }
}
