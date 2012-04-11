using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class BinaryOperations
    {
        public static byte GetBit(int Number, byte Bit)
        {
            return (byte)((Number >> (Bit - 1)) & 1);
        }

        public static bool IsBit(int Number, byte Bit)
        {
            return (byte)((Number >> (Bit - 1)) & 1) == 1;
        }

        public static void SetBit(ref int Number, byte Bit)
        {
            if (IsBit(Number, Bit))
                return;
            Number += (int)Math.Pow(2, Bit - 1);
        }

        public static void SetBit(ref byte Number, byte Bit)
        {
            if (IsBit(Number, Bit))
                return;
            Number += (byte)Math.Pow(2, Bit - 1);
        }
    }
}
