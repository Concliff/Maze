using System;
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
            respawnLocation = GetWorldMap().GetStartPoint();

            // Set Start Location
            Position = new GridGPS(respawnLocation, 25, 25);

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
                    OozeEnergy += IsAtRespawnLocation() ? 3 : 1;
                }
            }

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

        public GridGPS CopyGridGPS()
        {
            GridGPS Copy = new GridGPS();
            // Create Copy of Player's GPS (not refference)
            Copy.Location.X = Position.Location.X;
            Copy.Location.Y = Position.Location.Y;
            Copy.Location.Z = Position.Location.Z;
            Copy.Location.Level = Position.Location.Level;
            Copy.X = Position.X;
            Copy.Y = Position.Y;
            Copy.BlockID = Position.BlockID;
            return Copy;
        }

        /// <summary>
        /// Player Moving Handler
        /// </summary>
        /// <param name="MoveType">Flags of direction</param>
        public GridGPS MovementAction(uint MoveType)
        {
            if (!IsAlive())
                return Position;
            if (HasEffectType(EffectTypes.Root))
                return Position;

            // Define direction of motion
            // TODO: improve the next stupid if..else..if
            if ((MoveType & (uint)Directions.Up) != 0)
                this.currentDirection.First = Directions.Up;
            else if ((MoveType & (uint)Directions.Down) != 0)
                this.currentDirection.First = Directions.Down;
            else if ((MoveType & (uint)Directions.Left) != 0)
                this.currentDirection.First = Directions.Left;
            else if ((MoveType & (uint)Directions.Right) != 0)
                this.currentDirection.First = Directions.Right;
            else
                this.currentDirection.First = Directions.None;

            if (this.currentDirection.First == Directions.None)
                return Position;

            MoveType -= (uint)this.currentDirection.First;

            if ((MoveType & (uint)Directions.Up) != 0)
                this.currentDirection.Second = Directions.Up;
            else if ((MoveType & (uint)Directions.Down) != 0)
                this.currentDirection.Second = Directions.Down;
            else if ((MoveType & (uint)Directions.Left) != 0)
                this.currentDirection.Second = Directions.Left;
            else if ((MoveType & (uint)Directions.Right) != 0)
                this.currentDirection.Second = Directions.Right;
            else
                this.currentDirection.Second = Directions.None;

            if (HasEffectType(EffectTypes.MoveReverse))
            {
                this.currentDirection.First = GetOppositeDirection(this.currentDirection.First);
                this.currentDirection.Second = GetOppositeDirection(this.currentDirection.Second);
            }

            // Find a point in currectDirection + searchingStep
            GridGPS searchingPoint = Position;
            int searchingStep = 10;
            List<GridObject> slimeAround;
            bool slimePersist = false;
            double slimeSpeedRate = SpeedRate;

            switch (currentDirection.First)
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
            // Increase speed by 0.5
            if (slimePersist)
                slimeSpeedRate += 0.5;

            GridGPS previousPosition = Position;

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * slimeSpeedRate;
            if (this.currentDirection.Second != Directions.None)
                movementStepD = Math.Sqrt(2 * movementStepD);
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            MoveToDirection(movementStep, currentDirection);

            // Check movement occurrence
            if (previousPosition != Position)
            {
                this.isInMotion = true;

                //create slime at old position
                new Slime(Position);
            }
            else
            {
                this.isInMotion = false;
            }


            return Position;
        }

        public void LevelChanged()
        {
            respawnLocation = GetWorldMap().GetStartPoint();
            Position = new GridGPS(respawnLocation, 25, 25);
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
            new SlugClone(Position, this.currentDirection);
        }

    }
}
