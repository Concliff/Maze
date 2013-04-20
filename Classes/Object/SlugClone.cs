﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class SlugClone : Slug
    {
        public SlugClone(GPS position, double orientation)
        {
            UnitType = UnitTypes.SlugClone;
            this.objectType = ObjectType.Slug;
            Position = position;
            this.currentCell = GetWorldMap().GetCell(Position.Location);

            objectSize.Width = GlobalConstants.PLAYER_SIZE_WIDTH;
            objectSize.Height = GlobalConstants.PLAYER_SIZE_HEIGHT;

            this.motionMaster = new CustomMovement(this);
            ((CustomMovement)this.motionMaster).Orientation = orientation;

            BaseSpeed = 1.0d;
        }

        public override void UpdateState(int timeP)
        {
            // Always in motion
            if (!HasEffectType(EffectTypes.Root))
            {
                GPS previousPosition = Position;

                this.motionMaster.UpdateState(timeP);

                if (Position == previousPosition)
                    KillUnit(this);

                return;
            }

            CheckUnitsCollision();
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
    }
}