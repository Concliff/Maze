using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    class Deimos : Unit
    {

        public Deimos()
        {
            UnitType = UnitTypes.Deimos;

            Position = new GPS(Home, 25, 25);

            this.motionMaster = new RandomMovementGenerator(this);

            //currentGridMap = GetWorldMap().GetGridMap(Position.Location);
        }

        public Deimos(GridLocation respawnLocation)
            : this()
        {
            Home = respawnLocation;

            Position = new GPS(Home, 25, 25);

            this.motionMaster.DefineNextGPS(this.motionMaster.CurrentDirection);
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
