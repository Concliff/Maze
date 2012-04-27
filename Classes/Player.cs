using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Player : Unit
    {
        private String Name;
        private bool FinishReached;
        private int ressurectTimer;
        private int score;

        public Player()
        {
            Name = "Noname";
            UnitType = UnitTypes.Player;
            respawnLocation = GetWorldMap().GetStartPoint();

            // Set Start Location
            Position.Location = respawnLocation;
            Position.X = 25;
            Position.Y = 25;
            Position.BlockID = GetWorldMap().GetGridMap(Position.Location).ID;

            CurrentGridMap = GetWorldMap().GetGridMap(Position.Location);

            FinishReached = false;
            ressurectTimer = 3000;

            score = 0;
        }

        public Player(String name) : this()
        {
            Name = name;
        }

        public override void UpdateState(int timeP)
        {
            if (!FinishReached && IsAlive())
            {
                List<Unit> Units = GetUnitsWithinRange(30);
                if (Units != null && Units.Count != 0)
                    SetDeathState(DeathStates.Dead);
            }

            if (!IsAlive())
            {
                if (ressurectTimer < timeP)
                    RessurectPlayer();
                else
                    ressurectTimer -= timeP;
            }
        }

        public override void SetDeathState(DeathStates deathState)
        {
            if (deathState == DeathStates.Dead)
            {
                ressurectTimer = 3000;
            }

            base.SetDeathState(deathState);
        }

        public void RessurectPlayer()
        {
            // Return to start location
            Position.Location = respawnLocation;
            Position.X = 25;
            Position.Y = 25;
            CurrentGridMap = GetWorldMap().GetGridMap(Position.Location);
            Position.BlockID = CurrentGridMap.ID;

            SetDeathState(DeathStates.Alive);
        }

        public String GetName() { return Name; }
        public void SetName(String PlayerName) { Name = PlayerName; }

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
        public GridGPS MovementAction(byte MoveType)
        {
            if (!IsAlive())
                return Position;
            // New position coords
            int NewX, NewY;
            NewX = Position.X
                + (BinaryOperations.GetBit(MoveType, (byte)Directions.Right) -  BinaryOperations.GetBit(MoveType, (byte)Directions.Left))
                * GlobalConstants.MOVEMENT_STEP_PX;

            // New position X and Y should be within allowed range
            // If not - prevent movement or change location
            if (NewX > (GlobalConstants.GRIDMAP_BLOCK_WIDTH - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_WIDTH / 2))
            {
                if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)Directions.Right))
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
                if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)Directions.Left))
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

            NewY = Position.Y
                + (BinaryOperations.GetBit(MoveType, (byte)Directions.Down) - BinaryOperations.GetBit(MoveType, (byte)Directions.Up))
                * GlobalConstants.MOVEMENT_STEP_PX;
            if (NewY > (GlobalConstants.GRIDMAP_BLOCK_HEIGHT - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_HEIGHT / 2))
            {
                if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)Directions.Down))
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
                if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)Directions.Up))
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
            CurrentGridMap = GetWorldMap().GetGridMap(Position.Location);
            FinishReached = false;
        }


        public void TeleportTo() { }

        /// <summary>
        /// Called when player moved to the next block
        /// </summary>
        private void ReachedGridMap()
        {
            if (BinaryOperations.IsBit(CurrentGridMap.Attribute, (byte)Attributes.IsFinish) &&
                GetWorldMap().GetCoinsCount() == GetWorldMap().GetCollectedCoinsCount())
                FinishReached = true;

            if (BinaryOperations.IsBit(CurrentGridMap.Attribute, (byte)Attributes.HasCoin) &&
                !GetWorldMap().IsCoinCollected(CurrentGridMap))
            {
                GetWorldMap().CollectCoin(CurrentGridMap);

                AddPoints(10);
            }
        }

        public bool IsFinished() { return FinishReached; }

        public int GetScore() { return score; }
        public void AddPoints(int points) { this.score += points; }
    }
}
