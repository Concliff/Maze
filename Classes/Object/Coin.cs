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
            SetFlag(GridObjectFlags.Disposable);
        }

        public Coin(GridMap gridMap)
            : this()
        {
            Position.Location = gridMap.Location;
            currentGridMap = gridMap;
        }
        public override void Use(Unit user)
        {
            if (!IsActive() || user.GetType() != ObjectType.Slug)
                return;

            ((Slug)user).CollectCoin(this);

            base.Use(user);
        }
    }
}
