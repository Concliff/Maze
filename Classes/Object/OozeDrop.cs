using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class OozeDrop : GridObject
    {
        public OozeDrop()
        {
            gridObjectType = GridObjectType.OozeDrop;
            SetFlag(GridObjectFlags.Disposable);
        }

        public OozeDrop(GridMap gridMap)
            : this()
        {
            Position.Location = gridMap.Location;
            currentGridMap = gridMap;
        }
        public override void Use(Unit user)
        {
            if (!IsActive() || user.GetType() != ObjectType.Slug)
                return;

            ((Slug)user).CollectDrop(this);

            base.Use(user);
        }
    }
}
