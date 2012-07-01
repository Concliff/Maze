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
    };

    public enum DeathStates
    {
        Alive,
        Dead,
    };

    public class Unit : Object
    {
        protected bool gridMapReached;
        protected DeathStates deathState;
        protected double baseSpeed; // base speed of the Unit
        protected double speedRate; // Current speed(+effects)
        protected double stepRemainder; // Due to conversion from double(speedRate) to int(Coords)
        protected GPS respawnLocation;
        protected UnitTypes unitType;
        protected List<EffectHolder> effectList;

        protected bool isInMotion;
        protected Directions currentDirection;

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
            // asd
            List<GridObject> objects = GetGridObjectsWithinRange(GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2);

            foreach (GridObject obj in objects)
            {
                if (obj.IsActive() && obj.HasFlag(GridObjectFlags.Usable))
                    obj.Use(this);
            }

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
                        return;
                }

            effectList.Add(newHolder);

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

            int speedModifier = 0;
            List<EffectEntry> speedEffects;

            speedEffects = GetEffectsByType(EffectTypes.Snare);
            foreach (EffectEntry effect in speedEffects)
                speedModifier -= effect.Value;

            this.speedRate += this.speedRate * speedModifier / 100d;

            speedModifier = 0;
            speedEffects = GetEffectsByType(EffectTypes.IncreaseSpeed);
            foreach (EffectEntry effect in speedEffects)
                speedModifier += effect.Value;

            this.speedRate += this.speedRate * speedModifier / 100d;
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

        public bool IsAtRespawnLocation()
        {
            return currentGridMap.Location.Equals(respawnLocation);
        }

        public override void UpdateState(int timeP)
        {
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

            base.UpdateState(timeP);
        }

        public virtual void StartMotion() { }

        public bool IsVisible()
        {
            return (GetEffectsByType(EffectTypes.Invisibility).Count == 0);
        }
    }
}
