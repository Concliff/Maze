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

            Position = new GridGPS(Home, 25, 25);

            this.motionMaster = new RandomMovementGenerator(this);

            //currentGridMap = GetWorldMap().GetGridMap(Position.Location);
        }

        public Deimos(GPS respawnLocation)
            : this()
        {
            Home = respawnLocation;

            Position = new GridGPS(Home, 25, 25);
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
