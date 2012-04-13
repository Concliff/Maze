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
        private GridMap CurrentGridMap;
        private bool FinishReached;

        public Player()
        {
            Name = "Noname";

            Position.Location = World.GetWorldMap().GetStartPoint();
            Position.X = 25;
            Position.Y = 25;
            Position.BlockID = 0;

            CurrentGridMap = World.GetWorldMap().GetGridMapByGPS(Position.Location);

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
            int NewX, NewY;

            NewX = Position.X
                + (BinaryOperations.GetBit(MoveType, (byte)Directions.Right) -  BinaryOperations.GetBit(MoveType, (byte)Directions.Left))
                * GlobalConstants.MOVEMENT_STEP_PX;

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

            if ((Position.X <= GlobalConstants.GRIDMAP_BLOCK_WIDTH - GlobalConstants.GRIDMAP_BORDER_PX) &&
                (Position.X >= GlobalConstants.GRIDMAP_BORDER_PX) &&
                (Position.Y <= GlobalConstants.GRIDMAP_BLOCK_HEIGHT - GlobalConstants.GRIDMAP_BORDER_PX) &&
                (Position.Y >= GlobalConstants.GRIDMAP_BORDER_PX))
                ReachedGridMap(World.GetWorldMap().GetGridMapByGPS(Position.Location));
            return Position;
        }
        public void TeleportTo() { }

        private void ReachedGridMap(GridMap Block)
        {
            if (BinaryOperations.IsBit(Block.Attribute, (byte)Attributes.IsFinish) &&
                World.GetWorldMap().GetCoinsCount() == World.GetWorldMap().GetCollectedCoinsCount())
                FinishReached = true;

            if (BinaryOperations.IsBit(Block.Attribute, (byte)Attributes.HasCoin) &&
                !World.GetWorldMap().IsCoinCollected(CurrentGridMap))
                World.GetWorldMap().CollectCoin(CurrentGridMap);
        }

        private void ChangeGPSDueDirection(int BlockPassCount, Directions Direction)
        {
            switch (Direction)
            {
                case Directions.Up: Position.Location.Y -= BlockPassCount; break;
                case Directions.Down: Position.Location.Y += BlockPassCount; break;
                case Directions.Left: Position.Location.X -= BlockPassCount; break;
                case Directions.Right: Position.Location.X += BlockPassCount; break;
            }

            CurrentGridMap = World.GetWorldMap().GetGridMapByGPS(Position.Location);
        }

        public bool IsFinished() { return FinishReached; }
 


    }
}
