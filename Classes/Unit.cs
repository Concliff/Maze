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

        public void Update()
        {
            foreach (Unit unit in Units)
            {
                unit.Update();
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

        public double GetSpeedRate() { return SpeedRate; }
        public void SetSpeedRate(double SpeedRate) { this.SpeedRate = SpeedRate; }

        virtual public void UpdateState() { }
        virtual public void StartMotion() { }
    }
}
