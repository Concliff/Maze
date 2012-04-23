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
        private static Map WorldMap;
        private static Play PlayForm;
        private GameForm StartGameForm;
        private static UnitContainer unitContainer;
        
        public World()
        {
            unitContainer = new UnitContainer();
            NextAction = WorldNextAction.StartGame;
            WorldMap = new Map();
            StartGameForm = new GameForm();
            Program();
        }

        private void Program()
        {
            WorldMap.SetMap(0);
            CreatePlayForm();
            Application.Run(PlayForm);
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

        public static UnitContainer GetUnitContainer()
        {
            return unitContainer;
        }

        public static Map GetWorldMap()
        {
           return WorldMap;
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
