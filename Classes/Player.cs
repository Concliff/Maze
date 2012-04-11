using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Forms;

namespace Maze.Classes
{
    public class Player
    {
        private String Name;
        public GridGPS Position;    // Location on current Block
        private bool FinishReached;

        public Player()
        {
            Name = "Noname";

            Position.Location = World.GetWorldMap().GetStartPoint();
            Position.X = 25;
            Position.Y = 25;
            Position.BlockID = 0;

            FinishReached = false;
        }

        public Player(String name) : this()
        {
            Name = name;

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
            Copy.Location.Map = Position.Location.Map;
            Copy.X = Position.X;
            Copy.Y = Position.Y;
            Copy.BlockID = Position.BlockID;
            return Copy;
        }

        public GridGPS MovementAction(byte MoveType)
        {
            GridGPS NewPosition = new GridGPS();
            NewPosition = Position;
            GridMap CurrentGrid = (GridMap)World.GetWorldMap().GetGridMapByGPS(Position.Location);

            NewPosition.X = NewPosition.X
                + (BinaryOperations.GetBit(MoveType, (byte)Directions.Right) -  BinaryOperations.GetBit(MoveType, (byte)Directions.Left))
                * GlobalConstants.MOVEMENT_STEP_PX;

            if (NewPosition.X > (GlobalConstants.GRIDMAP_BLOCK_WIDTH - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_WIDTH /2))
            {
                if (BinaryOperations.IsBit(CurrentGrid.Type, (byte)Directions.Right))
                {
                    if (NewPosition.X > GlobalConstants.GRIDMAP_BLOCK_WIDTH)
                    {
                        NewPosition.X = NewPosition.X - GlobalConstants.GRIDMAP_BLOCK_WIDTH;
                        ++NewPosition.Location.X;
                    }
                }
                else
                    NewPosition.X = GlobalConstants.GRIDMAP_BLOCK_WIDTH - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_WIDTH / 2;
            }
            else if (NewPosition.X < (GlobalConstants.GRIDMAP_BORDER_PX + GlobalConstants.PLAYER_SIZE_WIDTH / 2))
            {
                if (BinaryOperations.IsBit(CurrentGrid.Type, (byte)Directions.Left))
                {
                    if (NewPosition.X < 0)
                    {
                        NewPosition.X = GlobalConstants.GRIDMAP_BLOCK_WIDTH + NewPosition.X;
                        --NewPosition.Location.X;
                    }
                }
                else
                    NewPosition.X = GlobalConstants.GRIDMAP_BORDER_PX + GlobalConstants.PLAYER_SIZE_WIDTH / 2;
            }

            NewPosition.Y = NewPosition.Y
                + (BinaryOperations.GetBit(MoveType, (byte)Directions.Down) - BinaryOperations.GetBit(MoveType, (byte)Directions.Up))
                * GlobalConstants.MOVEMENT_STEP_PX;
            if (NewPosition.Y > (GlobalConstants.GRIDMAP_BLOCK_HEIGHT - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_HEIGHT / 2))
            {
                if (BinaryOperations.IsBit(CurrentGrid.Type, (byte)Directions.Down))
                {
                    if (NewPosition.Y > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                    {
                        NewPosition.Y = NewPosition.Y - GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                        ++NewPosition.Location.Y;
                    }
                }
                else
                    NewPosition.Y = GlobalConstants.GRIDMAP_BLOCK_HEIGHT - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_HEIGHT / 2;
            }
            else if (NewPosition.Y < (GlobalConstants.GRIDMAP_BORDER_PX + GlobalConstants.PLAYER_SIZE_HEIGHT / 2))
            {
                if (BinaryOperations.IsBit(CurrentGrid.Type, (byte)Directions.Up))
                {
                    if (NewPosition.Y < 0)
                    {
                        NewPosition.Y = GlobalConstants.GRIDMAP_BLOCK_HEIGHT + NewPosition.Y;
                        --NewPosition.Location.Y;
                    }
                }
                else
                    NewPosition.Y = GlobalConstants.GRIDMAP_BORDER_PX + GlobalConstants.PLAYER_SIZE_HEIGHT / 2;
            }

            Position = NewPosition;

            if ((Position.X <= GlobalConstants.GRIDMAP_BLOCK_WIDTH - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_WIDTH / 2) &&
                (Position.X >= GlobalConstants.GRIDMAP_BORDER_PX + GlobalConstants.PLAYER_SIZE_WIDTH / 2) &&
                (Position.Y <= GlobalConstants.GRIDMAP_BLOCK_HEIGHT - GlobalConstants.GRIDMAP_BORDER_PX - GlobalConstants.PLAYER_SIZE_HEIGHT / 2) &&
                (Position.Y >= GlobalConstants.GRIDMAP_BORDER_PX + GlobalConstants.PLAYER_SIZE_HEIGHT / 2))
                ReachedGridMap(World.GetWorldMap().GetGridMapByGPS(Position.Location));
            return Position;
        }
        public void TeleportTo() { }

        private void ReachedGridMap(GridMap Block)
        {
            if (BinaryOperations.IsBit(Block.Attribute, (byte)Attributes.IsFinish))
                FinishReached = true;
        }

        public bool IsFinished() { return FinishReached; }
 


    }
}
