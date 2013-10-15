using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents the collection of settings parameters of the game application.
    /// </summary>
    public static class GlobalConstants
    {
        public const int TIMER_TICK_IN_MS      = 20;

        // Movement, Grid
        public const int MOVEMENT_STEP_PX = 4;
        public const int CELL_BORDER_PX = 4;
        public const int CELL_HEIGHT = 50;
        public const int CELL_WIDTH = 50;
        public const int GRIDMAP_WIDTH = 13;
        public const int GRIDMAP_HEIGHT = 9;

        // Files && Directories
        public const string MAPS_PATH = "Data\\Maps\\";
        public const string IMAGES_PATH = "Data\\Images\\";

        // Player
        public const int PLAYER_SIZE_WIDTH = 32;
        public const int PLAYER_SIZE_HEIGHT = 32;

    }
}
