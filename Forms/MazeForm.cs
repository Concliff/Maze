using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Classes;

namespace Maze.Forms
{
    public partial class MazeForm : System.Windows.Forms.Form
    {
        private static ObjectContainer objectContainer;
        private static Map worldMap;

        public static int FormTitleBarSize = 28;
        public static int FormBorderBarSize = 7;

        public MazeForm()
        {
            CreateWorldMap();
            CreateObjectContainer();
        }

        protected void CreateWorldMap()
        {
            worldMap = new Map();
        }

        protected void CreateObjectContainer()
        {
            objectContainer = new ObjectContainer();
        }

        public static Map GetWorldMap()
        {
            return worldMap;
        }

        public static ObjectContainer GetObjectContainer()
        {
            return objectContainer;
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
