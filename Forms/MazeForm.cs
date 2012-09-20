using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Classes;

namespace Maze.Forms
{
    public partial class MazeForm : System.Windows.Forms.Form
    {
        private static ObjectContainer objectContainer;
        private static Map WorldMap;

        public static int FormTitleBarSize = 28;
        public static int FormBorderBarSize = 7;

        /// <summary>
        /// Contains a reference to Map singleton instance
        /// </summary>
        protected Map worldMap;

        public MazeForm()
        {
            CreateObjectContainer();

            worldMap = Map.WorldMap;
        }

        protected void CreateObjectContainer()
        {
            objectContainer = new ObjectContainer();
        }

        public static ObjectContainer GetObjectContainer()
        {
            return objectContainer;
        }
    }
}
