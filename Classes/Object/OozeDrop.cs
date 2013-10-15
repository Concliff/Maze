using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents the main goal of the level. Collecting drops recover Slug's energy and allow to get the next level.
    /// </summary>
    public class OozeDrop : GridObject
    {
        public OozeDrop()
        {
            this.gridObjectType = GridObjectTypes.OozeDrop;
            this.gridObjectsFlags |= GridObjectFlags.Disposable;
        }

        public override void Use(Unit user)
        {
            if (!IsActive || user.ObjectType != ObjectTypes.Slug || !user.IsVisible || !user.IsAlive)
                return;

            ((Slug)user).CollectDrop(this);

            base.Use(user);
        }
    }
}
