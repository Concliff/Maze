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
    };

    public enum GridObjectState : byte
    {
        OFF,
        Active,
        Inactive,
    };

    public class GridObject : Object
    {
        protected GridObjectState objectState;
        protected GridObjectType gridObjectType;

        public GridObject()
        {
            objectState = GridObjectState.Active;
            objectType = ObjectType.GridObject;
            gridObjectType = GridObjectType.GridObject;

            // Always in center of GridMap
            Position.X = 25;
            Position.Y = 25;
        }

        public GPS GetLocation() { return Position.Location; }

        public GridObjectType GetObjectType() { return gridObjectType; }

        public void SetState(GridObjectState objectState)
        {
            if (objectState != GridObjectState.OFF)
            {
                this.objectState = objectState;
            }
        }

        public virtual void Use(Unit user) { }

        public bool IsActive() { return this.objectState == GridObjectState.Active; }
    }
}
