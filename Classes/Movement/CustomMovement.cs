using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents a movement generator that provides motion with the defined Direction from the outside.
    /// </summary>
    public class CustomMovement : MovementGenerator
    {
        public CustomMovement(Unit unit)
            : base(unit) { }

        public override void UpdateState(int timeP)
        {
            if (CurrentDirection.First == Directions.None)
                return;

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * this.mover.SpeedRate;
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            Move(movementStep);
        }

        /// <summary>
        /// Sets the motion direction that the mover will adhere to.
        /// </summary>
        /// <param name="direction">Current direction of the movement.</param>
        public void SetDirection(Direction direction)
        {
            CurrentDirection = direction;
        }
    }
}
