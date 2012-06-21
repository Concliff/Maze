using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class GlobalConstants
    {
        public static int TIMER_TICK_IN_MS      = 20;    

        // Movement, GridMap
        public static int MOVEMENT_STEP_PX = 5;
        public static int GRIDMAP_BORDER_PX = 4;
        public static int GRIDMAP_BLOCK_HEIGHT = 50;
        public static int GRIDMAP_BLOCK_WIDTH = 50;
        public static int GRIDMAP_WIDTH = 13;
        public static int GRIDMAP_HEIGHT = 9;

        // Files && Directories
        public static string MAPS_PATH = "Data\\Maps\\";
        public static string IMAGES_PATH = "Data\\Images\\";

        // Player
        public static int PLAYER_SIZE_WIDTH = 32;
        public static int PLAYER_SIZE_HEIGHT = 37;

    }
}
