using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Maze.Classes
{

    public enum UnitTypes
    {
        None = 0,
        Player = 1,
        Deimos = 2,
        Phobos = 3,
    };

    public class UnitContainer
    {
        private List<Unit> Units;

        public UnitContainer()
        {
            Units = new List<Unit>();
        }

        public int GetNextGuid() { return Units.Count; }

        public int CreateUnit(Unit NewUnit)
        {
            Units.Add(NewUnit);
            return Units.Count;
        }

        public Unit GetUnitByGUID(int GUID)
        {
            for (int i = 0; i < Units.Count; ++i)
                if (Units[i].GetGUID() == GUID)
                    return Units[i];
            return null;
        }

        public List<Unit> GetAllUnitsByGPS(GPS iGPS)
        {
            return Units.FindAll(p => p.Position.Location.Equals(iGPS));
        }

        public void UpdateState(int timeP)
        {
            foreach (Unit unit in Units)
            {
                unit.UpdateState(timeP);
            }
        }

        public void StartMotion()
        {
            foreach (Unit unit in Units)
            {
                unit.StartMotion();
            }
        }
    }

    public class Unit
    {
        public GridGPS Position;

        protected UnitTypes UnitType;
        protected int GUID;
        protected GridMap CurrentGridMap;
        protected double SpeedRate;

        public Unit()
        {
            UnitType = UnitTypes.None;
            GUID = World.GetUnitContainer().CreateUnit(this);
            SpeedRate = 1.0d;
        }

        public UnitTypes GetUnitType() { return UnitType; }

        public int GetGUID() { return GUID; }

        /// <summary>
        /// Change Unit's location by given block count in a given direction
        /// </summary>
        protected void ChangeGPSDueDirection(int BlockPassCount, Directions Direction)
        {
            switch (Direction)
            {
                case Directions.Up: Position.Location.Y -= BlockPassCount; break;
                case Directions.Down: Position.Location.Y += BlockPassCount; break;
                case Directions.Left: Position.Location.X -= BlockPassCount; break;
                case Directions.Right: Position.Location.X += BlockPassCount; break;
            }

            CurrentGridMap = GetWorldMap().GetGridMapByGPS(Position.Location);
        }

        protected Map GetWorldMap()
        {
            return World.GetWorldMap();
        }

        protected List<Unit> GetUnitsWithinRange(int RangeDistance)
        {
            List<Unit> Units = new List<Unit>();
            GPS SearchGPS = Position.Location;

            // How much grids use for search
            int GridToNorth = (int)Math.Ceiling(Math.Abs(Position.Y - RangeDistance) * 1d / GlobalConstants.GRIDMAP_BLOCK_HEIGHT);
            int GridToSouth = (int)Math.Floor(Math.Abs(Position.Y + RangeDistance) * 1d / GlobalConstants.GRIDMAP_BLOCK_HEIGHT);
            int GridToWest = (int)Math.Ceiling(Math.Abs(Position.X - RangeDistance) * 1d / GlobalConstants.GRIDMAP_BLOCK_WIDTH);
            int GridToEast = (int)Math.Floor(Math.Abs(Position.X + RangeDistance) * 1d / GlobalConstants.GRIDMAP_BLOCK_WIDTH);

            for (int width = Position.Location.X - GridToWest; width <= Position.Location.X + GridToEast; ++width)
                for (int height = Position.Location.Y - GridToNorth; height <= Position.Location.Y + GridToSouth; ++height)
                {
                    SearchGPS.X = width;
                    SearchGPS.Y = height;
                    Units.AddRange(World.GetUnitContainer().GetAllUnitsByGPS(SearchGPS));
                }

            // exclude itself
            Units.Remove(this);

            if (Units.Count == 0)
                return null;

            List<Unit> UnitsWithinRange = new List<Unit>();
            foreach (Unit unit in Units)
                // Calculate actual distance
                if (Math.Sqrt(Math.Pow(Position.X - unit.Position.X + (Position.Location.X - unit.Position.Location.X) * GlobalConstants.GRIDMAP_BLOCK_WIDTH, 2)
                    + Math.Pow(Position.Y - unit.Position.Y + (Position.Location.Y - unit.Position.Location.Y) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT, 2)) < RangeDistance)
                {
                    if (unit == this)
                        continue;
                    UnitsWithinRange.Add(unit);
                }

            return UnitsWithinRange;
        }

        public double GetSpeedRate() { return SpeedRate; }
        public void SetSpeedRate(double SpeedRate) { this.SpeedRate = SpeedRate; }

        virtual public void UpdateState(int timeP) { }
        virtual public void StartMotion() { }
    }
}
