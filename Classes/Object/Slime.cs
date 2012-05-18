using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Slime : GridObject
    {
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
                EffectEntry effectEntry = new EffectEntry();
                effectEntry.EffectType = EffectTypes.Speed;
                effectEntry.Duration = -1;
                if (unit.GetType() == ObjectType.Slug)
                    continue;
                else
                    effectEntry.Value = -50;

                Effect effect = new Effect(effectEntry, unit, this);
            }

            base.UpdateState(timeP);
        }
    }
}
