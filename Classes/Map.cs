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

            // Undefined level
            this.pr_CurrentLevel = -1;
        }

        #endregion


        /// <summary>
        /// Collection of the Cell objects on current Map.
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

        private Dictionary<int, GridLocation> startPoints;
        private Dictionary<int, GridLocation> finishPoints;

        /// <summary>
        /// Indicating whether the current Map is randomly generated.
        /// </summary>
        private bool isRandom;

        /// <summary>
        /// Levels count on the current Map.
        /// </summary>
        private int levelsCount;

        /// <summary>
        /// Gets the count of non-collected Ooze drops on current level.
        /// </summary>
        public int DropsRemain
        {
            get
            {
                return this.dropsCount[CurrentLevel];
            }
        }

        /// <summary>
        /// Gets the levels count of the current map
        /// </summary>
        public int LevelCount
        {
            get { return this.levelsCount; }
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

        private int pr_CurrentMap;
        /// <summary>
        /// Gets or sets the current map index.
        /// </summary>
        public int CurrentMap
        {
            get
            {
                return this.pr_CurrentMap;
            }
            set
            {
                this.pr_CurrentMap = value;

                // Random map is loading at CurrentLevel loading.
                if (this.pr_CurrentMap == -1)
                    this.isRandom = true;
                else
                    LoadMap(this.pr_CurrentMap);
            }
        }

        private int pr_CurrentLevel;
        /// <summary>
        /// Gets or sets the index of the current level of the map.
        /// </summary>
        public int CurrentLevel
        {
            get
            {
                return this.pr_CurrentLevel;
            }
            set
            {
                this.pr_CurrentLevel = value;

                LoadLevel();
            }
        }

        /// <summary>
        /// Gets a location of the start point on the current level.
        /// </summary>
        public GridLocation StartLocation
        {
            get
            {
                GridLocation result = new GridLocation();

                // The map was not loaded
                if (this.mapCells == null)
                    return result;

                result.Level = CurrentLevel;
                if (CurrentLevel < LevelCount)
                    result = this.startPoints[CurrentLevel];
                return result;
            }
        }

        /// <summary>
        /// Gets a location of the finish point on the current level.
        /// </summary>
        public GridLocation FinishLocation
        {
            get
            {
                GridLocation result = new GridLocation();

                // The map was not loaded
                if (this.mapCells == null)
                    return result;

                result.Level = CurrentLevel;
                if (CurrentLevel < LevelCount)
                    result = this.finishPoints[CurrentLevel];
                return result;
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
        /// Creates random level with <see cref="MazeGenerator"/> and assigns cells attributes for Start/Finish points, Drops, Portals and so on.
        /// </summary>
        private void GenerateRandomLevel()
        {
            // TODO: Vary level size depending on CurrentLevel increase.

            this.mapCells = new Dictionary<GridLocation, Cell>();
            this.mapCellsIds = new Dictionary<int, GridLocation>();

            // Geneate cell blocks
            MazeGenerator generator = new MazeGenerator();
            List<Cell> generatorCells = generator.Generate(0);
            foreach (Cell cell in generatorCells)
            {
                AddCell(cell);
            }

            this.startPoints = new Dictionary<int, GridLocation>();
            this.finishPoints = new Dictionary<int, GridLocation>();
            this.startPoints.Add(CurrentLevel, generator.StartPoint.Location);
            this.finishPoints.Add(CurrentLevel, generator.FinishPoint.Location);

            this.levelsCount = CurrentLevel + 1;
            this.dropsCount = new int[levelsCount];
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
            // Remove all the existing objects
            ObjectContainer.Instance.ClearEnvironment(true);

            this.isRandom = false;
            this.mapCells = new Dictionary<GridLocation, Cell>();
            this.mapCellsIds = new Dictionary<int, GridLocation>();
            this.startPoints = new Dictionary<int, GridLocation>();
            this.finishPoints = new Dictionary<int,GridLocation>();
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
                    this.startPoints.Add(CellStruct.Location.Level, CellStruct.Location);
                if (CellStruct.HasAttribute(CellAttributes.IsFinish))
                    this.finishPoints.Add(CellStruct.Location.Level, CellStruct.Location);
            }
            CellStream.Close();

            this.levelsCount = levelIndicator;
            dropsCount = new int[LevelCount];
        }

        /// <summary>
        /// Loads (or reloads) all the objects of the current level.
        /// </summary>
        private void LoadLevel()
        {
            // Remove all old objects and units of the previous level
            // Needed when existing game was resetted and started the new one.
            ObjectContainer.Instance.ClearEnvironment(false);

            // Load the map for random game
            if (IsRandom)
                GenerateRandomLevel();

            FillMapWithUnits(); // Add units to map
            FillMapWithObjects(); // Add objects

            // Change the rewpawn location for the Slug
            World.PlayForm.Player.LevelChanged();
        }

        /// <summary>
        /// Places units (Phobos and Deimos) on the map at the appropriate positions.
        /// </summary>
        public void FillMapWithUnits()
        {
            // Random levels alwways have units

            // No units on the first two levels
            if (CurrentLevel < 2 && !IsRandom)
                return;

            foreach (KeyValuePair<GridLocation, Cell> kvp in this.mapCells)
            {
                // Current level only
                if (kvp.Key.Level != CurrentLevel)
                    continue;

                if (kvp.Value.HasAttribute(CellAttributes.HasDrop))
                // Create Deimos at Ooze Drop Location
                {
                    Deimos deimos = new Deimos();
                    deimos.Create(kvp.Key);
                    deimos.StartMotion();
                    // Level 2 has only one Deimos
                    if (CurrentLevel == 2 && !IsRandom)
                        return;
                }
            }

            // No Phoboses until level 8
            if (CurrentLevel < 8 && !IsRandom)
                return;

            // Test-created monsters
            Phobos phobos = new Phobos();
            phobos.Create(this.finishPoints[CurrentLevel]);
            phobos.StartMotion();
        }

        /// <summary>
        /// Places gridObjects (Drops and Portals) on the map at the appropriate positions.
        /// </summary>
        public void FillMapWithObjects()
        {
            foreach (KeyValuePair<GridLocation, Cell> kvp in this.mapCells)
            {
                // Current level only
                if (kvp.Key.Level != CurrentLevel)
                    continue;

                if (kvp.Value.HasAttribute(CellAttributes.HasDrop))
                {
                    OozeDrop drop = new OozeDrop();
                    drop.Create(new GPS(kvp.Key));
                    ++dropsCount[kvp.Key.Level];
                }

                if (kvp.Value.HasOption(CellOptions.Portal))
                {
                    Portal portal = new Portal();
                    portal.Create(new GPS(kvp.Key));
                    portal.SetDestination(GetCell(kvp.Value.OptionValue));
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
