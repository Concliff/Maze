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
        public int Type;
        public int Attribute;
        public int Option;
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
            Location.Map = 0;
            Attribute = 0;
            Option = 0;
            OptionValue = 0;
            ND4 = 0;
        }
    };

    public struct GPS
    {
        public int X;
        public int Y;
        public int Z;
        public int Map;
    };

    // Location on current Block
    public struct GridGPS
    {
        public GPS Location;
        public int X;
        public int Y;
        public int BlockID;
    };

    // Not yet implemented
    public struct Picture
    {
        public int Type;
        public Image PictureImage;
    };

    public struct Coin
    {
        public int ID;              // Block ID
        public bool Collected;      // Is Coin Collected (Do not show it on map)
    };

    public struct GridMapGraph
    {
        public Graphics Graphic;
        public GridMap Block;
    }

    public class Map
    {
        private ArrayList MapBlocks;        // Array of GridMap block of current map
        private ArrayList Coins;            // Array of Coins on map
        private string[] MapNameList;       // Names of All downloaded maps
        private Image[] Pictures;           // Blocks Images
        public Image StartImage;
        public Image FinishImage;
        public Image DeimosImage;
        public Image CoinImage;
        private GPS StartPoint;
        private GPS FinishPoint;
        private int CellsCount;
        private int BlocksCount;
        private string CurrentMapName;
        private bool GridMapChanged;        // If map changed, it should be rewrited into mapFile

        private string MapDirectoryPath = GlobalConstants.MAPS_PATH;
        private string ImageDirectoryPath = GlobalConstants.IMAGES_PATH;

        public Map()
        {
            CellsCount = BlocksCount = 0;
            LoadMapNameList();
            LoadImages();
            GridMapChanged = false;
        }

        ~Map()
        {
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

        private void LoadImages()
        {
            StreamReader CellsStream = File.OpenText(ImageDirectoryPath + "Cells.dat");
            CellsCount = Convert.ToInt32(CellsStream.ReadLine());
            CellsStream.Close();

            Pictures = new Image[CellsCount];
            for (int i = 0; i < CellsCount; ++i)
                Pictures[i] = Image.FromFile(ImageDirectoryPath + "Cell" + i.ToString() + ".bmp");

            FinishImage = Image.FromFile(ImageDirectoryPath + "Finish.bmp");
            StartImage = Image.FromFile(ImageDirectoryPath + "Start.bmp");
            CoinImage = Image.FromFile(ImageDirectoryPath + "Coin.bmp");
            DeimosImage = Image.FromFile(ImageDirectoryPath + "Monster0.png");
        }

        public void CloseCurrentMap()
        {
            if (GridMapChanged)
                SaveToFile();
        }

        public void LoadMap(int MapIndex)
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
            MapBlocks = new ArrayList();
            Coins = new ArrayList();
            CurrentMapName = MapFileName.Split('.')[0];

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
                GridMapStruct.Location.Map = Convert.ToInt32(StringStruct[4]);
                GridMapStruct.Type = Convert.ToInt32(StringStruct[5]);
                GridMapStruct.Attribute = Convert.ToInt32(StringStruct[6]);
                GridMapStruct.Option = Convert.ToInt32(StringStruct[7]);
                GridMapStruct.OptionValue = Convert.ToInt32(StringStruct[8]);
                GridMapStruct.ND4 = Convert.ToInt32(StringStruct[9]);

                AddGridMap(GridMapStruct);

                if (BinaryOperations.IsBit(GridMapStruct.Attribute, (byte)Attributes.IsStart))
                    StartPoint = GridMapStruct.Location;
                if (BinaryOperations.IsBit(GridMapStruct.Attribute, (byte)Attributes.IsFinish))
                    FinishPoint = GridMapStruct.Location;
                if (BinaryOperations.IsBit(GridMapStruct.Attribute, (byte)Attributes.HasCoin))
                {
                    Coin NewCoin;
                    NewCoin.ID = GridMapStruct.ID;
                    NewCoin.Collected = false;
                    Coins.Add(NewCoin);
                }

            }
            GridMapStream.Close();
        }

        private void SaveToFile()
        {
            StreamWriter GridMapStream = new StreamWriter(MapDirectoryPath + CurrentMapName + ".map", false);
            string GridMapString;
            GridMap Block;

            for (int i = 0; i < BlocksCount; ++i)
            {
                Block = GetGridMapByID(i);
                GridMapString = Block.ID.ToString() + " "
                    + Block.Location.X.ToString() + " "
                    + Block.Location.Y.ToString() + " "
                    + Block.Location.Z.ToString() + " "
                    + Block.Location.Map.ToString() + " "
                    + Block.Type.ToString() + " "
                    + Block.Attribute.ToString() + " "
                    + Block.Option.ToString() + " "
                    + Block.OptionValue.ToString() + " "
                    + Block.ND4.ToString();
                GridMapStream.WriteLine(GridMapString);
            }
            GridMapStream.Close();
        }

        public GridMap GetGridMapByID(int BlockID)
        {
            GridMap MapBlock = new GridMap();
            MapBlock.Initialize();

            for (int i = 0; i < BlocksCount; ++i)
                if (((GridMap)MapBlocks[i]).ID == BlockID)
                    return (GridMap)MapBlocks[i];

            return MapBlock;
        }
        public GridMap GetGridMapByGPS(GPS BlockLocation)
        {
            GridMap MapBlock = new GridMap();
            MapBlock.Initialize();// Initialize(MapBlock);
            MapBlock.Location = BlockLocation;

            for (int i = 0; i < MapBlocks.Count; ++i)
                if (((GridMap)MapBlocks[i]).Location.Equals(BlockLocation))
                    return (GridMap)MapBlocks[i];

            return MapBlock;
        }

        public Image GetPictureByType(int Type)
        {
            return Pictures[Type];
        }

        public int GetBlocksCount()
        {
            return BlocksCount;
        }

        public bool AddGridMap(GridMap NewGridMap)
        {
            if (GetGridMapByID(NewGridMap.ID).ID != -1)
            {
                ReplaceGridMap(NewGridMap);
                return false;
            }

            MapBlocks.Add(NewGridMap);
            ++BlocksCount;
            GridMapChanged = true;
            return true;
        }

        private bool RemoveGridMap(GridMap RemovedGridMap)
        {
            MapBlocks.Remove(RemovedGridMap);
            --BlocksCount;
            return true;
        }
        private bool ReplaceGridMap(GridMap ChangedGridMap)
        {
            RemoveGridMap(GetGridMapByID(ChangedGridMap.ID));
            AddGridMap(ChangedGridMap);
            return true;
        }

        public string[] GetMapNamesList()
        {
            return MapNameList;
        }

        public GPS GetStartPoint() { return StartPoint; }
        public GPS GetFinishPoint() { return FinishPoint; }

        // Outdated method
        private void Initialize(GridMap NewGridMap)
        {
            NewGridMap.ID = -1;
            NewGridMap.Type = 16;
            NewGridMap.Location.X = 0;
            NewGridMap.Location.Y = 0;
            NewGridMap.Location.Z = 0;
            NewGridMap.Location.Map = 0;
            NewGridMap.Attribute = 0;
            NewGridMap.Option = 0;
            NewGridMap.OptionValue = 0;
            NewGridMap.ND4 = 0;
        }

        private bool IsMapExist(string MapName)
        {
            for (int i = 0; i < MapNameList.Count(); ++i)
                if (MapNameList[i].Equals(MapName))
                    return true;

            return false;
        }

        public int GetCoinsCount() { return Coins.Count; }

        public int GetCollectedCoinsCount()
        {
            int CollectedCoinsCount = 0;
            for (int i = 0; i < Coins.Count; ++i)
                if (((Coin)Coins[i]).Collected)
                    ++CollectedCoinsCount;

            return CollectedCoinsCount;
        }

        public void CollectCoin(GridMap Block)
        {
            for (int i = 0; i < Coins.Count; ++i)
                if (((Coin)Coins[i]).ID == Block.ID)
                {
                    Coin NewCoin;
                    NewCoin.ID = Block.ID;
                    NewCoin.Collected = true;
                    Coins.Remove(Coins[i]);
                    Coins.Add(NewCoin);
                    return;
                }
        }

        public bool IsCoinCollected(GridMap Block)
        {
            for (int i = 0; i < Coins.Count; ++i)
                if (((Coin)Coins[i]).ID == Block.ID)
                    return ((Coin)Coins[i]).Collected;
            return false;
        }
    }
}
