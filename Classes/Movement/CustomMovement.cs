using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents a movement generator, provides motion with the set Direction from the outside.
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

        public void SetDirection(Direction direction)
        {
            CurrentDirection = direction;
        }
    }
}
