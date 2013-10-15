using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents a special area with limited visibility.
    /// </summary>
    public class SmokeCloud : GridObject
    {
        public static int SmokeDuration = 5000;

        /// <summary>
        /// Inilitializes a new instance of the SmokeCloud class.
        /// </summary>
        public SmokeCloud()
        {
            this.gridObjectType = GridObjectTypes.SmokeCloud;
            this.gridObjectsFlags |= GridObjectFlags.Temporal | GridObjectFlags.AreaEffect;

            // Smoke Effect id
            this.areaEffect.ID = 13;
            this.areaEffect.Range = 75;

            this.timeToLive = SmokeCloud.SmokeDuration;
        }
    }
}
