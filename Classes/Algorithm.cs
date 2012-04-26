using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    class Algorithm
    {
        public struct CellParam
        {
            public int ID; // Cell's number in grid
            public int MID; // Master cell
            public int G; // Moving cost from the start piont to the current one
            public int H; // Moving cost from the current point to the final one
            public int F; // F= G + H

            public void InitializeCell()
            {
                ID = -1;
                MID = -1;
                G = 0;
                H = 0;
                F = 0;
            }
        };

        List<CellParam> OpenList;
        List<CellParam> CloseList;
        List<GridMap> Way;

        GridMap Block;

        public Algorithm()
        {
            OpenList = new List<CellParam>();
            CloseList = new List<CellParam>();
            Way = new List<GridMap>();

            // for test
            CellParam Start = new CellParam();
            Start.InitializeCell();
            Start.ID = 75;
            CellParam Finish = new CellParam();
            Finish.InitializeCell();
            Finish.ID = 73;
            Way = FindWay(Start, Finish);
            //
        }

        void AddOpenList(CellParam Cell)
        {
            OpenList.Add(Cell);
        }

        void AddCloseList(CellParam Cell)
        {
            CloseList.Add(Cell);
        }

        
        CellParam FindCell(GridMap Block)
        {
            CellParam Cell = new CellParam();
            Cell.InitializeCell();
            for (int i = 0; i < OpenList.Count; ++i)
            {
                if (Block.ID == OpenList[i].ID)
                    return OpenList[i];
            }
            return Cell;
        }

        GridMap FindCell(CellParam Cell)
        {
            GridMap Block = new GridMap();
            Map currentMap = new Map();
            Block.Initialize();
            for (int i = 0; i < OpenList.Count; ++i)
            {
                if (Cell.ID == OpenList[i].ID)
                {
                    Block = currentMap.GetGridMap(OpenList[i].ID);
                    break;
                }
            }
            return Block;
        }

        private List<GridMap> FindWay(CellParam StartPoint, CellParam FinalPoint)
        {
            CellParam currentCell = new CellParam();
            currentCell.InitializeCell();
            currentCell = StartPoint;
            AddOpenList(StartPoint);
            
            while(!currentCell.Equals(FinalPoint))
            {
                Block = FindCell(currentCell);
                FindNeighbors(FinalPoint);
                currentCell = GetCellWithMinF();
            }

            Way.Add(FindCell(FinalPoint));

            for (int i = CloseList.Count - 2; i <= 0; --i)
            {
                if (CloseList[i].ID == currentCell.MID)
                {
                    Way.Add(FindCell(CloseList[i]));
                }
            }
            return Way;
        }

        // Find all passable neighbors
        private void FindNeighbors(CellParam FinalPoint)
        {            
            Map currentMap = new Map();

            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (i != 0 && j != 0)
                    {
                        GPS location = new GPS();
                        location.X = Block.Location.X + i;
                        location.Y = Block.Location.Y + j;
                        location.Level = Block.Location.Level;
                        if (currentMap.GetGridMap(location).ID != 0)
                        {
                            CellParam Cell = new CellParam();
                            Cell.ID = currentMap.GetGridMap(location).ID;
                            Cell.MID = Block.ID;
                            if (i == 0 || j == 0)
                                Cell.G = FindCell(Block).G + 10; // Vertical or horizontal movement
                            else
                                Cell.G = FindCell(Block).G + 14; // Diagonal movement

                            Cell.H = (Math.Abs(location.X - currentMap.GetFinishPoint().X) + Math.Abs(location.Y - currentMap.GetFinishPoint().Y)) * 10;
                            Cell.F = Cell.G + Cell.H;

                            OpenList.Add(Cell);
                            OpenList.Remove(FindCell(Block));
                            CloseList.Add(FindCell(Block));
                        }
                    }
                }
            }
        }

        private CellParam GetCellWithMinF()
        {
            CellParam Cell = new CellParam();
            Cell.InitializeCell();
            int F = Cell.F;
            for (int i = 0; i < OpenList.Count; ++i)
            {
                if (OpenList[i].F < F)
                {
                    F = OpenList[i].F;
                    Cell = OpenList[i];
                }
            }
            return Cell;
        }
    }
}
