﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Classes;

namespace Maze.Forms
{
    public partial class MazeForm : System.Windows.Forms.Form
    {
        public static int FormTitleBarSize = 28;
        public static int FormBorderBarSize = 7;

        protected Map GetWorldMap()
        {
            return Maze.World.GetWorldMap();
        }

        protected void SetNextAction(WorldNextAction NextAction)
        {
            World.SetNextAction(NextAction);
        }

        protected UnitContainer GetUnitContainer()
        {
            return World.GetUnitContainer();
        }

        protected void SetBit(ref int Number, byte Bit)
        {
            BinaryOperations.SetBit(ref Number, Bit);
        }

        protected void SetBit(ref byte Number, byte Bit)
        {
            BinaryOperations.SetBit(ref Number, (byte)Bit);
        }

        protected bool HasBit(int Number, byte Bit)
        {
            return BinaryOperations.IsBit(Number, Bit);
        }
    }
}
