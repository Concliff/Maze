using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Drawing;

namespace Maze.Classes
{
    public struct GridMap
    {
        public int ID;
        //public int PictureID;
        public GPS Location;
        public uint Type;
        public uint Attribute;
        public uint Option;
        public int OptionValue;
        public int ND4;

        // Define members by specific default vaules
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

        public bool HasAttribute(GridMapAttributes attribute)
        {
            return (Attribute & (uint)attribute) != 0;
        }

        public bool HasOption(GridMapOptions option)
        {
            return (Option & (uint)option) != 0;
        }

        public bool CanMoveTo(Directions direction)
        {
            return (Type & (uint)direction) != 0;
        }
    };

    public struct GPS
    {
        public int X;
        public int Y;
        public int Z;
        public int Level;

        public static bool operator ==(GPS a, GPS b)
        {
            if (a == null || b == null)
                return false;

            return (a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.Level == b.Level);
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
            return ((this.X ^ this.Y) + (this.Z ^ this.Level));
        }
    };

    // Location on current Block
    public struct GridGPS
    {
        public GPS Location;
        public int X;
        public int Y;
        public int BlockID;

        // Some constructors for the simplified initialization

        /// <summary>
        /// Initialize with Custom LOcation and X, Y block coords
        /// </summary>
        public GridGPS(GPS Location, int X, int Y)
        {
            this.Location = Location;
            this.X = X;
            this.Y = Y;
            this.BlockID = Map.WorldMap.GetGridMap(this.Location).ID;
        }

        /// <summary>
        /// Initialize by copying Position with custom X and Y coords
        /// </summary>
        public GridGPS(GridGPS Position, int X, int Y)
        {
            this = Position;
            this.X = X;
            this.Y = Y;
        }

        public static bool operator ==(GridGPS a, GridGPS b)
        {
            if (a == null || b == null)
                return false;

            return (a.Location == b.Location && a.X == b.X && a.Y == b.Y && a.BlockID == b.BlockID);
        }

        public static bool operator !=(GridGPS a, GridGPS b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            try
            {
                GridGPS p = (GridGPS)obj;
            }
            // Can not convert with any reason
            catch
            {
                return false;
            }

            return this == (GridGPS)obj;
        }

        public bool Equals(GridGPS p)
        {
            if (p == null)
                return false;

            return this == p;
        }

        public override int GetHashCode()
        {
            return ((this.Location.GetHashCode() ^ this.X ^ this.Y) + this.BlockID);
        }
    };

    // Not yet implemented
    public struct Picture
    {
        public int Type;
        public Image PictureImage;
    };

    public struct GridMapGraph
    {
        public Graphics Graphic;
        public GridMap Block;
    }
}
