﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class SmokeCloud : GridObject
    {
        public static int SmokeDuration = 5000;

        public SmokeCloud(GPS currentPosition)
        {
            Position = currentPosition;
            currentCell = GetWorldMap().GetCell(currentPosition.Location);

            GridObjectType = GridObjectTypes.SmokeCloud;
            SetFlag(GridObjectFlags.Temporal);
            SetFlag(GridObjectFlags.AreaEffect);

            // Smoke Effect id
            this.areaEffect.ID = 13;
            this.areaEffect.Range = 75;

            this.timeToLive = SmokeCloud.SmokeDuration;
        }
    }
}
