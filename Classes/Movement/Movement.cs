using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents the base class for implementing various motion types.
    /// </summary>
    public abstract class Movement
    {
        /// <summary>
        /// Represents the information about direction the generator moves in.
        /// </summary>
        public struct Direction
        {
            /// <summary>
            ///  Main Direction. Is used for simple unidirectional movement.
            /// </summary>
            public Directions First;

            /// <summary>
            /// Secondary Direction. Is used for complex diagonal movement.
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
        /// Gets or sets the Direction that was after last movement action handling.
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

        /// <summary>
        /// Initializes a new instance of the Movement class.
        /// </summary>
        public Movement()
        {
            CurrentDirection = new Direction(Directions.None, Directions.None);
        }

        /// <summary>
        /// Determines what is the opposite direction for the specified one.
        /// </summary>
        /// <param name="direction">An initial direction</param>
        /// <returns>Direction that is opposite to given one.</returns>
        public static Directions GetOppositeDirection(Directions direction)
        {
            switch (direction)
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
