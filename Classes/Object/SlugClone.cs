﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents an exact copy of the <see cref="Slug"/> unit that moves at the same direction and detonates by any obstacle or unit.
    /// </summary>
    public class SlugClone : Unit
    {
        public SlugClone()
        {
            this.unitType = UnitTypes.SlugClone;
            this.ObjectType = ObjectTypes.Unit;
            this.unitSide = UnitSides.Good;
            this.currentCell = Map.Instance.GetCell(Position.Location);

            objectSize.Width = GlobalConstants.PLAYER_SIZE_WIDTH;
            objectSize.Height = GlobalConstants.PLAYER_SIZE_HEIGHT;

            this.motionMaster = new CustomMovement(this);

            BaseSpeed = 1.0d;
        }

        public void Create(GPS position, Movement.Direction currentDirection)
        {
            ((CustomMovement)this.motionMaster).SetDirection(currentDirection);
            base.Create(position);
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
            }

            base.UpdateState(timeP);
        }

        public override void SetDeathState(DeathStates deathState)
        {
            if (deathState == DeathStates.Dead)
            {
                // Cast Slimy Trick
                CastEffect(9, this);
                ObjectState = ObjectStates.Removed;
            }
        }

        protected override void UnitCollision(Unit unit)
        {
            unit.KillUnit(this);
        }
    }
}
