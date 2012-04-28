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
        private static WorldNextAction NextAction;
        private static Play PlayForm;
        private GameForm StartGameForm;
        
        public World()
        {
            NextAction = WorldNextAction.StartGame;
            StartGameForm = new GameForm();
            Program();
        }

        private void Program()
        {
            CreatePlayForm();
            Application.Run(PlayForm);
            // for test
            Algorithm alg = new Algorithm();
            
            /*
            while(true)
            {
                switch (NextAction)
                {
                    case WorldNextAction.GamePlay:
                        NextAction = WorldNextAction.ApplicationQuit;
                        Application.Run(PlayForm); 
                        break;
                    case WorldNextAction.MapEdit: 
                        Application.Run(MapEditorForm); 
                        break;
                    case WorldNextAction.StartGame:
                        NextAction = WorldNextAction.ApplicationQuit;
                        Application.Run(StartGameForm);  
                        break;
                    default: 
                        Application.Exit(); 
                        return;
                }
            }
             * */
        }

        public static Play GetPlayForm()
        {
            return PlayForm;
        }

        public static WorldNextAction GetNextAction()
        {
            return NextAction;
        }
        public static void SetNextAction(WorldNextAction _NextAction)
        {
            NextAction = _NextAction;
        }

        public static void CreatePlayForm()
        {
            PlayForm = new Play();
        }

    }
}
