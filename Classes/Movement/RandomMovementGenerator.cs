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
            SelectNewDirection();
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

            Move(movementStep);
        }

        private void SelectNewDirection()
        {
            SelectNewDirection(true);
        }

        private void SelectNewDirection(bool includeCurrent)
        {
            Directions newDirection = Directions.None;
            int maxIterations = 10;
            Cell currentCell = WorldMap.GetCell(this.unit.Position.Location);

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
                if (currentCell.CanMoveTo(newDirection) &&
                    (newDirection != GetOppositeDirection(CurrentDirection.First) || CurrentDirection.First == Directions.None))
                {
                    CurrentDirection = new Direction(newDirection);
                    return;
                }
            }

            // Go opposite Direction if no choice to go
            if (currentCell.CanMoveTo(GetOppositeDirection(CurrentDirection.First)))
                CurrentDirection = new Direction(GetOppositeDirection(CurrentDirection.First));
            else
                CurrentDirection = new Direction(Directions.None);

            // Selecting with random might be failed
            // Recheck the availability of all four directions
            if (CurrentDirection.First == Directions.None)
            {
                if (currentCell.CanMoveTo(Directions.Up))
                {
                    CurrentDirection = new Direction(Directions.Up);
                    return;
                }
                else if (currentCell.CanMoveTo(Directions.Left))
                {
                    CurrentDirection = new Direction(Directions.Left);
                    return;
                }
                else if (currentCell.CanMoveTo(Directions.Down))
                {
                    CurrentDirection = new Direction(Directions.Down);
                    return;
                }
                else if (currentCell.CanMoveTo(Directions.Right))
                {
                    CurrentDirection = new Direction(Directions.Right);
                    return;
                }
            }
        }

        protected override void OnDestinationReached()
        {
            this.cellReached = true;

            unit.Position = new GPS(unit.Position, 25, 25);

            if (Random.Int(100) <= 33)  // 33% chance to change direction
                SelectNewDirection();

            if (!WorldMap.GetCell(this.unit.Position.Location).CanMoveTo(CurrentDirection.First))
                SelectNewDirection();

            // Deimos can not pass through the Start Point
            // Check the next cell whether it is such block

            Cell nextCell;

            DefineNextGPS(CurrentDirection);

            nextCell = WorldMap.GetCell(nextGPS.Location);
            if (nextCell.HasAttribute(CellAttributes.IsStart))
            {
                SelectNewDirection(false);
                DefineNextGPS(CurrentDirection);
            }
        }

    }
}
