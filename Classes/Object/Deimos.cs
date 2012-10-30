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

            Position = new GridGPS(respawnLocation, 25, 25);

            isInMotion = false;

            this.motionMaster = new RandomMovementGenerator(this);

            //currentGridMap = GetWorldMap().GetGridMap(Position.Location);
        }

        public Deimos(GPS respawnLocation)
            : this()
        {
            this.respawnLocation = respawnLocation;

            Position = new GridGPS(respawnLocation, 25, 25);
        }

        public override void UpdateState(int timeP)
        {
            if (isInMotion & !HasEffectType(EffectTypes.Root))
                this.motionMaster.UpdateState(timeP);

            base.UpdateState(timeP);
        }

        public override void StartMotion()
        {
            // Already started
            if (isInMotion)
                return;

            isInMotion = true;
        }

        public void StopMotion() { isInMotion = false; }

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
