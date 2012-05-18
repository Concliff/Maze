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
        protected GPS respawnLocation;
        protected UnitTypes unitType;
        protected List<Effect> effectList;

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
            effectList = new List<Effect>();
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

        public void ApplyEffect(Effect newEffect)
        {
            // checking this effect for existing
            if (effectList.Count > 0)
                foreach (Effect effect in effectList)
                {
                    if (effect.GetType() == newEffect.GetType())
                        return;
                }

            effectList.Add(newEffect);

            // Update unit stats
            if (newEffect.GetType() == EffectTypes.Speed)
                CalculateSpeedRate();
        }

        public Effect GetEffectByType(EffectTypes effectType)
        {
            if (effectList.Count > 0)
                foreach (Effect effect in effectList)
                {
                    if (effect.GetType() == effectType)
                        return effect;
                }
            return null;
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
            Effect speedEffect = GetEffectByType(EffectTypes.Speed);
            if (speedEffect != null)
                speedModifier = speedEffect.Modifier;

            this.speedRate += this.speedRate * speedModifier / 100d;
        }

        public bool IsAlive() { return deathState == DeathStates.Alive; }
        public virtual void SetDeathState(DeathStates deathState)
        {
            this.deathState = deathState;
        }

        public override void UpdateState(int timeP)
        {
            // Update unit effects
            for (int i = 0; i < effectList.Count;)
            {
                if (effectList[i].GetState() == EffectState.Expired)
                {
                    // Save Type of removable effect
                    EffectTypes effectType = effectList[i].GetType();

                    effectList.Remove(effectList[i]);

                    // Update Speed
                    if (effectType == EffectTypes.Speed)
                        CalculateSpeedRate();

                    continue;
                }

                effectList[i].UpdateTime(timeP);
                ++i;
            }

            base.UpdateState(timeP);
        }

        virtual public void StartMotion() { }
    }
}
