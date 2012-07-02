﻿using System;
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
            gridObjectType = GridObjectType.Slime;
            SetFlag(GridObjectFlags.Temporal);
            if (World.GetPlayForm().GetPlayer().GetEffectsByType(EffectTypes.SlimeDuration).Count > 0)
                timeToLive = 5000;
            else
                timeToLive = 3000;
        }
        public Slime(GridGPS currentGridGPS)
            : this()
        {
            Position = currentGridGPS;
            currentGridMap = GetWorldMap().GetGridMap(currentGridGPS.Location);
        }

        public override void Use(Unit user)
        {
            if (user.GetType() != ObjectType.Slug)
                user.CastEffect(ViscousSlime, user);

            base.Use(user);
        }
    }
}
