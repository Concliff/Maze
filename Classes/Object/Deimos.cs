using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    class Deimos : Unit
    {

        public Deimos()
        {
            unitType = UnitTypes.Deimos;
            respawnLocation = GetWorldMap().GetFinishPoint();
            Position.Location = respawnLocation;
            Position.X = 25;
            Position.Y = 25;
            Position.BlockID = GetWorldMap().GetGridMap(Position.Location).ID;
            isInMotion = false;
            currentDirection = Directions.None;

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
            if (isInMotion)
                MovementAction();

            base.UpdateState(timeP);
        }

        public override void StartMotion()
        {
            isInMotion = true;
            
            // find the first allowed direction
            if (currentDirection == Directions.None)
                SelectNewDirection();

            // Selecting with random might be failed
            // Recheck the availability of all four directions
            if (currentDirection == Directions.None)
            {
                if (currentGridMap.CanMoveTo(Directions.Up))
                {
                    currentDirection = Directions.Up;
                    return;
                }
                else if (currentGridMap.CanMoveTo(Directions.Left))
                {
                    currentDirection = Directions.Left;
                    return;
                }
                else if (currentGridMap.CanMoveTo(Directions.Down))
                {
                    currentDirection = Directions.Down;
                    return;
                }
                else if (currentGridMap.CanMoveTo(Directions.Right))
                {
                    currentDirection = Directions.Right;
                    return;
                }
            }

        }

        public void StopMotion() { isInMotion = false; }

        public void MovementAction()
        {
            if (currentDirection == Directions.None)
                return;

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * this.speedRate;
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            switch (currentDirection)
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
            base.ReachedGridMap();

            if (Random.Int(100) <= 33)  // 33% chance to change direction
                SelectNewDirection();

            if (currentGridMap.CanMoveTo(currentDirection))
            {
                GPS nextGPS = Position.Location;
                GridMap nextGridMap;

                switch (currentDirection)
                {
                    case Directions.Up: --nextGPS.Y; break;
                    case Directions.Down: ++nextGPS.Y; break;
                    case Directions.Left: --nextGPS.X; break;
                    case Directions.Right: ++nextGPS.X; break;
                }

                nextGridMap = GetWorldMap().GetGridMap(nextGPS);
                if (nextGridMap.HasAttribute(GridMapAttributes.IsStart))
                {
                    SelectNewDirection(false);
                }

                return;
            }
            else
            {
                SelectNewDirection();
            }
        }

        private void SelectNewDirection()
        {
            SelectNewDirection(true);
        }

        private void SelectNewDirection(bool includeCurrent)
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
                if (!includeCurrent && newDirection == currentDirection)
                    continue;

                // Ignore Opposite Direction if there is another one
                if (currentGridMap.CanMoveTo(newDirection) &&
                    (newDirection != GetOppositeDirection(currentDirection) || currentDirection == Directions.None))
                {
                    currentDirection = newDirection;
                    return;
                }
            }

            // Go opposite Direction if no choice to go
            if (currentGridMap.CanMoveTo(GetOppositeDirection(currentDirection)))
                currentDirection = GetOppositeDirection(currentDirection);
            else
                currentDirection = Directions.None;
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
