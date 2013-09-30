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
            GridObjectType = GridObjectTypes.Portal;
            this.activationTime = 3000;  // 3 seconds
            this.destinationCell.Initialize();
        }

        public void SetDestination(GridLocation destinationGPS)
        {
            Cell destinationCell = Map.Instance.GetCell(destinationGPS);
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
