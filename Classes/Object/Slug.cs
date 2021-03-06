﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents the unit that is controlled by a player.
    /// </summary>
    public class Slug : Unit
    {
        /// <summary>
        /// Max value of energy the Slug may have.
        /// </summary>
        public const int MaxOozeEnergy = 100;

        private int downTime; // stand still time
        private int travelTime; // in motion time
        private bool isInMotion;

        /// <summary>
        /// Total game score of this Slug
        /// </summary>
        private int score;

        /// <summary>
        /// Count of Drops that Player has collected at current level.
        /// </summary>
        private int collectedDropsCount;

        /// <summary>
        /// Initialized a new instance of the Slug class.
        /// </summary>
        public Slug()
        {
            Name = "Noname";
            ObjectType = ObjectTypes.Slug;
            this.unitType = UnitTypes.Slug;
            this.unitSide = UnitSides.Good;

            // Home Poisition will be initialized after the Level loading.

            this.respawnTimer = 3000;

            OozeEnergy = MaxOozeEnergy;

            objectSize.Width = GlobalConstants.PLAYER_SIZE_WIDTH;
            objectSize.Height = GlobalConstants.PLAYER_SIZE_HEIGHT;

            BaseSpeed = 0.7d;

            this.motionMaster = new ManualMovement(this);
        }

        private String pr_Name;
        /// <summary>
        /// Gets or Sets Player's name
        /// </summary>
        public String Name
        {
            get { return pr_Name; }
            set { pr_Name = value; }
        }

        /// <summary>
        /// Gets total game score of this Slug.
        /// </summary>
        public int Score { get { return this.score; } }

        /// <summary>
        /// Gets Count of Drops that this Slug has collected at current Level.
        /// </summary>
        public int CollectedDropsCount { get { return this.collectedDropsCount; } }

        private int pr_oozeEnergy;
        /// <summary>
        /// Gets or Sets Slug's Energy supply.
        /// </summary>
        public int OozeEnergy
        {
            get { return pr_oozeEnergy; }
            set
            {
                if (value > MaxOozeEnergy)
                    pr_oozeEnergy = MaxOozeEnergy;
                else if (value <= 0)
                {
                    pr_oozeEnergy = 0;
                    // Cast Deslimation when energy ends
                    CastEffect(4, this);
                }
                else
                    pr_oozeEnergy = value;
            }
        }

        /// <summary>
        /// Registers a Slug in <see cref="ObjectContainer"/>. (Overrides <see cref="Object.Create"/>.)
        /// </summary>
        public override void Create()
        {
            base.Create();

            // Bind Player's effect events to Game Form
            // Needed to display auras at AuraBar
            effectList.EffectApplied += new EffectCollection.EffectHandler(World.PlayForm.OnEffectApplied);
            effectList.EffectRemoved += new EffectCollection.EffectHandler(World.PlayForm.OnEffectRemoved);
        }

        public override void UpdateState(int timeP)
        {
            // Slug is moving:
            // take 2 OozeEnergy every second
            if (isInMotion && GetEffectsByType(EffectTypes.Replenishment).Count == 0) // or id under Replenishment effect
            {
                travelTime += timeP;
                if (travelTime > 1000)    // 1 second of motion
                {
                    travelTime -= 1000;
                    OozeEnergy -= 2;
                }

                isInMotion = false;
            }
            // Slug is standing still:
            // give 1 (3 at Start Point) OozeEnergy every second
            else
            {
                downTime += timeP;
                if (downTime > 1000)    // not in motion over 1 seconds
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
            if (!IsAlive)
                return;

            if (HasEffectType(EffectTypes.Root))
                return;

            GPS previousPosition = Position;

            // Find a point in currectDirection + searchingStep
            // to determine whether Slug is affected by Slime speed-up boost
            GPS searchingPoint = Position;
            int searchingStep = 10;
            List<GridObject> slimeAround;
            bool slimePersist = false;

            switch (this.motionMaster.CurrentDirection.First)
            {
                case Directions.Right:
                    searchingPoint.X = Position.X + searchingStep;
                    if (searchingPoint.X > GlobalConstants.CELL_WIDTH)
                    {
                        searchingPoint.X -= GlobalConstants.CELL_WIDTH;
                        ++searchingPoint.Location.X;
                    }
                    break;
                case Directions.Left:
                    searchingPoint.X = Position.X - searchingStep;
                    if (searchingPoint.X < 0)
                    {
                        searchingPoint.X -= GlobalConstants.CELL_WIDTH;
                        --searchingPoint.Location.X;
                    }
                    break;
                case Directions.Up:
                    searchingPoint.Y = Position.Y - searchingStep;
                    if (searchingPoint.Y < 0)
                    {
                        searchingPoint.Y += GlobalConstants.CELL_HEIGHT;
                        --searchingPoint.Location.Y;
                    }
                    break;
                case Directions.Down:
                    searchingPoint.Y = Position.Y + searchingStep;
                    if (searchingPoint.Y > GlobalConstants.CELL_HEIGHT)
                    {
                        searchingPoint.Y -= GlobalConstants.CELL_HEIGHT;
                        ++searchingPoint.Location.Y;
                    }
                    break;
            }
            // try to find any Slime around that point
            slimeAround = ObjectSearcher.GetGridObjectsInArea(searchingPoint, searchingStep);
            foreach (GridObject slime in slimeAround)
            {
                if (slime.GridObjectType == GridObjectTypes.Slime)
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
                Slime slime = new Slime();
                slime.Create(previousPosition);
            }
            else
            {
                this.isInMotion = false;
            }
        }

        public void LevelChanged()
        {
            this.respawnLocation = Map.Instance.StartLocation;
            Position = new GPS(Home, 25, 25);
        }

        /// <summary>
        /// Handles process of picking up a <see cref="OozeDrop"/> object.
        /// </summary>
        /// <param name="drop"></param>
        public void CollectDrop(OozeDrop drop)
        {
            AddPoints(10);
            OozeEnergy += 10;
            Map.Instance.CollectDrop(drop);
            ++collectedDropsCount;
        }

        /// <summary>
        /// Adds points to Total <see cref="Slug.Score"/>
        /// </summary>
        /// <param name="points"></param>
        public void AddPoints(int points) { this.score += points; }

        /// <summary>
        /// Handles process of picking up a hidden bonus on map.
        /// </summary>
        /// <param name="effectID"><see cref="EffectEntry.ID"/> of the picking effect.</param>
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

        /// <summary>
        /// Create an exact copy of the Slug at the same position and moving in the same direction as the original.
        /// </summary>
        public void CreateClone()
        {
            SlugClone clone = new SlugClone();
            clone.Create(Position, this.motionMaster.CurrentDirection);
        }

        protected override void UnitCollisionEnds(Unit unit)
        {
            if (this.HasEffectType(EffectTypes.Shield))
            {
                EffectHolder holder = this.effectList.GetHolder(11);
                if (holder != null)
                    RemoveEffect(holder);
            }
        }

        protected override void UnitCollision(Unit unit)
        {
            // Do not kill with shield effect
            if (!this.HasEffectType(EffectTypes.Shield))
                unit.KillUnit(this);
        }

    }
}
