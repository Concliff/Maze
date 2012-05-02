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

    public enum GridObjectFlags : uint
    {
        Usable          = 0x001,        // May apply GridObject.Use
        Disposable      = 0x002,        // Can be Used only once
        AreaEffect      = 0x004,        // Apply effects to the nearest units
        AlwaysActive    = 0x008,        // Can not be Inactive
        Temporal        = 0x010,        // Disappears after TTL
    };

    public class GridObject : Object
    {
        protected int timeToLive;
        protected int activationTime;
        protected int timeToActivate;
        protected bool recentlyUsed;

        protected GridObjectState gridObjectState;
        protected GridObjectType gridObjectType;
        protected GridObjectFlags gridObjectsFlags;

        public GridObject()
        {
            gridObjectState = GridObjectState.Active;
            objectType = ObjectType.GridObject;
            gridObjectType = GridObjectType.GridObject;

            // Always in center of GridMap
            Position.X = 25;
            Position.Y = 25;

            // Flags by default
            SetFlag(GridObjectFlags.Usable);
            timeToLive = 0;
            activationTime = timeToActivate = 0;
            recentlyUsed = false;
        }

        public GPS GetLocation() { return Position.Location; }

        public GridObjectType GetGridObjectType() { return gridObjectType; }

        protected void SetFlag(GridObjectFlags flag)
        {
            if (!HasFlag(flag))
                gridObjectsFlags += (uint)flag;
        }
        protected void RemoveFlag(GridObjectFlags flag)
        {
            if (HasFlag(flag))
                gridObjectsFlags -= (uint)flag;
        }
        public bool HasFlag(GridObjectFlags flag)
        {
            return ((uint)flag & (uint)gridObjectsFlags) != 0;
        }


        public void SetGridObjectState(GridObjectState gridObjectState)
        {
            if (gridObjectState != GridObjectState.OFF)
            {
                this.gridObjectState = gridObjectState;
            }
        }

        public override void UpdateState(int timeP)
        {
            if (HasFlag(GridObjectFlags.Temporal))
            {
                if (timeToLive < timeP)
                    SetObjectState(ObjectState.Removed);
                else
                    timeToLive -= timeP;
            }

            if (recentlyUsed && !HasFlag(GridObjectFlags.Disposable))
            {
                if (timeToActivate < timeP)
                    SetGridObjectState(GridObjectState.Active);
                else
                    timeToActivate -= timeP;
            }

            base.UpdateState(timeP);
        }

        public virtual void Use(Unit user)
        {
            if (!HasFlag(GridObjectFlags.AlwaysActive))
            {
                SetGridObjectState(GridObjectState.Inactive);
                recentlyUsed = true;
                timeToActivate = activationTime;
            }

        }

        public bool IsActive() { return this.gridObjectState == GridObjectState.Active; }
    }
}
