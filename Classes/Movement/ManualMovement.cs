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
            pr_isOrientSet = false;
        }

        private void MovementAction(uint moveType)
        {
            // Define the orientation of motion
            double objectOrientation = 0;

            for (int i = 0; i < 4; ++i)
            {
                if ((moveType & (uint)WhatIsDirection(i * Math.PI / 2)) != 0)
                {
                    objectOrientation = i * Math.PI / 2;
                    pr_isOrientSet = true;
                    break;
                }
                else
                    pr_isOrientSet = false;
            }

            if (pr_isOrientSet == false)
                return;

            moveType -= (uint)WhatIsDirection(objectOrientation);

            for (int i = 0; i < 4; ++i)
            {
                if ((moveType & (uint)WhatIsDirection(i * Math.PI / 2)) != 0)
                {
                    objectOrientation = i * Math.PI / 2;
                    //pr_isOrientChanged = true;
                    break;
                }
                /*else
                    pr_isOrientChanged = false;*/
            }

            // Reverse moving if has Reverse effect
            if (mover.HasEffectType(EffectTypes.MoveReverse))
            {
                objectOrientation = GetOppositeDirection(objectOrientation);
            }

            Orientation = objectOrientation;
            pr_isOrientSet = true;

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * this.mover.SpeedRate;

            if (Orientation % Math.PI / 2 != 0)
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
