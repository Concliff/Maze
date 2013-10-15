using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Maze.Forms;
using Maze.Classes;

namespace Maze
{
    /// <summary>
    /// Represents the first start entry point for whole program initilization and creation.
    /// </summary>
    public class World
    {
        private static Play pr_PlayForm;
        /// <summary>
        /// Get the main Game Form instance.
        /// </summary>
        public static Play PlayForm
        {
            get { return pr_PlayForm; }
        }

        /// <summary>
        /// Initializes a new instance of the World class.
        /// </summary>
        public World()
        {
            Run();
        }

        /// <summary>
        /// Inilializes and loads all game resources and run game <see cref="Play"/> Form.
        /// </summary>
        private void Run()
        {
            //Initialize Utilities
            Random.Initialize();

            // Load Datas
            DBStores.Load();

            // Load all image files and graphic parts
            PictureManager.Load();

            // Create Play Form
            pr_PlayForm = new Play();

            // Run Play Form
            Application.Run(PlayForm);
        }
    }
}
