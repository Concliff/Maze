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
        private static Map instance;

        /// <summary>
        /// Gets reference to World Map instance
        /// </summary>
        public static Map Instance
        {
            get
            {
                if (instance == null)
                    instance = new Map();

                return instance;
            }
        }

        /// <summary>
        /// Private constructor for hiding singleton instance
        /// </summary>
        private Map()
        {
            LoadMapNameList();
        }

        #endregion


        /// <summary>
        /// Collection of the Cell object on current Map.
        /// </summary>
        private Dictionary<GridLocation, Cell> mapCells;

        /// <summary>
        /// Collection of the Cells' locations on the Map.
        /// Key = Cell Id.
        /// Value = Location of cell.
        /// </summary>
        private Dictionary<int, GridLocation> mapCellsIds;

        /// <summary>
        /// Count of Ooze Drops at every level of the map. These count values are values at the current time and are changed when a Drop has been collected.
        /// </summary>
        private int[] dropsCount;
        /// <summary>
        /// Names of all loaded maps.
        /// </summary>
        private string[] mapNames;
        private List<GridLocation> StartPoint;
        private List<GridLocation> FinishPoint;
        /// <summary>
        /// Name of the current map.
        /// </summary>
        private string CurrentMapName;
        private int currentMapIndex;
        /// <summary>
        /// Indicating whether the current Map is randomly generated.
        /// </summary>
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

        /// <summary>
        /// Gets the number of the Cells on current Map
        /// </summary>
        public int CellsCount
        {
            get
            {
                if (this.mapCells != null)
                    return this.mapCells.Count;

                return 0;
            }
        }

        /// <summary>
        /// Gets a value endicating whether the current Map is randomly generated.
        /// </summary>
        public bool IsRandom
        {
            get
            {
                return this.isRandom;
            }
        }

        /// <summary>
        /// Loads Names of maps with scanning for map files in the game directory.
        /// </summary>
        private void LoadMapNameList()
        {
            DirectoryInfo mapDirectory = new DirectoryInfo(GlobalConstants.MAPS_PATH);
            FileInfo[] mapFiles = mapDirectory.GetFiles();
            List<string> mapNames = new List<string>();

            foreach (FileInfo fi in mapFiles)
            {
                if (fi.Extension == ".map")
                    mapNames.Add(Path.GetFileNameWithoutExtension(fi.FullName));
            }

            this.mapNames = mapNames.ToArray();
        }

        /// <summary>
        /// Returns current Map index.
        /// </summary>
        public int GetMap() { return currentMapIndex; }

        public void SetMap(int mapIndex) { SetMap(mapIndex, 0); }

        public void SetMap(int mapIndex, int level)
        {
            if (currentMapIndex != mapIndex || this.mapCells == null)
                LoadMap(mapIndex);

            currentMapIndex = mapIndex;
            CurrentLevel = level;
        }

        /// <summary>
        /// Creates random Map with <see cref="MazeGenerator"/> and assigns cells attributes for Start/Finish points, Drops, Portals and so on.
        /// </summary>
        public void GenerateRandomMap()
        {
            isRandom = true;  // Mark current Map as RandomMap
            currentMapIndex = 0;
            CurrentLevel = 0;
            this.mapCells = new Dictionary<GridLocation, Cell>();
            this.mapCellsIds = new Dictionary<int, GridLocation>();

            // Geneate cell blocks
            MazeGenerator generator = new MazeGenerator();
            List<Cell> generatorCells = generator.Generate(0);
            foreach (Cell cell in generatorCells)
            {
                AddCell(cell);
            }

            StartPoint = new List<GridLocation>();
            FinishPoint = new List<GridLocation>();
            StartPoint.Add(generator.StartPoint.Location);
            FinishPoint.Add(generator.FinishPoint.Location);

            LevelCount = 1;
            dropsCount = new int[LevelCount];
            // Generate Drops Count
            // Every 15th block should have a drop
            int dropsCounter = CellsCount / 15;

            // Generate Drops Location
            int currentDropsCount = 0;
            while (currentDropsCount < dropsCounter)
            {
                Cell cell = GetCell(Random.Int(CellsCount));
                // Cell has not been found
                if (cell.ID == -1)
                    continue;

                if (cell.HasAttribute(CellAttributes.HasDrop) ||
                    cell.HasAttribute(CellAttributes.IsStart) ||
                    cell.HasAttribute(CellAttributes.IsFinish))
                    continue;

                cell.Attribute += (uint)CellAttributes.HasDrop;
                ++currentDropsCount;
                AddCell(cell, true);
            }

            // Generate Portals
            // Every 50th block
            int portalCounter = CellsCount / 50;
            int portalCount = 0;

            while (portalCount < portalCounter)
            {
                Cell portalBlock = GetCell(Random.Int(CellsCount));
                Cell destinationBlock = GetCell(Random.Int(CellsCount));

                // Cells have not been found
                if (portalBlock.ID == -1 || destinationBlock.ID == -1)
                    continue;

                if (portalBlock.ID == destinationBlock.ID ||
                    portalBlock.HasAttribute(CellAttributes.HasDrop) ||
                    portalBlock.HasAttribute(CellAttributes.IsStart) ||
                    portalBlock.HasAttribute(CellAttributes.IsFinish) ||
                    destinationBlock.HasAttribute(CellAttributes.IsStart))
                    continue;

                portalBlock.Option += (uint)CellOptions.Portal;
                portalBlock.OptionValue = destinationBlock.ID;
                ++portalCount;
                AddCell(portalBlock, true);
            }
        }

        /// <summary>
        /// Loads Map Cells for specific map.
        /// </summary>
        /// <param name="mapIndex">An index in the map names collection</param>
        private void LoadMap(int mapIndex)
        {
            this.isRandom = false;
            this.mapCells = new Dictionary<GridLocation, Cell>();
            this.mapCellsIds = new Dictionary<int, GridLocation>();
            StartPoint = new List<GridLocation>();
            FinishPoint = new List<GridLocation>();
            CurrentMapName = this.mapNames[mapIndex];
            int levelIndicator = 0;

            StreamReader CellStream = File.OpenText(GlobalConstants.MAPS_PATH + this.mapNames[mapIndex] + ".map");
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

        /// <summary>
        /// Places units (Phobos and Deimos) on the map at the appropriate positions.
        /// </summary>
        public void FillMapWithUnits()
        {
            foreach (KeyValuePair<GridLocation, Cell> cell in this.mapCells)
            {
                if (cell.Value.HasAttribute(CellAttributes.HasDrop))
                // Create Deimos at Ooze Drop Location
                {
                    Deimos deimos = new Deimos();
                    deimos.Create(cell.Key);
                }
            }

            // Test-created monsters
            Phobos phobos = new Phobos();
            phobos.Create(FinishPoint[CurrentLevel]);
        }

        /// <summary>
        /// Places gridObjects (Drops and Portals) on the map at the appropriate positions.
        /// </summary>
        public void FillMapWithObjects()
        {
            foreach (KeyValuePair<GridLocation, Cell> cell in this.mapCells)
            {
                if (cell.Value.HasAttribute(CellAttributes.HasDrop))
                {
                    OozeDrop drop = new OozeDrop();
                    drop.Create(new GPS(cell.Key));
                    ++dropsCount[cell.Key.Level];
                }

                if (cell.Value.HasOption(CellOptions.Portal))
                {
                    Portal portal = new Portal();
                    portal.Create(new GPS(cell.Key));
                    portal.SetDestination(GetCell(cell.Value.OptionValue));
                }
            }
        }

        /// <summary>
        /// Gets a Cell of the map with the specified ID.
        /// </summary>
        /// <param name="cellId">ID of the Cell.</param>
        /// <returns>Found Cell or default Cell value when no cells were found.</returns>
        public Cell GetCell(int cellId)
        {
            if (this.mapCellsIds.ContainsKey(cellId) && this.mapCells.ContainsKey(this.mapCellsIds[cellId]))
                return this.mapCells[this.mapCellsIds[cellId]];

            // return default cell
            Cell defaultCell = new Cell();
            defaultCell.Initialize();
            return defaultCell;
        }

        /// <summary>
        /// Gets a Cell of the map with the specified Location.
        /// </summary>
        /// <param name="location">Location where the Cell is expected to be.</param>
        /// <returns>Found Cell or default Cell value when no cells were found.</returns>
        public Cell GetCell(GridLocation location)
        {
            if (this.mapCells.ContainsKey(location))
                return this.mapCells[location];

            // return default cell
            Cell cell = new Cell();
            cell.Initialize();
            return cell;
        }

        /// <summary>
        /// Gets a Cell of the map with the specified Position.
        /// </summary>
        /// <param name="location">Location where the Cell is expected to be.</param>
        /// <returns>Found Cell or default Cell value when no cells were found.</returns>
        public Cell GetCell(GPS position)
        {
            return GetCell(position.Location);
        }

        private bool AddCell(Cell newCell, bool isReplaceOnExist = false)
        {
            // A Cell with the same id or at the same location exists
            if (this.mapCellsIds.ContainsKey(newCell.ID) || this.mapCells.ContainsKey(newCell.Location))
            {
                if (!isReplaceOnExist)
                    return false;

                this.mapCells.Remove(newCell.Location);
                this.mapCellsIds.Remove(newCell.ID);
            }

            this.mapCells.Add(newCell.Location, newCell);
            this.mapCellsIds.Add(newCell.ID, newCell.Location);
            return true;
        }

        /// <summary>
        /// Returns the Start Point on the current level.
        /// </summary>
        public GridLocation GetStartPoint()
        {
            GridLocation result = new GridLocation();
            result.Level = CurrentLevel;
            if (CurrentLevel < LevelCount)
                result = StartPoint[CurrentLevel];
            return result;
        }

        /// <summary>
        /// Returns the Finish Point on the current level.
        /// </summary>
        public GridLocation GetFinishPoint()
        {
            GridLocation result = new GridLocation();
            result.Level = CurrentLevel;
            if (CurrentLevel <= FinishPoint.Count)
                result = FinishPoint[CurrentLevel];
            return result;
        }

        public string[] GetMapNames()
        {
            if (this.mapNames == null || this.mapNames.Length == 0)
                return null;

            string[] result = new string[this.mapNames.Length];
            Array.Copy(this.mapNames, result, this.mapNames.Length);
            return result;
        }

        /// <summary>
        /// Determies if the specifed Map name is exists in the list of loaded maps.
        /// </summary>
        /// <param name="MapName">Name of the map to check</param>
        /// <returns><c>true</c> if the map file with the specified name was loaded; otherwise, <c>false</c>.</returns>
        private bool IsMapExist(string MapName)
        {
            for (int i = 0; i < this.mapNames.Count(); ++i)
                if (this.mapNames[i].Equals(MapName))
                    return true;

            return false;
        }

        /// <summary>
        /// Processes picking up the Drop on the map by the <see cref="Slug"/>.
        /// </summary>
        /// <param name="drop">The reference to an Drop object that has been collected.</param>
        public void CollectDrop(OozeDrop drop)
        {
            --dropsCount[CurrentLevel];
        }
    }
}
