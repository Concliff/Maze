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
        /// Returns current play form
        /// </summary>
        public static Play PlayForm
        {
            get { return pr_PlayForm; }
            private set { ;}
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

            PictureManager.InitializeComponents();
            PictureManager.Load();

            CreatePlayForm();
            Application.Run(pr_PlayForm);
            
        }

        public static void CreatePlayForm()
        {
            pr_PlayForm = new Play();
        }

    }
}
