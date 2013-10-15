using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents the movement generator that provides motion with randomly selected directions to the neighbour cells. The Algorithm provides a less chaotic movement and more ordered path selection.
    /// </summary>
    public class RandomMovementGenerator : MovementGenerator
    {
        /// <summary>
        /// Initializes a new instance of the RandomMovementGenerator class.
        /// </summary>
        /// <param name="unit">The owner of the generator</param>
        public RandomMovementGenerator(Unit unit)
            : base(unit)
        {
            this.generatorType = MovementGeneratorType.Random;
        }

        /// <summary>
        /// Overrides <see cref="MovementGenerator.UpdateState"/>.
        /// </summary>
        public override void UpdateState(int timeP)
        {
            if (!IsInMotion || this.mover.HasEffectType(EffectTypes.Root))
                return;

            if (this.remainDistance <= 0)
            {
                OnDestinationReached();
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

        /// <summary>
        /// Randomly selects a new direction of movement if this direction is available for the <see cref="MovementGenerator.owner"/>.
        /// </summary>
        private void SelectNewDirection()
        {
            SelectNewDirection(true);
        }

        /// <summary>
        /// Randomly selects a new direction of movement if this direction is available for the <see cref="MovementGenerator.owner"/>.
        /// </summary>
        /// <param name="includeCurrent"><c>true</c> if the new direction may be equal to the previous; otherwise, <c>false</c>.</param>
        private void SelectNewDirection(bool includeCurrent)
        {
            Directions newDirection = Directions.None;
            int maxIterations = 10;
            Cell currentCell = Map.Instance.GetCell(this.mover.Position.Location);

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
                }
                else if (currentCell.CanMoveTo(Directions.Left))
                {
                    CurrentDirection = new Direction(Directions.Left);
                }
                else if (currentCell.CanMoveTo(Directions.Down))
                {
                    CurrentDirection = new Direction(Directions.Down);
                }
                else if (currentCell.CanMoveTo(Directions.Right))
                {
                    CurrentDirection = new Direction(Directions.Right);
                }
            }
        }

        protected override void OnDestinationReached()
        {
            mover.Position = new GPS(mover.Position, 25, 25);

            if (CurrentDirection.First == Directions.None) // First time moving
                SelectNewDirection();
            else if (Random.Int(100) <= 33)  // 33% chance to change direction
                SelectNewDirection();

            if (!Map.Instance.GetCell(this.mover.Position.Location).CanMoveTo(CurrentDirection.First))
                SelectNewDirection();

            DefineNextGPS();

            // Deimos can not pass through the Start Point
            // Check the next cell whether it is such block

            Cell nextCell;

            nextCell = Map.Instance.GetCell(nextGPS.Location);
            if (nextCell.HasAttribute(CellAttributes.IsStart))
            {
                SelectNewDirection(false);
                DefineNextGPS();
            }


        }

    }
}
