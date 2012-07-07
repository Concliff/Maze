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
        private static Play PlayForm;
        
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
            Application.Run(PlayForm);
            
        }

        public static Play GetPlayForm()
        {
            return PlayForm;
        }

        public static void CreatePlayForm()
        {
            PlayForm = new Play();
        }

    }
}
