﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Slug : Unit
    {
        private String pr_Name;
        private int score;
        private int collectedDropsCount;    // at current level

        /// <summary>
        /// Returns player's name
        /// </summary>
        public String Name
        {
            get { return pr_Name; }
            set { pr_Name = value; }
        }

        // ooze
        public const int MaxOozeEnergy = 100;
        public int OozeEnergy
        {
            get { return oozeEnergy;}
            set
            {
                if (value > MaxOozeEnergy)
                    oozeEnergy = MaxOozeEnergy;
                else if (value <= 0)
                {
                    oozeEnergy = 0;
                    // Cast Deslimation when energy ends
                    CastEffect(4, this);
                }
                else
                    oozeEnergy = value;
            }
        }

        private int oozeEnergy;
        private int downTime;
        private int travelTime;

        public Slug()
        {
            Name = "Noname";
            objectType = ObjectType.Slug;
            UnitType = UnitTypes.Slug;
            Home = GetWorldMap().GetStartPoint();

            // Set Start Location
            Position = new GridGPS(Home, 25, 25);

            respawnTimer = 3000;

            score = 0;
            collectedDropsCount = 0;

            oozeEnergy = MaxOozeEnergy;
            downTime = 0;
            travelTime = 0;
            isInMotion = false;

            objectSize.Width = GlobalConstants.PLAYER_SIZE_WIDTH;
            objectSize.Height = GlobalConstants.PLAYER_SIZE_HEIGHT;

            BaseSpeed = 0.7d;

            this.motionMaster = new ManualMovement(this);

        }

        public void HookEvents()
        {
            effectList.EffectApplyEvent += new EffectCollection.EffectHandler(World.PlayForm.OnEffectApplied);

            effectList.EffectRemoveEvent += new EffectCollection.EffectHandler(World.PlayForm.OnEffectRemoved);
        }

        public Slug(String name) : this()
        {
            Name = name;
        }

        public override void UpdateState(int timeP)
        {
            if (IsAlive() && IsVisible())
            {
                List<Unit> Units = GetUnitsWithinRange(30);
                foreach (Unit unit in Units)
                    if (unit.GetType() != ObjectType.Slug)
                    {
                        unit.KillUnit(this);
                        return;
                    }

            }

            if (isInMotion && GetEffectsByType(EffectTypes.Replenishment).Count == 0)
            {
                travelTime += timeP;
                if (travelTime > 1000)    // 1 second of motion
                {
                    travelTime -= 1000;
                    OozeEnergy -= 2;
                }

                isInMotion = false;
            }
            else
            {
                downTime += timeP;
                if (downTime > 1000)    // not in motion over 5 seconds
                {
                    downTime -= 1000;
                    OozeEnergy += IsAtHome ? 3 : 1;
                }
            }

            MovementAction(timeP);

            base.UpdateState(timeP);
        }

        public override void SetDeathState(DeathStates deathState)
        {
            if (deathState == DeathStates.Dead)
            {
                respawnTimer = 3000;
            }

            base.SetDeathState(deathState);
        }

        /// <summary>
        /// Player Moving Handler
        /// </summary>
        /// <param name="MoveType">Flags of direction</param>
        private void MovementAction(int timeP)
        {
            if (!IsAlive())
                return;

            if (HasEffectType(EffectTypes.Root))
                return;

            GridGPS previousPosition = Position;

            // Find a point in currectDirection + searchingStep
            GridGPS searchingPoint = Position;
            int searchingStep = 10;
            List<GridObject> slimeAround;
            bool slimePersist = false;

            switch (this.motionMaster.CurrentDirection.First)
            {
                case Directions.Right:
                    searchingPoint.X = Position.X + searchingStep;
                    if (searchingPoint.X > GlobalConstants.GRIDMAP_BLOCK_WIDTH)
                    {
                        searchingPoint.X -= GlobalConstants.GRIDMAP_BLOCK_WIDTH;
                        ++searchingPoint.Location.X;
                    }
                    break;
                case Directions.Left:
                    searchingPoint.X = Position.X - searchingStep;
                    if (searchingPoint.X < 0)
                    {
                        searchingPoint.X -= GlobalConstants.GRIDMAP_BLOCK_WIDTH;
                        --searchingPoint.Location.X;
                    }
                    break;
                case Directions.Up:
                    searchingPoint.Y = Position.Y - searchingStep;
                    if (searchingPoint.Y < 0)
                    {
                        searchingPoint.Y += GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                        --searchingPoint.Location.Y;
                    }
                    break;
                case Directions.Down:
                    searchingPoint.Y = Position.Y + searchingStep;
                    if (searchingPoint.Y > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                    {
                        searchingPoint.Y -= GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                        ++searchingPoint.Location.Y;
                    }
                    break;
            }

            slimeAround = ObjectSearcher.GetGridObjectsInArea(searchingPoint, searchingStep);
            foreach (GridObject slime in slimeAround)
            {
                if (slime.GetGridObjectType() == GridObjectType.Slime)
                    slimePersist = true;
            }
            // Increase speed with an Effect "Viscous Slime - Slug"
            if (slimePersist)
                CastEffect(15, this);

            this.motionMaster.UpdateState(timeP);

            // Check movement occurrence
            if (previousPosition != Position)
            {
                this.isInMotion = true;

                //create slime at old position
                new Slime(previousPosition);
            }
            else
            {
                this.isInMotion = false;
            }
        }

        public void LevelChanged()
        {
            Home = GetWorldMap().GetStartPoint();
            Position = new GridGPS(Home, 25, 25);
        }

        public void CollectDrop(OozeDrop drop)
        {
            AddPoints(10);
            OozeEnergy += 10;
            GetWorldMap().CollectDrop(drop);
            ++collectedDropsCount;
        }

        public int GetScore() { return score; }
        public void AddPoints(int points) { this.score += points; }

        public int GetCollectedDropsCount() { return this.collectedDropsCount; }

        public void CollectHiddenBonus(ushort effectID)
        {
            EffectEntry effectEntry = DBStores.EffectStore[effectID];
            if(effectEntry.HasAttribute(EffectAttributes.CanBeSpell))
            {
                World.PlayForm.AddSpell(effectEntry);
            }
            else
            {
                Effect effect = new Effect(effectEntry, this, this);
                effect.Cast();
            }
        }

        public void CreateClone()
        {
            new SlugClone(Position, this.motionMaster.CurrentDirection);
        }

    }
}
