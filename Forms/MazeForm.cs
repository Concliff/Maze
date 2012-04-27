using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Classes;

namespace Maze.Forms
{
    public partial class MazeForm : System.Windows.Forms.Form
    {
        private static UnitContainer unitContainer;
        private static Map worldMap;

        public static int FormTitleBarSize = 28;
        public static int FormBorderBarSize = 7;

        public MazeForm()
        {
            CreateWorldMap();
            CreateUnitContainer();
        }

        protected void CreateWorldMap()
        {
            worldMap = new Map();
        }

        protected void CreateUnitContainer()
        {
            unitContainer = new UnitContainer();
        }

        public static Map GetWorldMap()
        {
            return worldMap;
        }

        public static UnitContainer GetUnitContainer()
        {
            return unitContainer;
        }

        protected void SetNextAction(WorldNextAction NextAction)
        {
            World.SetNextAction(NextAction);
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
