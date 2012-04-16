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

            int movementStep = (int)(GlobalConstants.MOVEMENT_STEP_PX * SpeedRate);

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
            if ((Position.X <= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 + movementStep / 2) &&
                (Position.X >= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 - movementStep / 2) &&
                (Position.Y <= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 + movementStep / 2) &&
                (Position.Y >= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 - movementStep / 2))
                ReachedGridMap();
        }

        private void ReachedGridMap()
        {
            Random RandomInt = new Random();
            if (RandomInt.Next(100) <= 20) // 20% chance to change direction
                SelectNewDirection();

            if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)CurrentDirection))
                return;
            else
                SelectNewDirection();
        }

        private void SelectNewDirection()
        {
            Directions NewDirection = Directions.None;

            int MaxIterations = 10;
            Random RandomInt = new Random();

            for (int i = 0; i < MaxIterations; ++i)
            {
                switch (RandomInt.Next(4) + 1)
                {
                    case 1: NewDirection = Directions.Right; break;
                    case 2: NewDirection = Directions.Down; break;
                    case 3: NewDirection = Directions.Left; break;
                    case 4: NewDirection = Directions.Up; break;
                }

                // Ignore Opposite Direction if there is another one
                if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)NewDirection) &&
                    NewDirection != GetOppositeDirection(CurrentDirection))
                {
                    CurrentDirection = NewDirection;
                    return;
                }
            }

            // Go opposite Direction if no choice to go
            if (BinaryOperations.IsBit(CurrentGridMap.Type, (byte)GetOppositeDirection(CurrentDirection)))
                CurrentDirection = GetOppositeDirection(CurrentDirection);
            else
                CurrentDirection = Directions.None;
        }

        private Directions GetOppositeDirection(Directions Direction)
        {
            switch (Direction)
            {
                case Directions.Left: return Directions.Right;
                case Directions.Right: return Directions.Left;
                case Directions.Down: return Directions.Up;
                case Directions.Up: return Directions.Down;
                default: return Directions.None;
            }
        }


    }
}
