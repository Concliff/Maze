using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public enum GridObjectType
    {
        GridObject,
        Coin,
        Portal,
        Bomb,
        Slime,
    };

    public enum GridObjectState : byte
    {
        OFF,
        Active,
        Inactive,
    };

    public class GridObject : Object
    {
        protected GridObjectState gridObjectState;
        protected GridObjectType gridObjectType;

        public GridObject()
        {
            gridObjectState = GridObjectState.Active;
            objectType = ObjectType.GridObject;
            gridObjectType = GridObjectType.GridObject;

            // Always in center of GridMap
            Position.X = 25;
            Position.Y = 25;
        }

        public GPS GetLocation() { return Position.Location; }

        public GridObjectType GetGridObjectType() { return gridObjectType; }

        public void SetGridObjectState(GridObjectState gridObjectState)
        {
            if (gridObjectState != GridObjectState.OFF)
            {
                this.gridObjectState = gridObjectState;
            }
        }

        public virtual void Use(Unit user) { }

        public bool IsActive() { return this.gridObjectState == GridObjectState.Active; }
    }
}
