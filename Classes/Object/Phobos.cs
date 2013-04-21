using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Phobos : Unit
    {
        public Phobos()
        {
            UnitType = UnitTypes.Phobos;
            SetUnitFlags(UnitFlags.CanNotBeKilled);

            this.motionMaster = new ChasingMovement(this);

            BaseSpeed = 0.4d;
        }

        public override void UpdateState(int timeP)
        {
            this.motionMaster.UpdateState(timeP);

            base.UpdateState(timeP);
        }

    }
}
