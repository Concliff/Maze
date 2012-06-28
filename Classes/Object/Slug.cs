using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Slug : Unit
    {
        private String Name;
        private bool FinishReached;
        private int ressurectTimer;
        private int score;
        private int collectedCoinsCount;    // at current level

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
            respawnLocation = GetWorldMap().GetStartPoint();

            // Set Start Location
            Position.Location = respawnLocation;
            Position.X = 25;
            Position.Y = 25;
            Position.BlockID = GetWorldMap().GetGridMap(Position.Location).ID;

            currentGridMap = GetWorldMap().GetGridMap(Position.Location);

            FinishReached = false;
            ressurectTimer = 3000;

            score = 0;
            collectedCoinsCount = 0;

            oozeEnergy = MaxOozeEnergy;
            downTime = 0;
            travelTime = 0;
            isInMotion = false;

            SetBaseSpeed(0.7d);

        }

        public Slug(String name) : this()
        {
            Name = name;
        }

        public override void UpdateState(int timeP)
        {
            if (!FinishReached && IsAlive() && IsVisible())
            {
                List<Unit> Units = GetUnitsWithinRange(30);
                if (Units != null && Units.Count != 0)
                {
                    SetDeathState(DeathStates.Dead);
                    return;
                }
            }

            if (!IsAlive())
            {
                if (ressurectTimer < timeP)
                    Ressurect();
                else
                    ressurectTimer -= timeP;

                return;
            }

            if (isInMotion)
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
                    OozeEnergy += 1;
                }
            }

            base.UpdateState(timeP);
        }

        public override void SetDeathState(DeathStates deathState)
        {
            if (deathState == DeathStates.Dead)
            {
                ressurectTimer = 3000;
            }

            base.SetDeathState(deathState);
        }

        public void Ressurect()
        {
            // Return to start location
            Position.Location = respawnLocation;
            Position.X = 25;
            Position.Y = 25;
            currentGridMap = GetWorldMap().GetGridMap(Position.Location);
            Position.BlockID = currentGridMap.ID;

            SetDeathState(DeathStates.Alive);
        }

        public String GetName() { return Name; }
        public void SetName(String name) { Name = name; }

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
            if (GetEffectsByType(EffectTypes.Root).Count != 0)
                return Position;

            // Define direction of motion
            if ((MoveType & (uint)Directions.Up) != 0)
                this.currentDirection = Directions.Up;
            else if ((MoveType & (uint)Directions.Down) != 0)
                this.currentDirection = Directions.Down;
            else if ((MoveType & (uint)Directions.Left) != 0)
                this.currentDirection = Directions.Left;
            else if ((MoveType & (uint)Directions.Right) != 0)
                this.currentDirection = Directions.Right;
            else
                this.currentDirection = Directions.None;

            // Find a point in currectDirection + searchingStep
            GridGPS searchingPoint = Position;
            int searchingStep = 10;
            List<GridObject> slimeAround;
            bool slimePersist = false;
            double slimeSpeedRate = GetSpeedRate();

            switch (currentDirection)
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

            //create slime at old position
            new Slime(Position);

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * slimeSpeedRate;
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            // New position coords
            int NewX, NewY;
            NewX = Position.X + (movementStep *
                (((MoveType & (uint)Directions.Right) == 0 ? 0 : 1) - ((MoveType & (uint)Directions.Left) == 0 ? 0 : 1)));

            // New position X and Y should be within allowed range
            // If not - prevent movement or change location
            if (NewX > (GlobalConstants.GRIDMAP_BLOCK_WIDTH - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_WIDTH / 2))
            {
                if (currentGridMap.CanMoveTo(Directions.Right))
                {
                    if (NewX > GlobalConstants.GRIDMAP_BLOCK_WIDTH)
                    {
                        Position.X = NewX - GlobalConstants.GRIDMAP_BLOCK_WIDTH;
                        ChangeGPSDueDirection(1, Directions.Right);
                    }
                    else
                        Position.X = NewX;
                }
                else
                    Position.X = GlobalConstants.GRIDMAP_BLOCK_WIDTH - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_WIDTH / 2;
            }
            else if (NewX < (GlobalConstants.GRIDMAP_BORDER_PX + GlobalConstants.PLAYER_SIZE_WIDTH / 2))
            {
                if (currentGridMap.CanMoveTo(Directions.Left))
                {
                    if (NewX < 0)
                    {
                        Position.X = GlobalConstants.GRIDMAP_BLOCK_WIDTH + NewX;
                        ChangeGPSDueDirection(1, Directions.Left);
                    }
                    else
                        Position.X = NewX;
                }
                else
                    Position.X = GlobalConstants.GRIDMAP_BORDER_PX + GlobalConstants.PLAYER_SIZE_WIDTH / 2;
            }
            else
                Position.X = NewX;

            NewY = Position.Y + (movementStep *
                (((MoveType & (uint)Directions.Down) == 0 ? 0 : 1) - ((MoveType & (uint)Directions.Up) == 0 ? 0 : 1)));
            if (NewY > (GlobalConstants.GRIDMAP_BLOCK_HEIGHT - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_HEIGHT / 2))
            {
                if (currentGridMap.CanMoveTo(Directions.Down))
                {
                    if (NewY > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                    {
                        Position.Y = NewY - GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                        ChangeGPSDueDirection(1, Directions.Down);
                    }
                    else
                        Position.Y = NewY;
                }
                else
                    Position.Y = GlobalConstants.GRIDMAP_BLOCK_HEIGHT - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_HEIGHT / 2;
            }
            else if (NewY < (GlobalConstants.GRIDMAP_BORDER_PX + GlobalConstants.PLAYER_SIZE_HEIGHT / 2))
            {
                if (currentGridMap.CanMoveTo(Directions.Up))
                {
                    if (NewY < 0)
                    {
                        Position.Y = GlobalConstants.GRIDMAP_BLOCK_HEIGHT + NewY;
                        ChangeGPSDueDirection(1, Directions.Up);
                    }
                    else
                        Position.Y = NewY;
                }
                else
                    Position.Y = GlobalConstants.GRIDMAP_BORDER_PX + GlobalConstants.PLAYER_SIZE_HEIGHT / 2;
            }
            else
                Position.Y = NewY;

            isInMotion = true;

            // Check whether player moved into block (account block border)
            if ((Position.X <= GlobalConstants.GRIDMAP_BLOCK_WIDTH - GlobalConstants.GRIDMAP_BORDER_PX) &&
                (Position.X >= GlobalConstants.GRIDMAP_BORDER_PX) &&
                (Position.Y <= GlobalConstants.GRIDMAP_BLOCK_HEIGHT - GlobalConstants.GRIDMAP_BORDER_PX) &&
                (Position.Y >= GlobalConstants.GRIDMAP_BORDER_PX))
                ReachedGridMap();
            return Position;
        }

        public void LevelChanged()
        {
            respawnLocation = GetWorldMap().GetStartPoint();
            Position.Location = respawnLocation;
            Position.X = 25;
            Position.Y = 25;
            currentGridMap = GetWorldMap().GetGridMap(Position.Location);
            FinishReached = false;
        }

        /// <summary>
        /// Called when player moved to the next block
        /// </summary>
        protected override void ReachedGridMap()
        {
            base.ReachedGridMap();

            if (currentGridMap.HasAttribute(GridMapAttributes.IsFinish) &&
                GetWorldMap().CoinsRemain() == 0)
                FinishReached = true;
        }

        public void CollectCoin(Coin coin)
        {
            AddPoints(10);
            GetWorldMap().CollectCoin(coin);
            ++collectedCoinsCount;
        }

        public bool IsFinished() { return FinishReached; }

        public int GetScore() { return score; }
        public void AddPoints(int points) { this.score += points; }

        public int GetCollectedCoinsCount() { return this.collectedCoinsCount; }
    }
}
