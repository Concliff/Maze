using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Maze.Forms;
using Maze.Classes;

namespace Maze
{
    public class World
    {
        private static Play pr_PlayForm;
        /// <summary>
        /// Returns main Game Form
        /// </summary>
        public static Play PlayForm
        {
            get { return pr_PlayForm; }
        }

        public World()
        {
            Program();
        }

        private void Program()
        {
            //Initialize Utilities
            Random.Initialize();

            // Load Datas
            DBStores.InitializeComponents();
            DBStores.Load();

            // Load all image files and graphic parts
            PictureManager.InitializeComponents();
            PictureManager.Load();

            // Create Play Form
            pr_PlayForm = new Play();

            // Run Play Form
            Application.Run(PlayForm);
        }
    }
}
