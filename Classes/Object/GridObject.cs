using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public enum GridObjectTypes
    {
        GridObject,
        OozeDrop,
        Portal,
        Bomb,
        Slime,
        Bonus,
        SmokeCloud,
    };

    public enum GridObjectStates : byte
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

    public struct AreaEffect
    {
        public ushort ID;
        public int Range;
    }

    public class GridObject : Object
    {
        protected int timeToLive;
        protected int activationTime;
        protected int timeToActivate;
        protected bool recentlyUsed;
        protected AreaEffect areaEffect;

        protected GridObjectStates gridObjectState;

        protected GridObjectTypes pr_gridObjectType;
        public GridObjectTypes GridObjectType
        {
            get
            {
                return this.pr_gridObjectType;
            }
            protected set
            {
                this.pr_gridObjectType = value;
            }
        }

        protected GridObjectFlags gridObjectsFlags;

        public GridObject()
        {
            gridObjectState = GridObjectStates.Active;
            objectType = ObjectType.GridObject;
            GridObjectType = GridObjectTypes.GridObject;

            // Always in center of the cell
            Position = new GPS(Position, 25, 25);

            // Flags by default
            SetFlag(GridObjectFlags.Usable);
            timeToLive = 0;
            activationTime = timeToActivate = 0;
            this.recentlyUsed = false;
        }

        public GridLocation GetLocation() { return Position.Location; }

        protected void SetFlag(GridObjectFlags flag)
        {
            if (!HasFlag(flag))
                this.gridObjectsFlags += (uint)flag;
        }
        protected void RemoveFlag(GridObjectFlags flag)
        {
            if (HasFlag(flag))
                this.gridObjectsFlags -= (uint)flag;
        }
        public bool HasFlag(GridObjectFlags flag)
        {
            return ((uint)flag & (uint)this.gridObjectsFlags) != 0;
        }


        public void SetGridObjectState(GridObjectStates gridObjectState)
        {
            if (gridObjectState != GridObjectStates.OFF)
            {
                this.gridObjectState = gridObjectState;
            }
        }

        public override void UpdateState(int timeP)
        {
            // Update life timer for Temporal GO
            if (HasFlag(GridObjectFlags.Temporal))
            {
                if (timeToLive < timeP)
                    SetObjectState(ObjectState.Removed);
                else
                    timeToLive -= timeP;
            }

            // Update activation timer for inactive GO
            if (this.recentlyUsed && !HasFlag(GridObjectFlags.Disposable))
            {
                if (timeToActivate < timeP)
                    SetGridObjectState(GridObjectStates.Active);
                else
                    timeToActivate -= timeP;
            }

            // AreaEffect GO
            // Apply effect on the nearest units
            if (HasFlag(GridObjectFlags.AreaEffect) && areaEffect.ID != 0)
            {
                List<Unit> units = GetUnitsWithinRange(areaEffect.Range);

                foreach (Unit unit in units)
                    unit.CastEffect(areaEffect.ID, unit);

            }

            base.UpdateState(timeP);
        }

        public virtual void Use(Unit user)
        {
            // Deactivate if needed
            if (!HasFlag(GridObjectFlags.AlwaysActive))
            {
                SetGridObjectState(GridObjectStates.Inactive);
                this.recentlyUsed = true;
                timeToActivate = activationTime;
            }

        }

        public bool IsActive() { return this.gridObjectState == GridObjectStates.Active; }
    }
}
