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
        private static MapEditor MapEditorForm;
        private GameForm StartGameForm;
        
        public World()
        {
            NextAction = WorldNextAction.StartGame;
            WorldMap = new Map();
            //PlayForm = new Play();
            //MapEditorForm = new MapEditor();
            StartGameForm = new GameForm();
            Program();
        }

        private void Program()
        {
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
        }

        public static Map GetWorldMap()
        {
           return WorldMap;
        }

        public static Play GetPlayForm()
        {
            return PlayForm;
        }

        public static MapEditor GetMapEditorForm()
        {
            return MapEditorForm;
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

        public static void CreateMapEditorForm()
        {
            MapEditorForm = new MapEditor();
        }

        
    }
}
