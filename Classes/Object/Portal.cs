using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Portal : GridObject
    {
        private bool recentlyUsed;
        private int timeToActivate;
        private GridMap destinationGridMap;

        public Portal()
        {
            gridObjectType = GridObjectType.Portal;
            recentlyUsed = false;
            timeToActivate = 3000;  // 3 seconds
            destinationGridMap.Initialize();
        }

        public Portal(GridMap position)
            : this()
        {
            Position.Location = position.Location;
            currentGridMap = position;
        }

        public void SetDestination(GPS destinationGPS)
        {
            GridMap destinationGridMap = GetWorldMap().GetGridMap(destinationGPS);
            SetDestination(destinationGridMap);
        }

        public void SetDestination(GridMap destinationGridMap)
        {
            this.destinationGridMap = destinationGridMap;
        }

        public override void UpdateState(int timeP)
        {
            if (recentlyUsed)
            {
                if (timeToActivate < timeP)
                {
                    SetGridObjectState(GridObjectState.Active);
                    recentlyUsed = false;
                }
                else
                {
                    timeToActivate -= timeP;
                }
            }

            base.UpdateState(timeP);
        }

        public override void Use(Unit user)
        {
            user.TeleportTo(destinationGridMap);

            SetGridObjectState(GridObjectState.Inactive);
            recentlyUsed = true;
            timeToActivate = 3000;

            base.Use(user);
        }
    }
}
