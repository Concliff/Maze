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

            uint moveType = 0x010;

            // Review all the keys are currently down
            for (int counter = 0; counter < keyMgr.KeysDownCount; ++counter)
                switch (keyMgr.KeyDown(counter))
                {
                    // Catch moving keys
                    case Keys.Up:
                    case Keys.W: moveType += GetNumericValue(Math.PI / 2); break;
                    case Keys.Left:
                    case Keys.A: moveType += GetNumericValue(Math.PI); break;
                    case Keys.Down:
                    case Keys.S: moveType += GetNumericValue(3 * Math.PI / 2); break;
                    case Keys.Right:
                    case Keys.D: moveType += GetNumericValue(0); break;
                }

            // Check if moving occurs
            if ((moveType & GetNumericValue(Math.PI / 2)) == 0 && (moveType & GetNumericValue(3 * Math.PI / 2)) == 0 &&
                (moveType & GetNumericValue(0)) == 0 && (moveType & GetNumericValue(Math.PI)) == 0)
                return;

            MovementAction(moveType);

        }

        private void MovementAction(uint moveType)
        {
            // Define the orientation of motion
            ObjectOrientation objectOrientation = new ObjectOrientation();

            for (int i = 0; i < 4; ++i)
            {
                if ((moveType & GetNumericValue(i * Math.PI / 2)) != 0)
                {
                    objectOrientation.Orientation = i * Math.PI / 2;
                    break;
                }
            }

            if (objectOrientation.Orientation == -1)
                return;

            moveType -= GetNumericValue(objectOrientation.Orientation);

            //        |pi/2
            //        |
            // pi-----------0
            //        |
            //        |3pi/2

            for (int i = 0; i < 4; ++i)
            {
                if ((moveType & GetNumericValue(i * Math.PI / 2)) != 0)
                {
                    if (objectOrientation.Orientation > 0)
                    {
                        objectOrientation.Orientation -= (objectOrientation.Orientation - i * Math.PI / 2) / 2;
                    }
                    else if (objectOrientation.Orientation == 0)
                    {
                        if (i == 1)
                        {
                            objectOrientation.Orientation = Math.PI / 4;
                            break;
                        }
                        else if (i == 3)
                        {
                            objectOrientation.Orientation = 7 * Math.PI / 4;
                            break;
                        }
                    }
                }
            }

            // Reverse moving if has Reverse effect
            if (mover.HasEffectType(EffectTypes.MoveReverse))
            {
                objectOrientation.Orientation = GetOppositeOrientation(objectOrientation.Orientation);
            }

            this.Orientation = objectOrientation.Orientation;

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * this.mover.SpeedRate;

            if (this.Orientation % Math.PI / 2 != 0)
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
