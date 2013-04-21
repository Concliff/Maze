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
            GridObjectType = GridObjectTypes.OozeDrop;
            SetFlag(GridObjectFlags.Disposable);
        }

        public OozeDrop(Cell cell)
            : this()
        {
            Position = new GPS(cell.Location);
        }

        public override void Use(Unit user)
        {
            if (!IsActive() || user.ObjectType != ObjectTypes.Slug || !user.IsVisible() || !user.IsAlive())
                return;

            ((Slug)user).CollectDrop(this);

            base.Use(user);
        }
    }
}
