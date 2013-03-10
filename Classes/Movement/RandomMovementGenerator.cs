using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class RandomMovementGenerator : MovementGenerator
    {
        public RandomMovementGenerator(Unit unit)
            : base(unit)
        {
            this.generatorType = MovementGeneratorType.Random;
        }

        public override void UpdateState(int timeP)
        {
            if (!IsInMotion || this.unit.HasEffectType(EffectTypes.Root))
                return;

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * this.unit.SpeedRate;
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            MoveToDirection(movementStep, CurrentDirection);
        }

        private void SelectNewDirection()
        {
            SelectNewDirection(true);
        }

        private void SelectNewDirection(bool includeCurrent)
        {
            Directions newDirection = Directions.None;
            int maxIterations = 10;
            GridMap currentGridMap = WorldMap.GetGridMap(this.unit.Position.Location);

            for (int i = 0; i < maxIterations; ++i)
            {
                switch (Random.Int(4) + 1)
                {
                    case 1: newDirection = Directions.Right; break;
                    case 2: newDirection = Directions.Down; break;
                    case 3: newDirection = Directions.Left; break;
                    case 4: newDirection = Directions.Up; break;
                }
                if (!includeCurrent && newDirection == CurrentDirection.First)
                    continue;

                // Ignore Opposite Direction if there is another one
                if (currentGridMap.CanMoveTo(newDirection) &&
                    (newDirection != GetOppositeDirection(CurrentDirection.First) || CurrentDirection.First == Directions.None))
                {
                    CurrentDirection = new Direction(newDirection);
                    return;
                }
            }

            // Go opposite Direction if no choice to go
            if (currentGridMap.CanMoveTo(GetOppositeDirection(CurrentDirection.First)))
                CurrentDirection = new Direction(GetOppositeDirection(CurrentDirection.First));
            else
                CurrentDirection = new Direction(Directions.None);

            // Selecting with random might be failed
            // Recheck the availability of all four directions
            if (CurrentDirection.First == Directions.None)
            {
                if (currentGridMap.CanMoveTo(Directions.Up))
                {
                    CurrentDirection = new Direction(Directions.Up);
                    return;
                }
                else if (currentGridMap.CanMoveTo(Directions.Left))
                {
                    CurrentDirection = new Direction(Directions.Left);
                    return;
                }
                else if (currentGridMap.CanMoveTo(Directions.Down))
                {
                    CurrentDirection = new Direction(Directions.Down);
                    return;
                }
                else if (currentGridMap.CanMoveTo(Directions.Right))
                {
                    CurrentDirection = new Direction(Directions.Right);
                    return;
                }
            }
        }

        protected override void OnPositionChanged(object sender, PositionEventArgs e)
        {
            // HACK: Ignore if not the center of the block
            int movementStep = (int)(GlobalConstants.MOVEMENT_STEP_PX * this.unit.SpeedRate) + 1;
            if (!(unit.Position.X >= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 - movementStep / 2 &&
                unit.Position.X <= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 + movementStep / 2 &&
                unit.Position.Y >= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 - movementStep / 2 &&
                unit.Position.Y <= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 + movementStep / 2 &&
                !this.gridMapReached))
                return;

            this.gridMapReached = true;

            //unit.Position = new GridGPS(unit.Position, 25, 25);

            if (Random.Int(100) <= 33)  // 33% chance to change direction
                SelectNewDirection();

            if (!WorldMap.GetGridMap(this.unit.Position.Location).CanMoveTo(CurrentDirection.First))
                SelectNewDirection();

            // Deimos can not pass through the Start Point
            // Check the next GridMap whether it is such block
            GPS nextGPS = unit.Position.Location;
            GridMap nextGridMap;

            switch (CurrentDirection.First)
            {
                case Directions.Up: --nextGPS.Y; break;
                case Directions.Down: ++nextGPS.Y; break;
                case Directions.Left: --nextGPS.X; break;
                case Directions.Right: ++nextGPS.X; break;
            }

            nextGridMap = WorldMap.GetGridMap(nextGPS);
            if (nextGridMap.HasAttribute(GridMapAttributes.IsStart))
                SelectNewDirection(false);
        }
    }
}
