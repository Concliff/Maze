using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents a hostile unit that rapidly moves in randomly selected direction.
    /// </summary>
    class Deimos : Unit
    {
        /// <summary>
        /// Initializes a new instance of the Deimos class.
        /// </summary>
        public Deimos()
        {
            this.unitType = UnitTypes.Deimos;

            Position = new GPS(Home, 25, 25);

            this.motionMaster = new RandomMovementGenerator(this);

            //currentCell = GetWorldMap().GetCell(Position.Location);
        }

        public override void UpdateState(int timeP)
        {
            this.motionMaster.UpdateState(timeP);

            base.UpdateState(timeP);
        }

        public override void SetDeathState(DeathStates deathState)
        {
            if (deathState == DeathStates.Dead)
                StopMotion();
            else
                StartMotion();

            base.SetDeathState(deathState);
        }

    }
}
