using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Maze.Forms;
using Maze.Classes;


namespace Maze
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create World instance to process with
            // application common data (win Forms, Stores, Files)
            World WorldMgr = new World();
        }
    }
}
