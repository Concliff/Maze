using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Coin : GridObject
    {
        public Coin()
        {
            gridObjectType = GridObjectType.Coin;
        }

        public Coin(GridMap gridMap)
            : this()
        {
            Position.Location = gridMap.Location;
            currentGridMap = gridMap;
        }
        public override void Use(Unit user)
        {
            if (objectState != GridObjectState.Active || user.GetType() != ObjectType.Slug)
                return;

            ((Slug)user).CollectCoin(this);

            SetState(GridObjectState.Inactive);
        }
    }
}
