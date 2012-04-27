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
            unitType = UnitTypes.Deimos;
            respawnLocation = GetWorldMap().GetFinishPoint();
            Position.Location = respawnLocation;
            Position.X = 25;
            Position.Y = 25;
            Position.BlockID = GetWorldMap().GetGridMap(Position.Location).ID;
            IsInMotion = false;
            CurrentDirection = Directions.None;

            currentGridMap = GetWorldMap().GetGridMap(Position.Location);
        }

        public Deimos(GPS respawnLocation)
            : this()
        {
            this.respawnLocation = respawnLocation;
            Position.Location = respawnLocation;
            currentGridMap = GetWorldMap().GetGridMap(Position.Location);
        }

        public override void UpdateState(int timeP)
        {
            if (IsInMotion)
                MovementAction();
        }

        public override void StartMotion()
        { 
            IsInMotion = true; 
            
            // find the first allowed direction
            if (CurrentDirection == Directions.None)
                SelectNewDirection();
        }

        public void StopMotion() { IsInMotion = false; }

        public void MovementAction()
        {
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

        protected override void ReachedGridMap()
        {
            if (Random.Int(100) <= 33)  // 33% chance to change direction
                SelectNewDirection();

            if (BinaryOperations.IsBit(currentGridMap.Type, (byte)CurrentDirection))
                return;
            else
                SelectNewDirection();

            base.ReachedGridMap();
        }

        private void SelectNewDirection()
        {
            Directions newDirection = Directions.None;
            int maxIterations = 10;

            for (int i = 0; i < maxIterations; ++i)
            {
                switch (Random.Int(4) + 1)
                {
                    case 1: newDirection = Directions.Right; break;
                    case 2: newDirection = Directions.Down; break;
                    case 3: newDirection = Directions.Left; break;
                    case 4: newDirection = Directions.Up; break;
                }
                // Ignore Opposite Direction if there is another one
                if (BinaryOperations.IsBit(currentGridMap.Type, (byte)newDirection) &&
                    (newDirection != GetOppositeDirection(CurrentDirection) || CurrentDirection == Directions.None))
                {
                    CurrentDirection = newDirection;
                    return;
                }
            }

            // Go opposite Direction if no choice to go
            if (BinaryOperations.IsBit(currentGridMap.Type, (byte)GetOppositeDirection(CurrentDirection)))
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
