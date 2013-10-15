using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents the puddles of slime on the Map that slow enemy unit passing by and speed up the Slug.
    /// </summary>
    public class Slime : GridObject
    {
        /// <summary>
        /// Slime effect ID
        /// </summary>
        private ushort ViscousSlime = 1;

        public Slime()
        {
            this.gridObjectType = GridObjectTypes.Slime;
            this.gridObjectsFlags |= GridObjectFlags.Temporal;
            if (World.PlayForm.Player.HasEffectType(EffectTypes.SlimeDuration))
                timeToLive = 5000;
            else
                timeToLive = 3000;
        }

        public override void Use(Unit user)
        {
            if (user.ObjectType != ObjectTypes.Slug)
                user.CastEffect(ViscousSlime, user);

            base.Use(user);
        }
    }
}
