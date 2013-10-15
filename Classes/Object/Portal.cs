using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents a gridobject that relocates a user to another defined location.
    /// </summary>
    public class Portal : GridObject
    {
        /// <summary>
        /// Where users will be relocated.
        /// </summary>
        private Cell destinationCell;

        /// <summary>
        /// Initializes a new instance of Portal class.
        /// </summary>
        public Portal()
        {
            this.gridObjectType = GridObjectTypes.Portal;
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
            if (!user.IsVisible)
                return;
            user.TeleportTo(destinationCell);

            base.Use(user);
        }
    }
}
