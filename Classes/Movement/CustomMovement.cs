using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class CustomMovement : MovementGenerator
    {
        public CustomMovement(Unit unit)
            : base(unit) { }

        public override void UpdateState(int timeP)
        {
            if (CurrentDirection.First == Directions.None)
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

        public void SetDirection(Direction direction)
        {
            CurrentDirection = direction;
        }
    }
}
