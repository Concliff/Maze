using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Maze.Classes
{
    public class Map
    {
        //
        // SINGLETON PART
        //
        #region singleton_part

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static Map worldMap;

        /// <summary>
        /// Returns World Map instance
        /// </summary>
        public static Map WorldMap
        {
            get
            {
                if (worldMap == null)
                    worldMap = new Map();

                return worldMap;
            }
            private set { ;}
        }

        /// <summary>
        /// Private constructor for hiding singleton instance
        /// </summary>
        private Map()
        {
            LoadMapNameList();
        }

        #endregion


        private List<GridMap> MapBlocks;    // Array of GridMap block of current map
        private int[] dropsCount;            // Ooze Drops per level on map
        private string[] MapNameList;       // Names of All downloaded maps
        private List<GPS> StartPoint;
        private List<GPS> FinishPoint;
        private string CurrentMapName;
        private bool GridMapChanged;        // If map changed, it should be rewrited into mapFile
        private int currentMapIndex;
        private string MapDirectoryPath = GlobalConstants.MAPS_PATH;
        private bool isRandom;

        /// <summary>
        /// Gets the count of non-collected Ooze drops on current level.
        /// </summary>
        public int DropsRemain
        {
            get
            {
                return dropsCount[CurrentLevel];
            }
        }

        private int pr_CurrentLevel;
        /// <summary>
        /// Gets the index number of the current level
        /// </summary>
        public int CurrentLevel
        {
            get { return pr_CurrentLevel; }
            private set { pr_CurrentLevel = value; }
        }

        private int pr_LevelCount;
        /// <summary>
        /// Gets the number of levels of current map
        /// </summary>
        public int LevelCount
        {
            get { return pr_LevelCount; }
            private set { pr_LevelCount = value; }
        }

        private int pr_BlocksCount;
        /// <summary>
        /// Gets the number of blocks of current map
        /// </summary>
        public int BlocksCount
        {
            get { return pr_BlocksCount; }
            private set { pr_BlocksCount = value; }
        }

        ~Map()
        {
            /// Save changed Map blocks back into file (Not for random mode)
            if (GridMapChanged)
                SaveToFile();
        }

        private void LoadMapNameList()
        {
            DirectoryInfo MapDirectory = new DirectoryInfo(MapDirectoryPath);
            FileInfo[] MapFiles = MapDirectory.GetFiles();
            MapNameList = new string[MapFiles.Count()];

            for (int i = 0; i < MapFiles.Count(); ++i)
            {
                string[] MapName = new string[2];
                MapName = MapFiles[i].Name.Split('.');
                if (MapName[1].Equals("map"))
                    MapNameList[i] = MapName[0];
            }

        }

        public void CloseCurrentMap()
        {
            if (GridMapChanged)
                SaveToFile();
        }

        public void Reset()
        {
            MapBlocks = null;
            BlocksCount = 0;
        }

        /// <summary>
        /// Returns current Map index.
        /// </summary>
        public int GetMap() { return currentMapIndex; }

        public void SetMap(int mapIndex) { SetMap(mapIndex, 0); }

        public void SetMap(int mapIndex, int level)
        {
            if (currentMapIndex != mapIndex || MapBlocks == null)
                LoadMap(mapIndex);
            else
                currentMapIndex = mapIndex;

            CurrentLevel = level;
        }

        public void GenerateRandomMap()
        {
            isRandom = true;  // Mark current Map as RandomMap
            currentMapIndex = 0;
            CurrentLevel = 0;

            // Geneate GridMap blocks
            MazeGenerator generator = new MazeGenerator();
            MapBlocks = generator.Generate(0);
            BlocksCount = MapBlocks.Count;
            StartPoint = new List<GPS>();
            FinishPoint = new List<GPS>();
            StartPoint.Add(generator.StartPoint.Location);
            FinishPoint.Add(generator.FinishPoint.Location);

            LevelCount = 1;
            dropsCount = new int[LevelCount];
            // Generate Drops Count
            // Every 15th block should have a drop
            int dropsCounter = MapBlocks.Count / 15;

            // Generate Drops Location
            int currentDropsCount = 0;
            while (currentDropsCount < dropsCounter)
            {
                GridMap block = MapBlocks[Random.Int(MapBlocks.Count)];
                if (block.HasAttribute(GridMapAttributes.HasDrop) ||
                    block.HasAttribute(GridMapAttributes.IsStart) ||
                    block.HasAttribute(GridMapAttributes.IsFinish))
                    continue;

                block.Attribute += (uint)GridMapAttributes.HasDrop;
                ++currentDropsCount;
                ReplaceGridMap(block);
            }

            // Generate Portals
            // Every 50th block
            int portalCounter = MapBlocks.Count / 50;
            int portalCount = 0;

            while (portalCount < portalCounter)
            {
                GridMap portalBlock = MapBlocks[Random.Int(MapBlocks.Count)];
                GridMap destinationBlock = MapBlocks[Random.Int(MapBlocks.Count)];

                if (portalBlock.ID == destinationBlock.ID ||
                    portalBlock.HasAttribute(GridMapAttributes.HasDrop) ||
                    portalBlock.HasAttribute(GridMapAttributes.IsStart) ||
                    portalBlock.HasAttribute(GridMapAttributes.IsFinish) ||
                    destinationBlock.HasAttribute(GridMapAttributes.IsStart))
                    continue;

                portalBlock.Option += (uint)GridMapOptions.Portal;
                portalBlock.OptionValue = destinationBlock.ID;
                ++portalCount;
                ReplaceGridMap(portalBlock);
            }
        }

        public bool IsRandom()
        {
            return isRandom;
        }

        private void LoadMap(int MapIndex)
        {
            LoadFromFile(MapNameList[MapIndex] + ".map");
        }

        public bool CreateMap(string MapName)
        {
            if (IsMapExist(MapName))
                return false;
            MapName = MapName + ".map";
            File.Create(MapDirectoryPath + MapName).Close();
            LoadFromFile(MapName);

            return true;
        }

        private void LoadFromFile(string MapFileName)
        {
            this.isRandom = false;
            MapBlocks = new List<GridMap>();
            StartPoint = new List<GPS>();
            FinishPoint = new List<GPS>();
            CurrentMapName = MapFileName.Split('.')[0];
            int levelIndicator = 0;

            StreamReader GridMapStream = File.OpenText(MapDirectoryPath + MapFileName);
            string CurrentString;
            while ((CurrentString = GridMapStream.ReadLine()) != null)
            {
                string[] StringStruct = new string[10];
                StringStruct = CurrentString.Split(' ');
                // Processing

                GridMap GridMapStruct;

                GridMapStruct.ID = Convert.ToInt32(StringStruct[0]);
                GridMapStruct.Location.X = Convert.ToInt32(StringStruct[1]);
                GridMapStruct.Location.Y = Convert.ToInt32(StringStruct[2]);
                GridMapStruct.Location.Z = Convert.ToInt32(StringStruct[3]);
                GridMapStruct.Location.Level = Convert.ToInt32(StringStruct[4]);
                GridMapStruct.Type = Convert.ToUInt32(StringStruct[5]);
                GridMapStruct.Attribute = Convert.ToUInt32(StringStruct[6]);
                GridMapStruct.Option = Convert.ToUInt32(StringStruct[7]);
                GridMapStruct.OptionValue = Convert.ToInt32(StringStruct[8]);
                GridMapStruct.ND4 = Convert.ToInt32(StringStruct[9]);

                AddGridMap(GridMapStruct);

                if (Convert.ToInt32(StringStruct[4]) >= levelIndicator)
                    levelIndicator++;

                if (GridMapStruct.HasAttribute(GridMapAttributes.IsStart))
                    StartPoint.Insert(GridMapStruct.Location.Level, GridMapStruct.Location);
                if (GridMapStruct.HasAttribute(GridMapAttributes.IsFinish))
                    FinishPoint.Insert(GridMapStruct.Location.Level, GridMapStruct.Location);
            }
            GridMapStream.Close();
            LevelCount = levelIndicator;
            dropsCount = new int[LevelCount];
        }

        private void SaveToFile()
        {
            if (isRandom)
                return;

            StreamWriter GridMapStream = new StreamWriter(MapDirectoryPath + CurrentMapName + ".map", false);
            string GridMapString;

            foreach(GridMap Block in MapBlocks)
            {
                GridMapString = Block.ID.ToString() + " "
                    + Block.Location.X.ToString() + " "
                    + Block.Location.Y.ToString() + " "
                    + Block.Location.Z.ToString() + " "
                    + Block.Location.Level.ToString() + " "
                    + Block.Type.ToString() + " "
                    + Block.Attribute.ToString() + " "
                    + Block.Option.ToString() + " "
                    + Block.OptionValue.ToString() + " "
                    + Block.ND4.ToString();
                GridMapStream.WriteLine(GridMapString);
            }
            GridMapStream.Close();
        }

        public void FillMapWithUnits()
        {
            foreach (GridMap block in MapBlocks)
            {
                if (block.HasAttribute(GridMapAttributes.HasDrop))
                    // Create Deimos at Ooze Drop Location
                    new Deimos(block.Location);
            }

            // Test-created monsters
            new Phobos(FinishPoint[CurrentLevel]);
        }

        internal void FillMapWithObjects()
        {
            foreach (GridMap block in MapBlocks)
            {
                if (block.HasAttribute(GridMapAttributes.HasDrop))
                {
                    new OozeDrop(block);
                    ++dropsCount[block.Location.Level];
                }

                if (block.HasOption(GridMapOptions.Portal))
                {
                    Portal portal = new Portal(block);
                    portal.SetDestination(GetGridMap(block.OptionValue));
                }
            }
        }

        public GridMap GetGridMap(int BlockID)
        {
            GridMap MapBlock = new GridMap();
            MapBlock.Initialize();

            for (int i = 0; i < BlocksCount; ++i)
                if (((GridMap)MapBlocks[i]).ID == BlockID)
                    return (GridMap)MapBlocks[i];

            return MapBlock;
        }
        public GridMap GetGridMap(GPS BlockLocation)
        {
            GridMap MapBlock = new GridMap();
            MapBlock.Initialize();// Initialize(MapBlock);
            MapBlock.Location = BlockLocation;

            for (int i = 0; i < MapBlocks.Count; ++i)
                if (((GridMap)MapBlocks[i]).Location.Equals(BlockLocation))
                    return (GridMap)MapBlocks[i];

            return MapBlock;
        }

        public bool AddGridMap(GridMap NewGridMap)
        {
            if (GetGridMap(NewGridMap.ID).ID != -1)
            {
                ReplaceGridMap(NewGridMap);
                return false;
            }

            MapBlocks.Add(NewGridMap);
            ++BlocksCount;
            GridMapChanged = true;
            return true;
        }

        public void AddEmptyGrigMap(GridMap NewGridMap)
        {
            RemoveGridMap(NewGridMap);
        }

        private bool RemoveGridMap(GridMap RemovedGridMap)
        {
            MapBlocks.Remove(RemovedGridMap);
            --BlocksCount;
            return true;
        }
        private bool ReplaceGridMap(GridMap ChangedGridMap)
        {
            RemoveGridMap(GetGridMap(ChangedGridMap.ID));
            AddGridMap(ChangedGridMap);
            return true;
        }

        public string[] GetMapNamesList()
        {
            return MapNameList;
        }

        public GPS GetStartPoint()
        {
            GPS result = new GPS();
            result.Level = CurrentLevel;
            if (CurrentLevel < LevelCount)
                result = StartPoint[CurrentLevel];
            return result;
        }
        public GPS GetFinishPoint()
        {
            GPS result = new GPS();
            result.Level = CurrentLevel;
            if (CurrentLevel <= FinishPoint.Count)
                result = FinishPoint[CurrentLevel];
            return result;
        }

        private bool IsMapExist(string MapName)
        {
            for (int i = 0; i < MapNameList.Count(); ++i)
                if (MapNameList[i].Equals(MapName))
                    return true;

            return false;
        }

        public void CollectDrop(OozeDrop drop)
        {
            --dropsCount[CurrentLevel];
        }
    }
}
