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
            // Already started
            if (isInMotion)
                return;

            isInMotion = true;
            
            // find the first allowed direction
            if (currentDirection.First == Directions.None)
                SelectNewDirection();

            // Selecting with random might be failed
            // Recheck the availability of all four directions
            if (currentDirection.First == Directions.None)
            {
                if (currentGridMap.CanMoveTo(Directions.Up))
                {
                    currentDirection.First = Directions.Up;
                    return;
                }
                else if (currentGridMap.CanMoveTo(Directions.Left))
                {
                    currentDirection.First = Directions.Left;
                    return;
                }
                else if (currentGridMap.CanMoveTo(Directions.Down))
                {
                    currentDirection.First = Directions.Down;
                    return;
                }
                else if (currentGridMap.CanMoveTo(Directions.Right))
                {
                    currentDirection.First = Directions.Right;
                    return;
                }
            }

        }

        public void StopMotion() { isInMotion = false; }

        public override void SetDeathState(DeathStates deathState)
        {
            if (deathState == DeathStates.Dead)
                StopMotion();
            else
                StartMotion();

            base.SetDeathState(deathState);
        }

        public void MovementAction()
        {
            if (currentDirection.First == Directions.None)
                return;

            if (HasEffectType(EffectTypes.Root))
                return;

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * this.speedRate;
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            MoveToDirection(movementStep, currentDirection);
        }

        protected override void ReachedGridMap()
        {
            base.ReachedGridMap();

            Position.X = 25;
            Position.Y = 25;

            if (Random.Int(100) <= 33)  // 33% chance to change direction
                SelectNewDirection();

            if (!currentGridMap.CanMoveTo(currentDirection.First))
                SelectNewDirection();

            // Deimos can not pass through the Start Point
            // Check the next GridMap whether it is such block
            GPS nextGPS = Position.Location;
            GridMap nextGridMap;

            switch (currentDirection.First)
            {
                case Directions.Up: --nextGPS.Y; break;
                case Directions.Down: ++nextGPS.Y; break;
                case Directions.Left: --nextGPS.X; break;
                case Directions.Right: ++nextGPS.X; break;
            }

            nextGridMap = GetWorldMap().GetGridMap(nextGPS);
            if (nextGridMap.HasAttribute(GridMapAttributes.IsStart))
                SelectNewDirection(false);
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
                if (!includeCurrent && newDirection == currentDirection.First)
                    continue;

                // Ignore Opposite Direction if there is another one
                if (currentGridMap.CanMoveTo(newDirection) &&
                    (newDirection != GetOppositeDirection(currentDirection.First) || currentDirection.First == Directions.None))
                {
                    currentDirection.First = newDirection;
                    return;
                }
            }

            // Go opposite Direction if no choice to go
            if (currentGridMap.CanMoveTo(GetOppositeDirection(currentDirection.First)))
                currentDirection.First = GetOppositeDirection(currentDirection.First);
            else
                currentDirection.First = Directions.None;
        }
    }
}
