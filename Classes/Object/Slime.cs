using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Slime : GridObject
    {
        // Slime effect ID
        private ushort ViscousSlime = 1;

        public Slime()
        {
            gridObjectType = GridObjectType.Slime;
            SetFlag(GridObjectFlags.Temporal);
            timeToLive = 3000;
        }
        public Slime(GridGPS currentGridGPS)
            : this()
        {
            Position = currentGridGPS;
            currentGridMap = GetWorldMap().GetGridMap(currentGridGPS.Location);
        }

        public override void UpdateState(int timeP)
        {
            List<Unit> nearestUnits = ObjectSearcher.GetUnitsWithinRange(this, 10);
            foreach (Unit unit in nearestUnits)
            {
                // ignore Slug
                if (unit.GetType() != ObjectType.Slug)
                    unit.CastEffect(ViscousSlime, unit);
            }

            base.UpdateState(timeP);
        }
    }
}
