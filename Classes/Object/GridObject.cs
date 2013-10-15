using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Specifies the type of object (or derived class) that an instance of the <see cref="GridObject"/> class represents.
    /// </summary>
    public enum GridObjectTypes
    {
        /// <summary>
        /// Default type. An <see cref="GridObject"/> instance is possibly had bad initialization.
        /// </summary>
        GridObject,
        /// <summary>
        /// An <see cref="GridObject"/> instance is <see cref="OozeDrop"/>.
        /// </summary>
        OozeDrop,
        /// <summary>
        /// An <see cref="GridObject"/> instance is <see cref="Portal"/>.
        /// </summary>
        Portal,
        /// <summary>
        /// An <see cref="GridObject"/> instance is <see cref="OozeBomb"/>.
        /// </summary>
        Bomb,
        /// <summary>
        /// An <see cref="GridObject"/> instance is <see cref="Slime"/>.
        /// </summary>
        Slime,
        /// <summary>
        /// An <see cref="GridObject"/> instance is <see cref="Bonus"/>.
        /// </summary>
        Bonus,
        /// <summary>
        /// An <see cref="GridObject"/> instance is <see cref="SmokeCloud"/>.
        /// </summary>
        SmokeCloud,
    };

    /// <summary>
    /// Defines the determinate state in which an instance of the <see cref="GridObject"/> class can be.
    /// </summary>
    public enum GridObjectStates : byte
    {
        /// <summary>
        /// An GridObject is "turned off". It is not located on map and cannot interact with units.
        /// </summary>
        Off,
        /// <summary>
        /// An GridObject is on the Map and ready to interact with units.
        /// </summary>
        Active,
        /// <summary>
        /// An GridObkect is temporary "turned off" and after the specified time (the activation time) the gridobject will be active again.
        /// </summary>
        Inactive,
    };

    /// <summary>
    /// Specified GridObject different behaviours.
    /// </summary>
    [Flags]
    public enum GridObjectFlags : uint
    {
        /// <summary>
        /// Units can interact with a GridObject (apply <see cref="GridObject.User"/> method).
        /// </summary>
        Usable          = 0x001,
        /// <summary>
        /// Units can interact with a GridObject only once. After that the gridobject is marked as <see cref="ObjectStates.Removed"/>
        /// </summary>
        Disposable      = 0x002,
        /// <summary>
        /// A GridObject applies effect at the nearest units within specified range.
        /// </summary>
        AreaEffect      = 0x004,
        /// <summary>
        /// A GridObject cannot be switched to <see cref="GridObjectStates.Inactive"/> state. This gridobject is unlimited usable.
        /// </summary>
        AlwaysActive    = 0x008,
        /// <summary>
        /// A GridIbject has its lifetime. When time has expired, object is marked as <see cref="ObjectStates.Removed"/>.
        /// </summary>
        Temporal        = 0x010,
    };

    /// <summary>
    /// Contains information about <see cref="Effect"/> that a gridobject applies.
    /// </summary>
    public struct AreaEffect
    {
        /// <summary>
        /// <see cref="EffectEntry.ID"/>
        /// </summary>
        public ushort ID;
        /// <summary>
        /// A gridobject applies effect on unit within this range value.
        /// </summary>
        public int Range;
    }

    /// <summary>
    /// Specifies the base class for static objects on Map that are bound to the specific point. Provides a method for interacting units with these objects.
    /// </summary>
    public abstract class GridObject : Object
    {
        /// <summary>
        /// The GridObject lifetime remains. Uses only with <see cref="GridObjectFlags.Temporal"/>
        /// </summary>
        protected int timeToLive;

        /// <summary>
        /// The time that must elapsed before changing the gridobject state from <see cref="GridObjectStates.Inactive"/> to <see cref="GridObjectStates.Active"/>.
        /// </summary>
        protected int activationTime;

        /// <summary>
        /// Remaining time until the gridobject becomes active.
        /// </summary>
        protected int activationTimer;

        /// <summary>
        /// Indicates that the gridobject is in inactive state because of its recent interaction with an unit.
        /// </summary>
        protected bool recentlyUsed;

        /// <summary>
        /// Contains information about the effect that the gridObject is applying to the nearest units.
        /// </summary>
        protected AreaEffect areaEffect;

        /// <summary>
        /// GridObject flags set.
        /// </summary>
        protected GridObjectFlags gridObjectsFlags;

        /// <summary>
        /// GridObject type.
        /// </summary>
        protected GridObjectTypes gridObjectType;

        /// <summary>
        /// Initialized a new instance of the GridObject class.
        /// </summary>
        public GridObject()
        {
            pr_GridObjectState = GridObjectStates.Active;
            ObjectType = ObjectTypes.GridObject;
            this.gridObjectType = GridObjectTypes.GridObject;

            // Always in center of the cell
            Position = new GPS(Position, 25, 25);

            // Flags by default
            this.gridObjectsFlags = GridObjectFlags.Usable;
        }

        protected GridObjectStates pr_GridObjectState;
        /// <summary>
        /// Gets or sets the gridobject current state
        /// </summary>
        public GridObjectStates GridObjectState
        {
            get
            {
                return this.pr_GridObjectState;
            }
            set
            {
                // May not change state of disabled gridobjects
                if (this.pr_GridObjectState != GridObjectStates.Off)
                    this.pr_GridObjectState = value;
            }
        }

        /// <summary>
        /// Gets the gridObject type.
        /// </summary>
        public GridObjectTypes GridObjectType
        {
            get
            {
                return this.gridObjectType;
            }
        }

        /// <summary>
        /// Gets a value indicating that the gridobject is in <see cref="GridObjectStates.Active"/> state.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return GridObjectState == GridObjectStates.Active;
            }
        }

        /// <summary>
        /// Inidicating whether the gridobject has the specified flags.
        /// </summary>
        /// <param name="flags">Set of the flags to check. Multiple flags should be write as 'Flag1 | Flag2 | Flag3 ...'.</param>
        /// <returns><c>true</c> if the gridobject has all the checked flags; otherwise, <c>false</c>.</returns>
        public bool HasFlags(GridObjectFlags flags)
        {
            return (flags & this.gridObjectsFlags) == flags;
        }

        public override void UpdateState(int timeP)
        {
            // Update life timer for Temporal GO
            if (HasFlags(GridObjectFlags.Temporal))
            {
                if (timeToLive < timeP)
                    ObjectState = ObjectStates.Removed;
                else
                    timeToLive -= timeP;
            }

            // Update activation timer for inactive GO
            if (this.recentlyUsed && !HasFlags(GridObjectFlags.Disposable))
            {
                if (activationTimer < timeP)
                    GridObjectState = GridObjectStates.Active;
                else
                    activationTimer -= timeP;
            }

            // AreaEffect GO
            // Apply effect on the nearest units
            if (HasFlags(GridObjectFlags.AreaEffect) && areaEffect.ID != 0)
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
            if (!HasFlags(GridObjectFlags.AlwaysActive))
            {
                GridObjectState = GridObjectStates.Inactive;
                this.recentlyUsed = true;
                activationTimer = activationTime;
            }

        }
    }
}
