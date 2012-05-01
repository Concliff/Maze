using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Slime : GridObject
    {
        private int timeToLive;

        public Slime()
        {
            gridObjectType = GridObjectType.Slime;
            timeToLive = 5000;
        }
        public Slime(GridGPS currentGridGPS)
            : this()
        {
            Position = currentGridGPS;
            currentGridMap = GetWorldMap().GetGridMap(currentGridGPS.Location);
        }

        public int GetTTL()
        {
            return timeToLive;
        }

        public override void UpdateState(int timeP)
        {
            if (timeToLive < timeP)
            {
                SetObjectState(ObjectState.Removed);
            }
            else
                timeToLive -= timeP;

            base.UpdateState(timeP);
        }
    }
}
