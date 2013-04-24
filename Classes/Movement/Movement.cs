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
            /// <summary>
            ///  Main Direction. Used to unidirectional movement.
            /// </summary>
            public Directions First;

            /// <summary>
            /// Secondary Direction. Used to define diagonal movement.
            /// </summary>
            public Directions Second;

            public Direction(Directions first, Directions second)
            {
                First = first;
                Second = second;
            }

            public Direction(Directions first)
                : this(first, Directions.None) { }
        };

        /// <summary>
        /// Fraction part after conversion from double(speedRate) to int(Coords)
        /// </summary>
        protected double stepRemainder;

        public Map WorldMap = Map.WorldMap;

        /// <summary>
        /// Returns Orientation that was after last movement action handling.
        /// </summary>
        protected double pr_orientation;
        public double Orientation
        {
            get { return pr_orientation; }
            set { pr_orientation = value; }
        }

        public Movement()
        {
            Orientation = -1;
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
