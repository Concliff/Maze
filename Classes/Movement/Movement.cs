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

        protected Direction pr_CurrentDirection;
        /// <summary>
        /// Returns Direction that was after last movement action handling.
        /// </summary>
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

        public Movement()
        {
            CurrentDirection = new Direction(Directions.None, Directions.None);
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
