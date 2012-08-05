using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class SlugClone : Slug
    {
        public SlugClone(GridGPS position, Direction currentDirection)
        {
            unitType = UnitTypes.SlugClone;

            Position = position;
            this.currentGridMap = GetWorldMap().GetGridMap(Position.Location);
            this.currentDirection = currentDirection;

            objectSize.Width = GlobalConstants.PLAYER_SIZE_WIDTH;
            objectSize.Height = GlobalConstants.PLAYER_SIZE_HEIGHT;

            BaseSpeed = 1.0d;
        }

        public override void UpdateState(int timeP)
        {
            // Always in motion
            MovementAction();
            
            base.UpdateState(timeP);
        }

        public override void SetDeathState(DeathStates deathState)
        {
            if (deathState == DeathStates.Dead)
            {
                // Cast Slimy Trick
                CastEffect(9, this);
                SetObjectState(ObjectState.Removed);
            }
        }

        private void MovementAction()
        {
            if (HasEffectType(EffectTypes.Root))
                return;

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * SpeedRate;
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            MoveToDirection(movementStep, currentDirection);

        }

        protected override void ReachedGridMap()
        {
            base.ReachedGridMap();

            if (!currentGridMap.CanMoveTo(currentDirection.First))
                SetDeathState(DeathStates.Dead);

        }
    }
}