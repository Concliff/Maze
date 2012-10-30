using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public abstract class Movement
    {
        public struct Direction
        {
            public Directions First;
            public Directions Second;

            public Direction(Directions first, Directions second)
            {
                First = first;
                Second = second;
            }

            public Direction(Directions first)
                : this(first, Directions.None) { }

        };

        protected double stepRemainder;
        protected Direction pr_CurrentDirection;

        public Map WorldMap = Map.WorldMap;

        public Direction CurrentDirection
        {
            get
            {
                return pr_CurrentDirection;
            }
            protected set
            {
                pr_CurrentDirection = value;
            }
        }

        public static Directions GetOppositeDirection(Directions Direction)
        {
            switch (Direction)
            {
                case Directions.Left: return Directions.Right;
                case Directions.Right: return Directions.Left;
                case Directions.Down: return Directions.Up;
                case Directions.Up: return Directions.Down;
                default: return Directions.None;
            }
        }
    }
}
