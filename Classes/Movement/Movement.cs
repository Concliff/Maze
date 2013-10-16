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

        protected bool pr_isOrientSet;

        public Movement()
        {
            pr_isOrientSet = false;
        }

        public static Directions WhatIsDirection(double orientation)
        {
            while (orientation >= 2 * Math.PI)
                orientation -= 2 * Math.PI;

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

        public static double GetOppositeDirection(double orientation)
        {
            //opposite angles:
            //0 - pi; pi/2 - 3pi/2; pi - 0; 3pi/2 - pi/2 
            //watch only the angles from 0 to 2pi
            return orientation >= Math.PI ? orientation - Math.PI : orientation + Math.PI;
        }

        public static double GetNeighbourOrientation(double orientation, int number) 
        { 
            //if number == 1 - left, if number == 2 - right
            //watch only the angles from 0 to 2pi
            if (number == 1) 
            { 
                return orientation >= 3 * Math.PI / 2 ? 0 : orientation + Math.PI / 2; 
            } 
            else
                return orientation < Math.PI / 2 ? 3 * Math.PI / 2 : orientation - Math.PI / 2; 
        }
    }
}
