using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class ObjectOrientation
    {
        protected double pr_Orientation;
        public double Orientation
        {
            get { return pr_Orientation; }
            set { pr_Orientation = value; }
        }

        public ObjectOrientation()
        {
            this.Orientation = -1;
        }

        public double GetOppositeOrientation(double orientation)
        {
            //0 - pi; pi/2 - 3pi/2; pi - 0; 3pi/2 - pi/2
            return orientation + Math.PI >= 2 * Math.PI ? orientation - Math.PI : orientation + Math.PI;
        }

        public double GetNeighbourOrientation(double orientation, int number)
        {
            // 1 - left, 2 - right
            if (number == 1)
            {
                return orientation + Math.PI / 2 >= 2 * Math.PI ? 0 : orientation + Math.PI / 2;
            }
            else
                return orientation - Math.PI / 2 < 0 ? 3 * Math.PI / 2 : orientation - Math.PI / 2;
        }

        public uint GetNumericValue(double orientation)
        {
            // right (0) = 1; down (3pi/2) = 2; left (pi) = 4; up (pi/2) = 8;
            if (orientation == 0)
                orientation = 2 * Math.PI;
            return (uint)Math.Pow(2, 4 - orientation * 2 / Math.PI);
        }
    }
}
