using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Slime : GridObject
    {
        // Slime effect ID
        private ushort ViscousSlime = 1;

        public Slime()
        {
            GridObjectType = GridObjectTypes.Slime;
            SetFlag(GridObjectFlags.Temporal);
            if (World.PlayForm.Player.HasEffectType(EffectTypes.SlimeDuration))
                timeToLive = 5000;
            else
                timeToLive = 3000;
        }
        public Slime(GPS currentPositionS)
            : this()
        {
            Position = currentPositionS;
            currentCell = GetWorldMap().GetCell(currentPositionS.Location);
        }

        public override void Use(Unit user)
        {
            if (user.ObjectType != ObjectTypes.Slug)
                user.CastEffect(ViscousSlime, user);

            base.Use(user);
        }
    }
}
