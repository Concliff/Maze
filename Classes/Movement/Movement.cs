using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public abstract class Movement: ObjectOrientation
    {
        /// <summary>
        /// Fraction part after conversion from double(speedRate) to int(Coords)
        /// </summary>
        protected double stepRemainder;

        public Map WorldMap = Map.WorldMap;

        /// <summary>
        /// Returns Orientation that was after last movement action handling.
        /// </summary>
        public ObjectOrientation orientation;

        public Movement()
        {
            orientation = new ObjectOrientation();
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
