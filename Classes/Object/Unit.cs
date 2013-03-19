using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Maze.Forms;

namespace Maze.Classes
{
    public enum UnitTypes
    {
        Unit,
        Deimos,
        Phobos,
        Slug,
        SlugClone,
    };

    public enum DeathStates
    {
        Alive,
        Dead,
    };

    public enum UnitFlags
    {
        None            = 0,
        CanNotBeKilled  = 0x001,
    };

    public class EffectEventArgs : EventArgs
    {
        public EffectHolder holder;

        public EffectEventArgs(EffectHolder holder)
        {
            this.holder = holder;
        }
    }

    public class EffectCollection
    {
        private List<EffectHolder> effectList;
        private Unit owner;

        public delegate void EffectHandler(object sender, EffectEventArgs e);
        public event EffectHandler EffectApplyEvent;
        public event EffectHandler EffectRemoveEvent;

        public int Count
        {
            get
            {
                return effectList.Count;
            }
        }

        public EffectHolder this[int index]
        {
            get
            {
                return effectList[index];
            }
        }

        public EffectCollection(Unit owner)
        {
            this.owner = owner;
            effectList = new List<EffectHolder>();
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

            if (EffectApplyEvent != null)
            {
                EffectEventArgs e = new EffectEventArgs(holder);
                EffectApplyEvent(owner, e);
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
                if (EffectRemoveEvent != null)
                {
                    EffectEventArgs e = new EffectEventArgs(holder);
                    EffectRemoveEvent(owner, e);
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

    public class Unit : Object
    {
        protected DeathStates deathState;
        protected double baseSpeed; // base speed of the Unit
        public double BaseSpeed
        {
            get
            {
                return baseSpeed;
            }
            protected set
            {
                baseSpeed = value;
                CalculateSpeedRate();
            }
        }

        protected double speedRate; // Current speed(+effects)
        public double SpeedRate
        {
            get
            {
                return speedRate;
            }
            protected set
            {
                speedRate = value;
            }
        }

        protected GridLocation respawnLocation;
        /// <summary>
        /// Gets Respawn Location of the unit.
        /// </summary>
        public GridLocation Home
        {
            get
            {
                return this.respawnLocation;
            }
            protected set
            {
                this.respawnLocation = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a Unit is located at its respawn Point
        /// </summary>
        public bool IsAtHome
        {
            get
            {
                return currentCell.Location.Equals(Home);
            }
        }

        protected int respawnTimer;
        protected UnitTypes pr_unitType;
        protected int unitFlags;
        protected EffectCollection effectList;

        protected MovementGenerator motionMaster;

        /// <summary>
        /// Determines the type of the unit
        /// </summary>
        public UnitTypes UnitType
        {
            get { return pr_unitType; }
            set { pr_unitType = value; }
        }

        public Unit()
        {
            SetDeathState(DeathStates.Alive);
            SetUnitFlags(UnitFlags.None);

            this.respawnLocation = new GridLocation();

            objectType = ObjectType.Unit;
            UnitType = UnitTypes.Unit;

            effectList = new EffectCollection(this);

            BaseSpeed = 1.0d;
            SpeedRate = BaseSpeed;

            respawnTimer = 3000;

            effectList.EffectApplyEvent += new EffectCollection.EffectHandler(OnEffectApplied);

            effectList.EffectRemoveEvent += new EffectCollection.EffectHandler(OnEffectRemoved);
        }

        public void OnEffectApplied(object sender, EffectEventArgs e)
        {
            EffectHolder holder = e.holder;

            // Update unit stats
            if (holder.EffectInfo.EffectType == EffectTypes.Snare ||
                holder.EffectInfo.EffectType == EffectTypes.IncreaseSpeed)
                CalculateSpeedRate();
        }

        public void OnEffectRemoved(object sender, EffectEventArgs e)
        {
            EffectHolder holder = e.holder;

            // Update unit stats
            if (holder.EffectInfo.EffectType == EffectTypes.Snare ||
                holder.EffectInfo.EffectType == EffectTypes.IncreaseSpeed)
                CalculateSpeedRate();
        }

        //public UnitTypes GetUnitType() { return pr_unitType; }

        /// <summary>
        /// Sets all specified flags if unit doesnt have it
        /// </summary>
        /// <param name="unitFlags"></param>
        public void SetUnitFlags(params UnitFlags[] unitFlags)
        {
            for (int i = 0; i < unitFlags.Count(); ++i)
            {
                if (!HasUnitFlag(unitFlags[i]))
                    this.unitFlags += (int)unitFlags[i];
            }
        }

        public bool HasUnitFlag(UnitFlags unitFlag)
        {
            return ((uint)unitFlag & (uint)this.unitFlags) != 0;
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
                        Cell interimPoint = GetWorldMap().GetCell(intermidiatePosition.Location);
                        if (interimPoint.ID != -1)
                            newPosition = intermidiatePosition;

                        distance -= GlobalConstants.CELL_HEIGHT;
                    }
                    break;
                case Directions.Up:
                    while (distance >= GlobalConstants.CELL_HEIGHT)
                    {
                        intermidiatePosition.Location.Y--;
                        Cell interimPoint = GetWorldMap().GetCell(intermidiatePosition.Location);
                        if (interimPoint.ID != -1)
                            newPosition = intermidiatePosition;

                        distance -= GlobalConstants.CELL_HEIGHT;
                    }
                    break;
                case Directions.Left:
                    while (distance >= GlobalConstants.CELL_HEIGHT)
                    {
                        intermidiatePosition.Location.X--;
                        Cell interimPoint = GetWorldMap().GetCell(intermidiatePosition.Location);
                        if (interimPoint.ID != -1)
                            newPosition = intermidiatePosition;

                        distance -= GlobalConstants.CELL_HEIGHT;
                    }
                    break;
                case Directions.Right:
                    while (distance >= GlobalConstants.CELL_HEIGHT)
                    {
                        intermidiatePosition.Location.X++;
                        Cell interimPoint = GetWorldMap().GetCell(intermidiatePosition.Location);
                        if (interimPoint.ID != -1)
                            newPosition = intermidiatePosition;

                        distance -= GlobalConstants.CELL_HEIGHT;
                    }
                    break;
            }

            Position = newPosition;
        }

        public void TeleportTo(Cell destinationCell)
        {
            Position = new GPS(destinationCell.Location, 25, 25);
        }

        public void TeleportTo(GridLocation destinationGPS)
        {
            Cell destinationCell = GetWorldMap().GetCell(destinationGPS);
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

        public bool HasEffectType(EffectTypes effectType)
        {
            return GetEffectsByType(effectType).Count > 0;
        }

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

        public bool IsAlive() { return deathState == DeathStates.Alive; }
        public virtual void SetDeathState(DeathStates deathState)
        {
            if (deathState == DeathStates.Dead)
            {
                // Remove All Effects
                effectList.RemoveAll();

            }

            this.deathState = deathState;

        }

        public bool KillUnit(Unit victim)
        {
            if (victim.HasUnitFlag(UnitFlags.CanNotBeKilled))
                return false;

            victim.SetDeathState(DeathStates.Dead);

            // TODO:
            // Kind of reward for the killer

            return true;
        }

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
                if (obj.IsActive() && obj.HasFlag(GridObjectFlags.Usable))
                    obj.Use(this);
            }

            base.UpdateState(timeP);
        }

        public void StartMotion()
        {
            if (this.motionMaster != null)
                this.motionMaster.StartMotion();
        }

        public void StopMotion()
        {
            if (this.motionMaster != null)
                this.motionMaster.StopMotion();
        }

        public bool IsVisible()
        {
            return !HasEffectType(EffectTypes.Invisibility);
        }
    }
}
