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
            if (!IsInMotion || this.mover.HasEffectType(EffectTypes.Root))
                return;

            if (this.remainDistance <= 0)
            {
                OnDestinationReached();
                IsOrientChanged = false;
                //return;
            }

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * this.mover.SpeedRate;
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            remainDistance -= movementStep;

            Move(movementStep);
        }

        public override void StartMotion()
        {
            SelectNewDirection();
            base.StartMotion();
        }

        private void SelectNewDirection()
        {
            SelectNewDirection(true);
        }

        private void SelectNewDirection(bool includeCurrent)
        {
            double newOrientation = 0;

            int maxIterations = 10;
            Cell currentCell = WorldMap.GetCell(this.mover.Position.Location);

            for (int i = 0; i < maxIterations; ++i)
            {
                switch(Random.Int(3))
                {
                    case 0: newOrientation = ORIENTATION_RIGHT; break;
                    case 1: newOrientation = ORIENTATION_UP; break;
                    case 2: newOrientation = ORIENTATION_LEFT; break;
                    case 3: newOrientation = ORIENTATION_DOWN; break;
                }
                if (!includeCurrent && newOrientation % Math.PI / 2 == 0)
                    continue;

                // Ignore Opposite Direction if there is another one
                if (currentCell.CanMoveTo(newOrientation) &&
                    (newOrientation != GetOppositeOrientation(Orientation)))
                {
                    Orientation = newOrientation;
                    IsOrientChanged = true;
                    return;
                }
            }

            // Go opposite Direction if no choice to go
            if (currentCell.CanMoveTo(GetOppositeOrientation(Orientation)))
            {
                Orientation = GetOppositeOrientation(Orientation);
                IsOrientChanged = true;
            }
            else
                IsOrientChanged = false;

            // Selecting with random might be failed
            // Recheck the availability of all four directions
            if (IsOrientChanged == false)
            {
                for(int i = 0; i < 4; ++i)
                    if (currentCell.CanMoveTo(i * Math.PI / 2))
                    {
                        Orientation = i * Math.PI / 2;
                        break;
                    }
            }
        }

        protected override void OnDestinationReached()
        {
            mover.Position = new GPS(mover.Position, 25, 25);

            if (IsOrientChanged == false) // First time moving
                SelectNewDirection();
            else if (Random.Int(100) <= 33)  // 33% chance to change direction
                SelectNewDirection();

            if (!WorldMap.GetCell(this.mover.Position.Location).CanMoveTo(Orientation))
                SelectNewDirection();

            DefineNextGPS();

            // Deimos can not pass through the Start Point
            // Check the next cell whether it is such block

            Cell nextCell;

            nextCell = WorldMap.GetCell(nextGPS.Location);
            if (nextCell.HasAttribute(CellAttributes.IsStart))
            {
                SelectNewDirection(false);
                DefineNextGPS();
            }
        }

    }
}
