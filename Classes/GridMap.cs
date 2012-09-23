using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Drawing;

namespace Maze.Classes
{
    public struct GridMap
    {
        public int ID;
        //public int PictureID;
        public GPS Location;
        public uint Type;
        public uint Attribute;
        public uint Option;
        public int OptionValue;
        public int ND4;

        // Define members by specific default vaules
        public void Initialize()
        {
            ID = -1;
            Type = 16;
            Location.X = 0;
            Location.Y = 0;
            Location.Z = 0;
            Location.Level = 0;
            Attribute = 0;
            Option = 0;
            OptionValue = 0;
            ND4 = 0;
        }

        public bool HasAttribute(GridMapAttributes attribute)
        {
            return (Attribute & (uint)attribute) != 0;
        }

        public bool HasOption(GridMapOptions option)
        {
            return (Option & (uint)option) != 0;
        }

        public bool CanMoveTo(Directions direction)
        {
            return (Type & (uint)direction) != 0;
        }
    };

    public struct GPS
    {
        public int X;
        public int Y;
        public int Z;
        public int Level;

        public static bool operator ==(GPS a, GPS b)
        {
            if (a == null || b == null)
                return false;

            return (a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.Level == b.Level);
        }

        public static bool operator !=(GPS a, GPS b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            try
            {
                GPS p = (GPS)obj;
            }
            // Can not convert with any reason
            catch
            {
                return false;
            }

            return this == (GPS)obj;
        }

        public bool Equals(GPS p)
        {
            if (p == null)
                return false;

            return this == p;
        }

        public override int GetHashCode()
        {
            return ((this.X ^ this.Y) + (this.Z ^ this.Level));
        }
    };

    // Location on current Block
    public struct GridGPS
    {
        public GPS Location;
        public int X;
        public int Y;
        public int BlockID;

        // Some constructors for the simplified initialization

        /// <summary>
        /// Initialize with Custom LOcation and X, Y block coords
        /// </summary>
        public GridGPS(GPS Location, int X, int Y)
        {
            this.Location = Location;
            this.X = X;
            this.Y = Y;
            this.BlockID = Map.WorldMap.GetGridMap(this.Location).ID;
        }

        /// <summary>
        /// Initialize by copying Position with custom X and Y coords
        /// </summary>
        public GridGPS(GridGPS Position, int X, int Y)
        {
            this = Position;
            this.X = X;
            this.Y = Y;
        }

        public static bool operator ==(GridGPS a, GridGPS b)
        {
            if (a == null || b == null)
                return false;

            return (a.Location == b.Location && a.X == b.X && a.Y == b.Y && a.BlockID == b.BlockID);
        }

        public static bool operator !=(GridGPS a, GridGPS b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            try
            {
                GridGPS p = (GridGPS)obj;
            }
            // Can not convert with any reason
            catch
            {
                return false;
            }

            return this == (GridGPS)obj;
        }

        public bool Equals(GridGPS p)
        {
            if (p == null)
                return false;

            return this == p;
        }

        public override int GetHashCode()
        {
            return ((this.Location.GetHashCode() ^ this.X ^ this.Y) + this.BlockID);
        }
    };

    // Not yet implemented
    public struct Picture
    {
        public int Type;
        public Image PictureImage;
    };

    public struct GridMapGraph
    {
        public Graphics Graphic;
        public GridMap Block;
    }

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
        private int BlocksCount;
        private string CurrentMapName;
        private bool GridMapChanged;        // If map changed, it should be rewrited into mapFile
        private int currentMapIndex;
        private int currentLevel;
        private int levelCount;
        private string MapDirectoryPath = GlobalConstants.MAPS_PATH;
        private bool isRandom;

        public int DropsRemain
        {
            get
            {
                return dropsCount[GetLevel()];
            }
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
            this.BlocksCount = 0;
        }

        public int GetLevel() { return currentLevel; }
        public int GetMap() { return currentMapIndex; }
        public int GetLevelCount() { return levelCount; }

        public void SetMap(int mapIndex) { SetMap(mapIndex, 0); }

        public void SetLevelCount(int levelCount) { this.levelCount = levelCount; }

        public void SetMap(int mapIndex, int level)
        {
            if (currentMapIndex != mapIndex || MapBlocks == null)
                LoadMap(mapIndex);
            else
                currentMapIndex = mapIndex;

            currentLevel = level;
        }

        public void GenerateRandomMap()
        {
            isRandom = true;
            currentMapIndex = 0;
            currentLevel = 0;

            MazeGenerator generator = new MazeGenerator();
            MapBlocks = generator.Generate(0);
            BlocksCount = MapBlocks.Count;
            StartPoint = new List<GPS>();
            FinishPoint = new List<GPS>();
            StartPoint.Add(generator.StartPoint.Location);
            FinishPoint.Add(generator.FinishPoint.Location);

            SetLevelCount(1);
            dropsCount = new int[GetLevelCount()];
            // Generate Drops
            // Every 15th block should have a drop
            int dropsCounter = MapBlocks.Count / 15;

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
            SetLevelCount(levelIndicator);
            dropsCount = new int[GetLevelCount()];
        }

        private void SaveToFile()
        {
            if (isRandom)
                return;

            StreamWriter GridMapStream = new StreamWriter(MapDirectoryPath + CurrentMapName + ".map", false);
            string GridMapString;
            GridMap Block;

            for (int i = 0; i < BlocksCount; ++i)
            {
                Block = GetGridMap(i);
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
            new Phobos(FinishPoint[GetLevel()]);
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

        public int GetBlocksCount()
        {
            return BlocksCount;
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
            result.Level = currentLevel;
            if (currentLevel < GetLevelCount())
                result = StartPoint[currentLevel];
            return result;
        }
        public GPS GetFinishPoint()
        {
            GPS result = new GPS();
            result.Level = currentLevel;
            if (currentLevel <= FinishPoint.Count)
                result = FinishPoint[currentLevel];
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
            --dropsCount[GetLevel()];
        }
    }
}
