using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    class Deimos : Unit
    {
        private Directions CurrentDirection;
        private bool IsInMotion;

        public Deimos()
        {
            UnitType = UnitTypes.Deimos;
            Position.Location = World.GetWorldMap().GetFinishPoint();
            Position.X = 25;
            Position.Y = 25;
            Position.BlockID = 0;
            IsInMotion = false;
            CurrentDirection = Directions.None;

            CurrentGridMap = World.GetWorldMap().GetGridMapByGPS(Position.Location);
        }

        public void StartMotion()
        { 
            IsInMotion = true; 
            
            // find the first allowed direction
            if (CurrentDirection == Directions.None)
                SelectNewDirection();
        }

        public void StopMotion() { IsInMotion = false; }

        public void MovementAction()
        {
            if (!IsInMotion)
                return;
            if (CurrentDirection == Directions.None)
                return;

            int movementStep = (int)(GlobalConstants.MOVEMENT_STEP_PX * 1.0);

            switch (CurrentDirection)
            {
                case Directions.Up:
                    {
                        Position.X = 25;
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
                        Position.X = 25;
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
                        Position.Y = 25;
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
                        Position.Y = 25;
                        Position.X += movementStep;
                        if (Position.X > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                        {
                            Position.X -= GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                            ChangeGPSDueDirection(1, Directions.Right);
                        }
                        break;
                    }
            }
            if ((Position.X <= GlobalConstants.GRIDMAP_BLOCK_WIDTH - GlobalConstants.GRIDMAP_BORDER_PX) &&
                (Position.X >= GlobalConstants.GRIDMAP_BORDER_PX) &&
                (Position.Y <= GlobalConstants.GRIDMAP_BLOCK_HEIGHT - GlobalConstants.GRIDMAP_BORDER_PX) &&
                (Position.Y >= GlobalConstants.GRIDMAP_BORDER_PX))
                ReachedGridMap();
        }

        private void ReachedGridMap()
        {
            if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)CurrentDirection))
                return;
            else
                SelectNewDirection();
        }

        private void SelectNewDirection()
        {
            if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)Directions.Left))
                CurrentDirection = Directions.Left;
            else if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)Directions.Right))
                CurrentDirection = Directions.Right;
            else if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)Directions.Up))
                CurrentDirection = Directions.Up;
            else if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)Directions.Down))
                CurrentDirection = Directions.Down;
            else
                IsInMotion = false;
        }


    }
}
