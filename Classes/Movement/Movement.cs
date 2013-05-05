using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public abstract class Movement
    {
        public const double ORIENTATION_RIGHT = 0;
        public const double ORIENTATION_UP = Math.PI / 2;
        public const double ORIENTATION_LEFT = Math.PI;
        public const double ORIENTATION_DOWN = 3 * Math.PI / 2;

        public const double QuarterAngle = Math.PI / 2;

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
        public double Orientation;

        public Movement()
        {
            Orientation = ORIENTATION_RIGHT;
        }

        public static Directions WhatIsDirection(double orientation)
        {
            while (orientation >= 4 * QuarterAngle)
                orientation -= 4 * QuarterAngle;

            if (orientation == ORIENTATION_RIGHT)
                return Directions.Right;
            else if (orientation == ORIENTATION_UP)
                return Directions.Up;
            else if (orientation == ORIENTATION_LEFT)
                return Directions.Left;
            else if (orientation == ORIENTATION_DOWN)
                return Directions.Down;
            else
                return Directions.None;
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
