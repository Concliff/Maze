using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
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

    public enum UnitType
    {
        None = 0,
        Player = 2,
        Deimos = 3,
    };  

    public class Unit
    {
        public GridGPS Position;

        protected UnitType unitType;
        protected int GUID;
        protected GridMap CurrentGridMap;

        public Unit()
        {
            GUID = World.GetUnitContainer().CreateUnit(this);
        }

        public UnitType GetUnitType() { return unitType; }

        public int GetGUID() { return GUID; }
    }
}
