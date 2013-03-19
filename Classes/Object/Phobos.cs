using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Phobos : Unit
    {
        public Phobos(GridLocation respawnLocation)
        {
            UnitType = UnitTypes.Phobos;
            SetUnitFlags(UnitFlags.CanNotBeKilled);

            Home = respawnLocation;

            Position = new GPS(respawnLocation, 25, 25);

            this.motionMaster = new ChasingMovement(this);

            currentGridMap = GetWorldMap().GetGridMap(Position.Location);

            BaseSpeed = 0.4d;
        }

        public override void UpdateState(int timeP)
        {
            this.motionMaster.UpdateState(timeP);

            base.UpdateState(timeP);
        }

    }
}
