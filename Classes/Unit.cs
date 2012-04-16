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
        private int UnitCounts;
        private ArrayList Units;

        public UnitContainer()
        {
            UnitCounts = 0;
            Units = new ArrayList();
        }

        public int GetNextGuid() { return UnitCounts + 1; }

        public int CreateUnit(Unit NewUnit)
        {
            ++UnitCounts;
            Units.Add(NewUnit);
            return UnitCounts - 1;
        }

        public Unit GetUnitByGUID(int GUID)
        {
            for (int i = 0; i < UnitCounts; ++i)
                if (((Unit)Units[i]).GetGUID() == GUID)
                    return (Unit)Units[i];
            return null;
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

        protected double GetSpeedRate() { return SpeedRate; }
        protected void SetSpeedRate(double SpeedRate) { this.SpeedRate = SpeedRate; }
    }
}
