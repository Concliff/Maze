using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Phobos : Unit
    {
        public Phobos(GPS respawnLocation)
        {
            UnitType = UnitTypes.Phobos;
            SetUnitFlags(UnitFlags.CanNotBeKilled);
            isInMotion = false;

            Home = respawnLocation;

            Position = new GridGPS(respawnLocation, 25, 25);

            this.motionMaster = new ChasingMovement(this);

            currentGridMap = GetWorldMap().GetGridMap(Position.Location);

            BaseSpeed = 0.4d;
        }

        public override void UpdateState(int timeP)
        {
            // Nothing to do if is dead
            if (this.isInMotion && !HasEffectType(EffectTypes.Root))
                this.motionMaster.UpdateState(timeP);

            base.UpdateState(timeP);
        }


        public override void StartMotion()
        {
            this.isInMotion = true;
        }

        public void StopMotion() { isInMotion = false; }

    }
}
