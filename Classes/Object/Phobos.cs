using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents a hostile unit that chases the Slug.
    /// </summary>
    public class Phobos : Unit
    {
        /// <summary>
        /// Initializes a new instance of Phobos class.
        /// </summary>
        public Phobos()
        {
            this.unitType = UnitTypes.Phobos;
            this.unitFlags |= UnitFlags.CanNotBeKilled;

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
