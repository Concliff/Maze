using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Portal : GridObject
    {
        private GridMap destinationGridMap;

        public Portal()
        {
            gridObjectType = GridObjectType.Portal;
            activationTime = 3000;  // 3 seconds
            destinationGridMap.Initialize();
        }

        public Portal(GridMap position)
            : this()
        {
            GridGPS startPosition = Position;
            startPosition.Location = position.Location;
            Position = startPosition;
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
            base.UpdateState(timeP);
        }

        public override void Use(Unit user)
        {
            user.TeleportTo(destinationGridMap);

            base.Use(user);
        }
    }
}
