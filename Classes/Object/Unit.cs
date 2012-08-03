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

    public class Unit : Object
    {
        protected bool gridMapReached;
        protected DeathStates deathState;
        protected double baseSpeed; // base speed of the Unit
        protected double speedRate; // Current speed(+effects)
        protected double stepRemainder; // Due to conversion from double(speedRate) to int(Coords)
        protected GPS respawnLocation;
        protected int respawnTimer;
        protected UnitTypes unitType;
        protected List<EffectHolder> effectList;

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
            baseSpeed = 1.0d;
            speedRate = baseSpeed;
            stepRemainder = 0;
            effectList = new List<EffectHolder>();

            currentDirection.First = Directions.None;
            currentDirection.Second = Directions.None;

            respawnTimer = 3000;
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
            // prevent applying double effect
            if (this.effectList.Count > 0)
                foreach (EffectHolder effect in effectList)
                {
                    if (effect.EffectInfo.ID == newHolder.EffectInfo.ID)
                    {
                        effect.Refresh();
                        return;
                    }
                }

            effectList.Add(newHolder);
            if (GetUnitType() == UnitTypes.Slug)
                World.GetPlayForm().OnEffectApplied(newHolder);

            // Update unit stats
            if (newHolder.EffectInfo.EffectType == EffectTypes.Snare ||
                newHolder.EffectInfo.EffectType == EffectTypes.IncreaseSpeed)
                CalculateSpeedRate();
        }

        private void RemoveEffect(EffectHolder effectHolder)
        {
            if (!effectList.Contains(effectHolder))
                return;

            // Save Type of removable effect
            EffectTypes effectType = effectHolder.EffectInfo.EffectType;

            if (effectList.Remove(effectHolder))
            {
                if (GetUnitType() == UnitTypes.Slug)
                    World.GetPlayForm().OnEffectRemoved(effectHolder);

                // Update Speed
                if (effectType == EffectTypes.Snare ||
                    effectType == EffectTypes.IncreaseSpeed)
                    CalculateSpeedRate();
            }
        }

        public List<EffectEntry> GetEffectsByType(EffectTypes effectType)
        {
            List<EffectEntry> result = new List<EffectEntry>();
            foreach (EffectHolder effect in effectList)
            {
                if (effect.EffectInfo.EffectType == effectType)
                    result.Add(effect.EffectInfo);
            }
            return result;
        }


        public double GetSpeedRate() { return speedRate; }
        public void SetBaseSpeed(double speed)
        {
            this.baseSpeed = speed;

            CalculateSpeedRate();
        }
        protected void CalculateSpeedRate()
        {
            this.speedRate = this.baseSpeed;

            double speedModifier = 100;
            List<EffectEntry> speedEffects;

            speedEffects = GetEffectsByType(EffectTypes.Snare);
            foreach (EffectEntry effect in speedEffects)
                speedModifier *= effect.Value / 100d;

            speedEffects = GetEffectsByType(EffectTypes.IncreaseSpeed);
            foreach (EffectEntry effect in speedEffects)
                speedModifier *= effect.Value/100d + 1;

            this.speedRate *= speedModifier / 100d;
        }

        public bool IsAlive() { return deathState == DeathStates.Alive; }
        public virtual void SetDeathState(DeathStates deathState)
        {
            if (deathState == DeathStates.Dead)
            {
                // Remove All Effects
                int count = effectList.Count;
                for (int i = 0; i < count; ++i)
                    RemoveEffect(effectList[0]);

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
            for (int i = 0; i < effectList.Count;)
            {
                if (effectList[i].GetState() == EffectState.Expired)
                {
                    RemoveEffect(effectList[i]);
                    continue;
                }

                effectList[i].UpdateTime(timeP);
                ++i;
            }

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
            return (GetEffectsByType(EffectTypes.Invisibility).Count == 0);
        }
    }
}
