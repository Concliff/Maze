using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Maze.Forms;

namespace Maze.Classes
{
    /// <summary>
    /// Specifies the type of object (or derived class) that an instance of the <see cref="Unit"/> class represents.
    /// </summary>
    public enum UnitTypes
    {
        /// <summary>
        /// Default type. An <see cref="Unit"/> instance is not initialized and cannot belong to the specific type.
        /// </summary>
        Unit,
        /// <summary>
        /// An <see cref="Unit"/> instance is derived from <see cref="Deimos"/> class.
        /// </summary>
        Deimos,
        /// <summary>
        /// An <see cref="Unit"/> instance is derived from <see cref="Phobos"/> class.
        /// </summary>
        Phobos,
        /// <summary>
        /// An <see cref="Unit"/> instance is derived from <see cref="Slug"/> class.
        /// </summary>
        Slug,
        /// <summary>
        /// An <see cref="Unit"/> instance is derived from <see cref="SlugClone"/> class.
        /// </summary>
        SlugClone,
    };

    /// <summary>
    /// Defines the Unit state showing possibility to interact with other objects or environment.
    /// </summary>
    public enum DeathStates
    {
        /// <summary>
        /// Unit is existing on map and can move, cast spells etc.
        /// </summary>
        Alive,
        /// <summary>
        /// Unit is waiting for respawn timer expired to become <see cref="DeathStates.Alive"/>.
        /// </summary>
        Dead,
    };

    /// <summary>
    /// Specifies unit special features.
    /// </summary>
    [Flags]
    public enum UnitFlags
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None            = 0x000,
        /// <summary>
        /// The <see cref="Unit"/> cannot be in the <see cref="DeathStatus.Dead"/> state.
        /// </summary>
        CanNotBeKilled  = 0x001,
    };

    /// <summary>
    /// Provides data for the <see cref="EffectHandler.EffectApplied"/> and <see cref="EffectHandler.EffectRemoved"/> events.
    /// </summary>
    public class EffectEventArgs : EventArgs
    {
        private EffectHolder pr_Holder;
        /// <summary>
        /// Gets an <see cref="EffectHolder"/> instance that raises the event.
        /// </summary>
        public EffectHolder Holder { get { return this.pr_Holder; } }

        /// <summary>
        /// Initializes a new instance of the EffectEventArgs class.
        /// </summary>
        /// <param name="holder">EffectHolder that raises the event.</param>
        public EffectEventArgs(EffectHolder holder)
        {
            this.pr_Holder = holder;
        }
    }

    /// <summary>
    /// Represents custom collection class that stores all the <see cref="EffectHolder"/> instances applied to the specific <see cref="Unit"/> instance.
    /// </summary>
    public class EffectCollection
    {
        /// <summary>
        /// Represents the method that will handle the <see cref="EffectApplied"/> or <see cref="EffectRemoved"/> event.
        /// </summary>
        /// <param name="sender">The sourse of the event.</param>
        /// <param name="e">A <see cref="EffectEventArgs"/> that contains the event data.</param>
        public delegate void EffectHandler(object sender, EffectEventArgs e);
        /// <summary>
        /// Occurs after a holder has been added from <see cref="EffectCollection"/>.
        /// </summary>
        public event EffectHandler EffectApplied;
        /// <summary>
        /// Occurs after a holder has been deleted from <see cref="EffectCollection"/>.
        /// </summary>
        public event EffectHandler EffectRemoved;

        private List<EffectHolder> effectList;
        private Unit owner;

        /// <summary>
        /// Initializes a new instance of the EffectCollection class.
        /// </summary>
        /// <param name="owner">The <see cref="Unit"/> instance whom all these effect will belong to.</param>
        public EffectCollection(Unit owner)
        {
            this.owner = owner;
            effectList = new List<EffectHolder>();
        }

        /// <summary>
        /// Gets a number of effects contained in <see cref="EffectCollection"/>.
        /// </summary>
        public int Count
        {
            get
            {
                return effectList.Count;
            }
        }

        /// <summary>
        /// Gets an effect holder at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The <see cref="EffectHolder"/> instance at the specific index.</returns>
        public EffectHolder this[int index]
        {
            get
            {
                return effectList[index];
            }
        }

        public bool Add(EffectHolder holder)
        {
            // prevent applying double effect
            if (effectList.Count > 0)
                foreach (EffectHolder effect in effectList)
                {
                    if (effect.EffectInfo.ID == holder.EffectInfo.ID)
                    {
                        effect.Refresh();
                        return true;
                    }
                }

            effectList.Add(holder);

            if (EffectApplied != null)
            {
                EffectEventArgs e = new EffectEventArgs(holder);
                EffectApplied(owner, e);
            }

            return true;
        }

        public EffectHolder GetHolder(int effectId)
        {
            return this.effectList.Find(p => p.EffectInfo.ID == effectId);
        }

        public bool Remove(EffectHolder holder)
        {

            if (!effectList.Contains(holder))
                return false;

            // Save Type of removable effect
            EffectTypes effectType = holder.EffectInfo.EffectType;

            if (effectList.Remove(holder))
            {
                if (EffectRemoved != null)
                {
                    EffectEventArgs e = new EffectEventArgs(holder);
                    EffectRemoved(owner, e);
                }

            }
            return true;
        }

        public void RemoveAll()
        {
            while(effectList.Count > 0)
            {
                Remove(effectList[0]);
            }
        }

        public void Update(int timeP)
        {
            for (int i = 0; i < effectList.Count; )
            {
                if (effectList[i].EffectState == EffectState.Expired)
                {
                    Remove(effectList[i]);
                    continue;
                }

                effectList[i].UpdateTime(timeP);
                ++i;
            }
        }

    }

    /// <summary>
    /// Represents a base class for dynamic objects that can move through the map.
    /// </summary>
    public abstract class Unit : Object
    {
        /// <summary>
        /// Occurs when object changed its Position without any movement processes.
        /// </summary>
        public event PositionHandler Relocated;

        /// <summary>
        /// Gets or sets respawn Location of the Unit.
        /// </summary>
        protected GridLocation respawnLocation;

        /// <summary>
        /// The current unit state.
        /// </summary>
        protected DeathStates deathState;

        protected int respawnTime;

        /// <summary>
        /// Shows after which time the unit will respawn (will change its deathState from <see cref="DeathState.Dead"/> to <see cref="DeathState.Alive"/>).
        /// </summary>
        protected int respawnTimer;

        /// <summary>
        /// Contains the flags set.
        /// </summary>
        protected UnitFlags unitFlags;

        /// <summary>
        /// Collection of effect holders that the unit has at this moment.
        /// </summary>
        protected EffectCollection effectList;

        /// <summary>
        /// The current movement engine of the unit.
        /// </summary>
        protected MovementGenerator motionMaster;

        /// <summary>
        /// Type of the unit. Specifies the unit's derived class.
        /// </summary>
        protected UnitTypes unitType;

        /// <summary>
        /// Initializes a new instance of the Unit class.
        /// </summary>
        public Unit()
        {
            SetDeathState(DeathStates.Alive);
            this.unitFlags = UnitFlags.None;

            this.respawnLocation = new GridLocation();

            ObjectType = ObjectTypes.Unit;
            this.unitType = UnitTypes.Unit;

            this.effectList = new EffectCollection(this);

            BaseSpeed = 1.0d;
            SpeedRate = BaseSpeed;

            this.respawnTimer = 3000;

            this.effectList.EffectApplied += new EffectCollection.EffectHandler(OnEffectApplied);
            this.effectList.EffectRemoved += new EffectCollection.EffectHandler(OnEffectRemoved);
        }

        protected double pr_BaseSpeed;
        /// <summary>
        /// Gets or sets the Unit base speed value. Base Unit Speed is the speed of specific Unit type, without any effects or smth else.
        /// </summary>
        public double BaseSpeed
        {
            get
            {
                return this.pr_BaseSpeed;
            }
            protected set
            {
                this.pr_BaseSpeed = value;
                CalculateSpeedRate();
            }
        }

        protected double pt_SpeedRate; // Current speed(+effects)
        /// <summary>
        /// Gets or sets Unit current speed considering all movement effect at it.
        /// </summary>
        public double SpeedRate
        {
            get
            {
                return this.pt_SpeedRate;
            }
            protected set
            {
                this.pt_SpeedRate = value;
            }
        }

        /// <summary>
        /// Gets respawn location of the unit.
        /// </summary>
        public GridLocation Home
        {
            get
            {
                return this.respawnLocation;
            }
        }

        /// <summary>
        /// Gets a value indicating that the unit is in the <see cref="DeathStates.Alive"/> state.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return deathState == DeathStates.Alive;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a Unit is located at its respawn <see cref="Cell"/>.
        /// </summary>
        public bool IsAtHome
        {
            get
            {
                return currentCell.Location.Equals(Home);
            }
        }

        /// <summary>
        /// Gets a value indicating that the unit is displayed on map and other units can interact with it.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return !HasEffectType(EffectTypes.Invisibility);
            }
        }

        /// <summary>
        /// Gets the unit type.
        /// </summary>
        public UnitTypes UnitType
        {
            get { return this.unitType; }
        }

        /// <summary>
        /// Registers this unit in <see cref="ObjectContainer"/> and sets its Home location.
        /// </summary>
        /// <param name="respawnLocation">Home location of the unit.</param>
        public void Create(GridLocation respawnLocation)
        {
            this.respawnLocation = respawnLocation;
            base.Create(new GPS(respawnLocation));
        }

        public void OnEffectApplied(object sender, EffectEventArgs e)
        {
            EffectHolder holder = e.Holder;

            // Update unit stats
            if (holder.EffectInfo.EffectType == EffectTypes.Snare ||
                holder.EffectInfo.EffectType == EffectTypes.IncreaseSpeed)
                CalculateSpeedRate();
        }

        public void OnEffectRemoved(object sender, EffectEventArgs e)
        {
            EffectHolder holder = e.Holder;

            // Update unit stats
            if (holder.EffectInfo.EffectType == EffectTypes.Snare ||
                holder.EffectInfo.EffectType == EffectTypes.IncreaseSpeed)
                CalculateSpeedRate();
        }

        /// <summary>
        /// Inidicating whether the unit has the specified flags.
        /// </summary>
        /// <param name="flags">Set of the flags to check. Multiple flags should be write as 'Flag1 | Flag2 | Flag3 ...'.</param>
        /// <returns><c>true</c> if the unit has all the checked flags; otherwise, <c>false</c>.</returns>
        public bool HasUnitFlags(UnitFlags flags)
        {
            return (flags & this.unitFlags) == flags;
        }

        /// <summary>
        /// Moves(Teleport) Unit on current direction
        /// </summary>
        /// <param name="distance">Distance in pixels</param>
        public void JumpThroughDistance(int distance)
        {
            if (this.motionMaster == null)
                return;
            // TODO:
            // 1. Add diagonal checking
            // 2. Improve current method

            bool isDiagonal = false;
            if (this.motionMaster.CurrentDirection.Second != Directions.None)
            {
                isDiagonal = true;
                distance = (int)Math.Sqrt(distance);
            }

            GPS newPosition = Position;
            GPS intermidiatePosition = Position;

            switch (this.motionMaster.CurrentDirection.First)
            {
                case Directions.Down:
                    while (distance >= GlobalConstants.CELL_HEIGHT)
                    {
                        intermidiatePosition.Location.Y++;
                        Cell interimPoint = Map.Instance.GetCell(intermidiatePosition.Location);
                        if (interimPoint.ID != -1)
                            newPosition = intermidiatePosition;

                        distance -= GlobalConstants.CELL_HEIGHT;
                    }
                    break;
                case Directions.Up:
                    while (distance >= GlobalConstants.CELL_HEIGHT)
                    {
                        intermidiatePosition.Location.Y--;
                        Cell interimPoint = Map.Instance.GetCell(intermidiatePosition.Location);
                        if (interimPoint.ID != -1)
                            newPosition = intermidiatePosition;

                        distance -= GlobalConstants.CELL_HEIGHT;
                    }
                    break;
                case Directions.Left:
                    while (distance >= GlobalConstants.CELL_HEIGHT)
                    {
                        intermidiatePosition.Location.X--;
                        Cell interimPoint = Map.Instance.GetCell(intermidiatePosition.Location);
                        if (interimPoint.ID != -1)
                            newPosition = intermidiatePosition;

                        distance -= GlobalConstants.CELL_HEIGHT;
                    }
                    break;
                case Directions.Right:
                    while (distance >= GlobalConstants.CELL_HEIGHT)
                    {
                        intermidiatePosition.Location.X++;
                        Cell interimPoint = Map.Instance.GetCell(intermidiatePosition.Location);
                        if (interimPoint.ID != -1)
                            newPosition = intermidiatePosition;

                        distance -= GlobalConstants.CELL_HEIGHT;
                    }
                    break;
            }

            // save previous position
            GPS prevPosition = Position;

            Position = newPosition;

            if (Relocated != null)
                Relocated(this, new PositionEventArgs(prevPosition, Position));
        }

        /// <summary>
        /// Relocates the unit to the specifed destination <see cref="Cell"/>.
        /// </summary>
        /// <param name="destinationCell">A cell where the unit is relocating</param>
        public void TeleportTo(Cell destinationCell)
        {
            // save old position
            GPS prevPosition = Position;
            Position = new GPS(destinationCell.Location, 25, 25);

            if (Relocated != null)
                Relocated(this, new PositionEventArgs(prevPosition, Position));

        }

        /// <summary>
        /// Relocates the unit to the secified destination position.
        /// </summary>
        /// <param name="destinationGPS">A new position where the unit is relocating.</param>
        public void TeleportTo(GridLocation destinationGPS)
        {
            Cell destinationCell = Map.Instance.GetCell(destinationGPS);
            TeleportTo(destinationCell);
        }

        public void CastEffect(ushort effectID, Unit target)
        {
            EffectEntry effectEntry = DBStores.EffectStore[effectID];

            Effect effect = new Effect(effectEntry, target, this);
            effect.Cast();
        }

        public void ApplyEffect(EffectHolder newHolder)
        {
            effectList.Add(newHolder);
        }

        protected void RemoveEffect(EffectHolder effectHolder)
        {
            effectList.Remove(effectHolder);
        }

        /// <summary>
        /// Determines whether the unit has applied aura with the specified effect type.
        /// </summary>
        /// <param name="effectType">One of the <see cref="EffectTypes"/> value.</param>
        /// <returns><c>true</c> if the unit has at least one aura with the specified type; otherwise, <c>false</c>.</returns>
        public bool HasEffectType(EffectTypes effectType)
        {
            return GetEffectsByType(effectType).Count > 0;
        }

        /// <summary>
        /// Returns a collection of effects with the specifed type.
        /// </summary>
        /// <param name="effectType"></param>
        /// <returns></returns>
        public List<EffectEntry> GetEffectsByType(EffectTypes effectType)
        {
            List<EffectEntry> result = new List<EffectEntry>();
            /*foreach (EffectHolder effect in effectList)
            {
                if (effect.EffectInfo.EffectType == effectType)
                    result.Add(effect.EffectInfo);
            }*/
            for (int i = 0; i < effectList.Count; ++i)
            {
                if (effectList[i].EffectInfo.EffectType == effectType)
                    result.Add(effectList[i].EffectInfo);
            }
            return result;
        }

        /// <summary>
        /// Computes the actual speed rate considering all the influencing effects.
        /// </summary>
        protected void CalculateSpeedRate()
        {
            SpeedRate = BaseSpeed;

            double speedModifier = 100;
            List<EffectEntry> speedEffects;

            speedEffects = GetEffectsByType(EffectTypes.Snare);
            foreach (EffectEntry effect in speedEffects)
                speedModifier *= effect.Value / 100d;

            speedEffects = GetEffectsByType(EffectTypes.IncreaseSpeed);
            foreach (EffectEntry effect in speedEffects)
                speedModifier *= effect.Value/100d + 1;

            SpeedRate *= speedModifier / 100d;
        }

        public virtual void SetDeathState(DeathStates deathState)
        {
            if (deathState == DeathStates.Dead)
            {
                // Remove All Effects
                effectList.RemoveAll();

            }

            this.deathState = deathState;

        }

        /// <summary>
        /// The unit changes the death state of another unit by force, getting a reward for it.
        /// </summary>
        /// <param name="victim">The target unit</param>
        /// <returns><c>true</c>, if a victim bacame dead; otherwise, <c>false</c>.</returns>
        public bool KillUnit(Unit victim)
        {
            if (victim.HasUnitFlags(UnitFlags.CanNotBeKilled))
                return false;

            victim.SetDeathState(DeathStates.Dead);

            // TODO:
            // Kind of reward for the killer

            return true;
        }

        /// <summary>
        /// Returns the unit to its Home location and changes the state to <see cref="DeathState.Alive"/>.
        /// </summary>
        protected void Respawn()
        {
            // Return to start location
            Position = new GPS(Home, 25, 25);

            SetDeathState(DeathStates.Alive);
        }

        public override void UpdateState(int timeP)
        {
            if (deathState == DeathStates.Dead)
            {
                if (respawnTimer < 0)
                {
                    Respawn();
                    respawnTimer = 3000;
                }
                else
                    respawnTimer -= timeP;
            }

            // Update unit effects
            effectList.Update(timeP);

            // Check for the nearest GridObjects
            List<GridObject> objects = GetGridObjectsWithinRange(30);

            foreach (GridObject obj in objects)
            {
                if (obj.IsActive && obj.HasFlags(GridObjectFlags.Usable))
                    obj.Use(this);
            }

            base.UpdateState(timeP);
        }

        /// <summary>
        /// Starts the motion of a unit's movement generator.
        /// </summary>
        public void StartMotion()
        {
            if (this.motionMaster != null)
                this.motionMaster.StartMotion();
        }

        /// <summary>
        /// Stops the motion of a unit's movement generator.
        /// </summary>
        public void StopMotion()
        {
            if (this.motionMaster != null)
                this.motionMaster.StopMotion();
        }
    }
}
