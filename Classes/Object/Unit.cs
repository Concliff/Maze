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

    public struct Direction
    {
        public Directions First;
        public Directions Second;
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
                if (effectList[i].GetState() == EffectState.Expired)
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
        protected bool gridMapReached;
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

        protected double stepRemainder; // Due to conversion from double(speedRate) to int(Coords)
        protected GPS respawnLocation;
        protected int respawnTimer;
        protected UnitTypes unitType;
        protected EffectCollection effectList;

        protected bool isInMotion;
        protected Direction currentDirection;

        public Unit()
        {
            SetDeathState(DeathStates.Alive);
            gridMapReached = true;

            respawnLocation.Level = respawnLocation.X =
                respawnLocation.Y = respawnLocation.Z = 0;

            objectType = ObjectType.Unit;
            unitType = UnitTypes.Unit;
            BaseSpeed = 1.0d;
            SpeedRate = BaseSpeed;
            stepRemainder = 0;
            effectList = new EffectCollection(this);

            currentDirection.First = Directions.None;
            currentDirection.Second = Directions.None;

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

        public UnitTypes GetUnitType() { return unitType; }

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

            currentGridMap = GetWorldMap().GetGridMap(Position.Location);
            gridMapReached = false;
        }

        public void TeleportTo(GridMap destinationGridMap)
        {
            currentGridMap = destinationGridMap;
            Position.Location = destinationGridMap.Location;
            Position.X = 25;
            Position.Y = 25;
            Position.BlockID = destinationGridMap.ID;

            ReachedGridMap();
        }

        public void TeleportTo(GPS destinationGPS)
        {
            GridMap destinationGridMap = GetWorldMap().GetGridMap(destinationGPS);
            TeleportTo(destinationGridMap);
        }

        protected virtual void ReachedGridMap()
        {
            if (gridMapReached)
                return;

            gridMapReached = true;
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

        private void RemoveEffect(EffectHolder effectHolder)
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
            // TODO:
            // units that can not be killed

            victim.SetDeathState(DeathStates.Dead);

            // TODO:
            // Kind of reward for the killer

            return true;
        }

        protected void Respawn()
        {
            // Return to start location
            Position.Location = respawnLocation;
            Position.X = 25;
            Position.Y = 25;
            currentGridMap = GetWorldMap().GetGridMap(Position.Location);
            Position.BlockID = currentGridMap.ID;

            SetDeathState(DeathStates.Alive);
        }

        protected void MoveToDirection(int movementStep, Direction direction)
        {
            for (int i = 0; i < 2; ++i)
                switch (i == 0 ? direction.First : direction.Second)
                {
                    case Directions.Up:
                        {
                            Position.Y -= movementStep;
                            if (Position.Y < 0)
                            {
                                Position.Y += GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Up);
                            }

                            break;
                        }
                    case Directions.Down:
                        {
                            Position.Y += movementStep;
                            if (Position.Y > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                            {
                                Position.Y -= GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Down);
                            }
                            break;
                        }
                    case Directions.Left:
                        {
                            Position.X -= movementStep;
                            if (Position.X < 0)
                            {
                                Position.X += GlobalConstants.GRIDMAP_BLOCK_WIDTH;
                                ChangeGPSDueDirection(1, Directions.Left);
                            }
                            break;
                        }
                    case Directions.Right:
                        {
                            Position.X += movementStep;
                            if (Position.X > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                            {
                                Position.X -= GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Right);
                            }
                            break;
                        }
                }

            // Check whether a unit moved into the block (account block border)
            if ((Position.X <= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 + movementStep / 2) &&
                 (Position.X >= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 - movementStep / 2) &&
                 (Position.Y <= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 + movementStep / 2) &&
                 (Position.Y >= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 - movementStep / 2) &&
                !this.gridMapReached)
                ReachedGridMap();

            NormalizePosition();
        }

        protected Directions GetOppositeDirection(Directions Direction)
        {
            switch (Direction)
            {
                case Directions.Left: return Directions.Right;
                case Directions.Right: return Directions.Left;
                case Directions.Down: return Directions.Up;
                case Directions.Up: return Directions.Down;
                default: return Directions.None;
            }
        }

        public bool IsAtRespawnLocation()
        {
            return currentGridMap.Location.Equals(respawnLocation);
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

        public virtual void StartMotion() { }

        public bool IsVisible()
        {
            return !HasEffectType(EffectTypes.Invisibility);
        }
    }
}
