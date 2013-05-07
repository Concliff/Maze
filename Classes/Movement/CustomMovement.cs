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
            if (IsOrientChanged == false)
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
    }
}
