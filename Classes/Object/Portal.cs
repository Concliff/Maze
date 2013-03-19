using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Portal : GridObject
    {
        private Cell destinationCell;

        public Portal()
        {
            gridObjectType = GridObjectType.Portal;
            activationTime = 3000;  // 3 seconds
            destinationCell.Initialize();
        }

        public Portal(Cell position)
            : this()
        {
            GPS startPosition = Position;
            startPosition.Location = position.Location;
            Position = startPosition;
            currentCell = position;
        }

        public void SetDestination(GridLocation destinationGPS)
        {
            Cell destinationCell = GetWorldMap().GetCell(destinationGPS);
            SetDestination(destinationCell);
        }

        public void SetDestination(Cell destinationCell)
        {
            this.destinationCell = destinationCell;
        }

        public override void UpdateState(int timeP)
        {
            base.UpdateState(timeP);
        }

        public override void Use(Unit user)
        {
            if (!user.IsVisible())
                return;
            user.TeleportTo(destinationCell);

            base.Use(user);
        }
    }
}
