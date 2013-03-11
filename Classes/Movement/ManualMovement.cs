using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Maze.Classes
{
    public class ManualMovement : MovementGenerator
    {
        public ManualMovement(Unit unit)
            : base(unit)
        {
            generatorType = MovementGeneratorType.Manual;
        }

        public override void UpdateState(int timeP)
        {
            KeyManager keyMgr = World.PlayForm.KeyMgr;

            uint moveType = (uint)Directions.None;

            // Review all the keys are currently down
            for (int counter = 0; counter < keyMgr.KeysDownCount; ++counter)
                switch (keyMgr.KeyDown(counter))
                {
                    // Catch moving keys
                    case Keys.Up:
                    case Keys.W: moveType += (uint)Directions.Up; break;
                    case Keys.Left:
                    case Keys.A: moveType += (uint)Directions.Left; break;
                    case Keys.Down:
                    case Keys.S: moveType += (uint)Directions.Down; break;
                    case Keys.Right:
                    case Keys.D: moveType += (uint)Directions.Right; break;
                }

            // Check if moving occurs
            if ((moveType & (uint)Directions.Up) == 0 && (moveType & (uint)Directions.Down) == 0 &&
                (moveType & (uint)Directions.Right) == 0 && (moveType & (uint)Directions.Left) == 0)
                return;

            MovementAction(moveType);

        }

        private void MovementAction(uint moveType)
        {
            Direction direction;
            // Define direction of motion
            // TODO: improve the next stupid if..else..if
            if ((moveType & (uint)Directions.Up) != 0)
                direction.First = Directions.Up;
            else if ((moveType & (uint)Directions.Down) != 0)
                direction.First = Directions.Down;
            else if ((moveType & (uint)Directions.Left) != 0)
                direction.First = Directions.Left;
            else if ((moveType & (uint)Directions.Right) != 0)
                direction.First = Directions.Right;
            else
                direction.First = Directions.None;

            if (direction.First == Directions.None)
                return;

            moveType -= (uint)direction.First;

            if ((moveType & (uint)Directions.Up) != 0)
                direction.Second = Directions.Up;
            else if ((moveType & (uint)Directions.Down) != 0)
                direction.Second = Directions.Down;
            else if ((moveType & (uint)Directions.Left) != 0)
                direction.Second = Directions.Left;
            else if ((moveType & (uint)Directions.Right) != 0)
                direction.Second = Directions.Right;
            else
                direction.Second = Directions.None;

            if (unit.HasEffectType(EffectTypes.MoveReverse))
            {
                direction.First = GetOppositeDirection(direction.First);
                direction.Second = GetOppositeDirection(direction.Second);
            }

            CurrentDirection = direction;

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * this.unit.SpeedRate;
            if (CurrentDirection.Second != Directions.None)
                movementStepD = Math.Sqrt(2 * movementStepD);
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
