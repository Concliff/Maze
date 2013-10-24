using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Drawing;

namespace Maze.Classes
{
    /// <summary>
    /// Represents a components (separate grid) of the Map. A square area with walls and free ways, having its own <see cref="GridLocation"/>.
    /// </summary>
    public struct Cell
    {
        public int ID;
        //public int PictureID;
        /// <summary>
        /// Position of the Cell on the Map.
        /// </summary>
        public GridLocation Location;
        public uint Type;
        public uint Attribute;
        public uint Option;
        public int OptionValue;
        /// <summary>
        /// Not Defined value.
        /// </summary>
        public int ND4;

        /// <summary>
        /// Defines members by specific default vaules.
        /// </summary>
        public void Initialize()
        {
            ID = -1;
            Type = 16;
            Location.X = 0;
            Location.Y = 0;
            Location.Z = 0;
            Location.Level = 0;
            Attribute = 0;
            Option = 0;
            OptionValue = 0;
            ND4 = 0;
        }

        /// <summary>
        /// Determines if the cell has the specified attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check</param>
        /// <returns><c>true</c> if the cell has the specified attribute; otherwise, <c>false</c>.</returns>
        public bool HasAttribute(CellAttributes attribute)
        {
            return (Attribute & (uint)attribute) != 0;
        }

        /// <summary>
        /// Determines if the cell has the specified option.
        /// </summary>
        /// <param name="option">The option to check</param>
        /// <returns><c>true</c> if the cell has the specified option; otherwise, <c>false</c></returns>
        public bool HasOption(CellOptions option)
        {
            return (Option & (uint)option) != 0;
        }

        /// <summary>
        /// Determines whether the specified direction is free to move from the cell (determines whether the wall is there).
        /// </summary>
        /// <param name="direction">The direction to check</param>
        /// <returns><c>true</c> if the direction has no any obstacles; otherwise, <c>false</c>.</returns>
        public bool CanMoveTo(Directions direction)
        {
            return (Type & (uint)direction) != 0;
        }
    };

    /// <summary>
    /// Descibes the positioning (or coordinates) of a grid (or a <see cref="Cell"/>).
    /// </summary>
    public struct GridLocation
    {
        public int X;
        public int Y;
        public int Z;
        public int Level;

        public GridLocation(int x, int y, int z, int level)
        {
            X = x;
            Y = y;
            Z = z;
            Level = level;
        }

        public static bool operator ==(GridLocation a, GridLocation b)
        {
            if (a == null || b == null)
                return false;

            return (a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.Level == b.Level);
        }

        public static bool operator !=(GridLocation a, GridLocation b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            try
            {
                GridLocation p = (GridLocation)obj;
            }
            // Can not convert with any reason
            catch
            {
                return false;
            }

            return this == (GridLocation)obj;
        }

        public bool Equals(GridLocation p)
        {
            if (p == null)
                return false;

            return this == p;
        }

        public override int GetHashCode()
        {
            return ((this.X ^ this.Y) + (this.Z ^ this.Level));
        }
    };

    /// <summary>
    /// Describes the positioning of any point on the Map. Includes <see cref="GridLocation"/> of the <see cref="Cell"/> where the point is and position inside the cell (X and Y coords).
    /// </summary>
    public struct GPS
    {
        public GridLocation Location;
        /// <summary>
        /// Gets or sets the absolute position of the GPS.
        /// </summary>
        public GridLocation Absolute
        {
            get
            {
                GridLocation absolute = new GridLocation();

                absolute.X = Location.X * GlobalConstants.CELL_WIDTH + X;
                absolute.Y = Location.Y * GlobalConstants.CELL_HEIGHT + Y;
                absolute.Z = Location.Z;
                absolute.Level = Location.Level;

                return absolute;
            }
            set
            {
                Location.X = value.X / GlobalConstants.CELL_WIDTH;
                if (value.X < 0)
                    Location.X--;
                Location.Y = value.Y / GlobalConstants.CELL_HEIGHT;
                if (value.Y < 0)
                    Location.Y--;
                Location.Z = value.Z;
                Location.Level = value.Level;

                X = value.X - Location.X * GlobalConstants.CELL_WIDTH;
                Y = value.Y - Location.Y * GlobalConstants.CELL_HEIGHT;
            }
        }
        public int X;
        public int Y;

        // Some constructors for the simplified initialization

        /// <summary>
        /// Initialize with Custom Location.
        /// X = Y = 25;
        /// </summary>
        public GPS(GridLocation location) : this(location, 25, 25) { }

        /// <summary>
        /// Initialize with Custom Location and X, Y block coords
        /// </summary>
        public GPS(GridLocation Location, int X, int Y)
        {
            this.Location = Location;
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Initialize by copying Position with custom X and Y coords
        /// </summary>
        public GPS(GPS Position, int X, int Y)
        {
            this = Position;
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Calculates the absolute distance to another GPS value.
        /// </summary>
        /// <param name="point">Target position.</param>
        /// <returns>Distance to the point (in arbitrary units or in visual pixels on the Map).</returns>
        public double GetDistance(GPS point)
        {
            double distance = Math.Sqrt(Math.Pow(this.X - point.X + (this.Location.X - point.Location.X) * GlobalConstants.CELL_WIDTH, 2)
                    + Math.Pow(this.Y - point.Y + (this.Location.Y - point.Location.Y) * GlobalConstants.CELL_HEIGHT, 2));

            return distance;
        }

        public static bool operator ==(GPS a, GPS b)
        {
            if (a == null || b == null)
                return false;

            return (a.Location == b.Location && a.X == b.X && a.Y == b.Y);
        }

        public static bool operator !=(GPS a, GPS b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            try
            {
                GPS p = (GPS)obj;
            }
            // Can not convert with any reason
            catch
            {
                return false;
            }

            return this == (GPS)obj;
        }

        public bool Equals(GPS p)
        {
            if (p == null)
                return false;

            return this == p;
        }

        public override int GetHashCode()
        {
            return (this.Location.GetHashCode() ^ this.X ^ this.Y);
        }
    };

    // Not yet implemented
    public struct Picture
    {
        public int Type;
        public Image PictureImage;
    };
}
