using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Classes;

namespace Maze.Forms
{
    public partial class MazeForm : System.Windows.Forms.Form
    {
        public static int FormTitleBarSize = 28;
        public static int FormBorderBarSize = 7;

        /// <summary>
        /// Contains a reference to Map singleton instance
        /// </summary>
        protected Map worldMap;

        /// <summary>
        /// Contains a reference to Object Container instance
        /// </summary>
        protected ObjectContainer objectContainer;

        public MazeForm()
        {
            objectContainer = ObjectContainer.Container;

            worldMap = Map.WorldMap;
        }
    }
}
