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

        public OozeDrop(Cell cell)
            : this()
        {
            GPS position = Position;
            position.Location = cell.Location;
            Position = position;
            currentCell = cell;
        }
        public override void Use(Unit user)
        {
            if (!IsActive() || user.GetType() != ObjectType.Slug || !user.IsVisible() || !user.IsAlive())
                return;

            ((Slug)user).CollectDrop(this);

            base.Use(user);
        }
    }
}
