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


        private List<Cell> MapBlocks;   // Array of cells of current map
        private int[] dropsCount;       // Ooze Drops per level on map
        private string[] MapNameList;   // Names of All downloaded maps
        private List<GridLocation> StartPoint;
        private List<GridLocation> FinishPoint;
        private string CurrentMapName;
        private bool CellChanged;    // If map changed, it should be rewrited into mapFile
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
            if (CellChanged)
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
            if (CellChanged)
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

            // Geneate cell blocks
            MazeGenerator generator = new MazeGenerator();
            MapBlocks = generator.Generate(0);
            BlocksCount = MapBlocks.Count;
            StartPoint = new List<GridLocation>();
            FinishPoint = new List<GridLocation>();
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
                Cell block = MapBlocks[Random.Int(MapBlocks.Count)];
                if (block.HasAttribute(CellAttributes.HasDrop) ||
                    block.HasAttribute(CellAttributes.IsStart) ||
                    block.HasAttribute(CellAttributes.IsFinish))
                    continue;

                block.Attribute += (uint)CellAttributes.HasDrop;
                ++currentDropsCount;
                ReplaceCell(block);
            }

            // Generate Portals
            // Every 50th block
            int portalCounter = MapBlocks.Count / 50;
            int portalCount = 0;

            while (portalCount < portalCounter)
            {
                Cell portalBlock = MapBlocks[Random.Int(MapBlocks.Count)];
                Cell destinationBlock = MapBlocks[Random.Int(MapBlocks.Count)];

                if (portalBlock.ID == destinationBlock.ID ||
                    portalBlock.HasAttribute(CellAttributes.HasDrop) ||
                    portalBlock.HasAttribute(CellAttributes.IsStart) ||
                    portalBlock.HasAttribute(CellAttributes.IsFinish) ||
                    destinationBlock.HasAttribute(CellAttributes.IsStart))
                    continue;

                portalBlock.Option += (uint)CellOptions.Portal;
                portalBlock.OptionValue = destinationBlock.ID;
                ++portalCount;
                ReplaceCell(portalBlock);
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
            MapBlocks = new List<Cell>();
            StartPoint = new List<GridLocation>();
            FinishPoint = new List<GridLocation>();
            CurrentMapName = MapFileName.Split('.')[0];
            int levelIndicator = 0;

            StreamReader CellStream = File.OpenText(MapDirectoryPath + MapFileName);
            string CurrentString;
            while ((CurrentString = CellStream.ReadLine()) != null)
            {
                string[] StringStruct = new string[10];
                StringStruct = CurrentString.Split(' ');
                // Processing

                Cell CellStruct;

                CellStruct.ID = Convert.ToInt32(StringStruct[0]);
                CellStruct.Location.X = Convert.ToInt32(StringStruct[1]);
                CellStruct.Location.Y = Convert.ToInt32(StringStruct[2]);
                CellStruct.Location.Z = Convert.ToInt32(StringStruct[3]);
                CellStruct.Location.Level = Convert.ToInt32(StringStruct[4]);
                CellStruct.Type = Convert.ToUInt32(StringStruct[5]);
                CellStruct.Attribute = Convert.ToUInt32(StringStruct[6]);
                CellStruct.Option = Convert.ToUInt32(StringStruct[7]);
                CellStruct.OptionValue = Convert.ToInt32(StringStruct[8]);
                CellStruct.ND4 = Convert.ToInt32(StringStruct[9]);

                AddCell(CellStruct);

                if (Convert.ToInt32(StringStruct[4]) >= levelIndicator)
                    levelIndicator++;

                if (CellStruct.HasAttribute(CellAttributes.IsStart))
                    StartPoint.Insert(CellStruct.Location.Level, CellStruct.Location);
                if (CellStruct.HasAttribute(CellAttributes.IsFinish))
                    FinishPoint.Insert(CellStruct.Location.Level, CellStruct.Location);
            }
            CellStream.Close();
            LevelCount = levelIndicator;
            dropsCount = new int[LevelCount];
        }

        private void SaveToFile()
        {
            if (isRandom)
                return;

            StreamWriter CellStream = new StreamWriter(MapDirectoryPath + CurrentMapName + ".map", false);
            string CellString;

            foreach(Cell Block in MapBlocks)
            {
                CellString = Block.ID.ToString() + " "
                    + Block.Location.X.ToString() + " "
                    + Block.Location.Y.ToString() + " "
                    + Block.Location.Z.ToString() + " "
                    + Block.Location.Level.ToString() + " "
                    + Block.Type.ToString() + " "
                    + Block.Attribute.ToString() + " "
                    + Block.Option.ToString() + " "
                    + Block.OptionValue.ToString() + " "
                    + Block.ND4.ToString();
                CellStream.WriteLine(CellString);
            }
            CellStream.Close();
        }

        public void FillMapWithUnits()
        {
            foreach (Cell block in MapBlocks)
            {
                if (block.HasAttribute(CellAttributes.HasDrop))
                    // Create Deimos at Ooze Drop Location
                    new Deimos(block.Location);
            }

            // Test-created monsters
            new Phobos(FinishPoint[CurrentLevel]);
        }

        internal void FillMapWithObjects()
        {
            foreach (Cell block in MapBlocks)
            {
                if (block.HasAttribute(CellAttributes.HasDrop))
                {
                    new OozeDrop(block);
                    ++dropsCount[block.Location.Level];
                }

                if (block.HasOption(CellOptions.Portal))
                {
                    Portal portal = new Portal(block);
                    portal.SetDestination(GetCell(block.OptionValue));
                }
            }
        }

        public Cell GetCell(int BlockID)
        {
            Cell MapBlock = new Cell();
            MapBlock.Initialize();

            for (int i = 0; i < BlocksCount; ++i)
                if (((Cell)MapBlocks[i]).ID == BlockID)
                    return (Cell)MapBlocks[i];

            return MapBlock;
        }
        public Cell GetCell(GridLocation BlockLocation)
        {
            Cell MapBlock = new Cell();
            MapBlock.Initialize();// Initialize(MapBlock);
            MapBlock.Location = BlockLocation;

            for (int i = 0; i < MapBlocks.Count; ++i)
                if (((Cell)MapBlocks[i]).Location.Equals(BlockLocation))
                    return (Cell)MapBlocks[i];

            return MapBlock;
        }

        public bool AddCell(Cell NewCell)
        {
            if (GetCell(NewCell.ID).ID != -1)
            {
                ReplaceCell(NewCell);
                return false;
            }

            MapBlocks.Add(NewCell);
            ++BlocksCount;
            CellChanged = true;
            return true;
        }

        public void AddEmptyGrigMap(Cell NewCell)
        {
            RemoveCell(NewCell);
        }

        private bool RemoveCell(Cell RemovedCell)
        {
            MapBlocks.Remove(RemovedCell);
            --BlocksCount;
            return true;
        }
        private bool ReplaceCell(Cell ChangedCell)
        {
            RemoveCell(GetCell(ChangedCell.ID));
            AddCell(ChangedCell);
            return true;
        }

        public string[] GetMapNamesList()
        {
            return MapNameList;
        }

        public GridLocation GetStartPoint()
        {
            GridLocation result = new GridLocation();
            result.Level = CurrentLevel;
            if (CurrentLevel < LevelCount)
                result = StartPoint[CurrentLevel];
            return result;
        }
        public GridLocation GetFinishPoint()
        {
            GridLocation result = new GridLocation();
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
