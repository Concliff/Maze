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

            Map currentMap = GetWorldMap();
            
            // for test
            GridMap StartPoint = new GridMap();
            StartPoint.ID = 75;
            StartPoint = GetWorldMap().GetGridMap(StartPoint.ID);
             
            GridMap FinishPoint = new GridMap();
            FinishPoint.ID = 73;
            FinishPoint = GetWorldMap().GetGridMap(FinishPoint.ID);
            Way = FindWay(StartPoint, FinishPoint, currentMap);
            //
        }

        private void AddOpenList(CellParam Cell)
        {
            OpenList.Add(Cell);
        }

        private void AddCloseList(CellParam Cell)
        {
            CloseList.Add(Cell);
        }

        private bool SearchInList(List<CellParam> List, CellParam Element)
        {
            foreach (CellParam el in List)
            {
                if (el.ID == Element.ID)
                    return true;
            }
            return false;
        }

        private bool SearchInList(List<GridMap> List, GridMap Element)
        {
            foreach (GridMap el in List)
            {
                if (el.ID == Element.ID)
                    return true;
            }
            return false;
        }

        private CellParam FindCell(GridMap Block)
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

        private GridMap FindCell(List<CellParam> List, CellParam Cell, Map CurrentMap)
        {
            GridMap Block = new GridMap();
            
            Block.Initialize();
            for (int i = 0; i < List.Count; ++i)
            {
                if (Cell.ID == List[i].ID)
                {
                    Block = CurrentMap.GetGridMap(List[i].ID);
                    break;
                }
            }
            return Block;
        }

        private List<GridMap> FindWay(GridMap StartPoint, GridMap FinalPoint, Map currentMap)
        {
            CellParam currentCell = new CellParam();
            CellParam finalCell = new CellParam();
            currentCell.InitializeCell();
            finalCell.InitializeCell();

            currentCell.ID = StartPoint.ID;
            AddOpenList(currentCell);
            
            finalCell.ID = FinalPoint.ID;
            AddOpenList(finalCell);

            while(currentCell.ID != FinalPoint.ID)
            {
                Block = FindCell(OpenList, currentCell, currentMap);
                FindNeighbors(Block, finalCell, currentMap);
                if (!SearchInList(CloseList, finalCell))
                    currentCell = GetCellWithMinF();
                else
                    break;
            }

            currentCell = CloseList[CloseList.Count - 1];
            Way.Add(FindCell(OpenList, currentCell, currentMap));

            for (int i = CloseList.Count - 1; i > 0; --i)   // CloseList[0] == Start Cell
            {
                foreach(CellParam element in CloseList)
                {
                    GridMap tempCell = new GridMap();
                    tempCell = FindCell(CloseList, element, currentMap);
                    if (element.ID == CloseList[i].MID)
                    {
                        if (!SearchInList(Way, tempCell))
                        {
                            Way.Add(tempCell);
                            break;
                        }
                    }
                }
            }
            return Way;
        }

        // Find all passable neighbors
        private void FindNeighbors(GridMap Block, CellParam FinalPoint, Map CurrentMap)
        {
            CellParam currentCell = FindCell(Block);
            
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (i != 0 || j != 0)
                    {
                        GPS location = new GPS();
                        location.X = Block.Location.X + i;
                        location.Y = Block.Location.Y + j;
                        location.Level = Block.Location.Level;
                        if (CurrentMap.GetGridMap(location).Type != 16)
                        {
                            CellParam Cell = new CellParam();
                            Cell.ID = CurrentMap.GetGridMap(location).ID;
                            Cell.MID = Block.ID;
                            if (i == 0 || j == 0)
                                Cell.G = currentCell.G + 10;    // Vertical or horizontal movement
                            else
                                Cell.G = currentCell.G + 14;    // Diagonal movement

                            Cell.H = (Math.Abs(location.X - FindCell(OpenList, FinalPoint, CurrentMap).Location.X) + Math.Abs(location.Y - FindCell(OpenList, FinalPoint, CurrentMap).Location.Y)) * 10;
                            Cell.F = Cell.G + Cell.H;

                            if(!SearchInList(OpenList, Cell) && !SearchInList(CloseList, Cell))
                                OpenList.Add(Cell);
                            if (!SearchInList(CloseList, currentCell))
                                CloseList.Add(currentCell);
                            if (SearchInList(OpenList, currentCell))
                                OpenList.Remove(currentCell);
                            if (Cell.ID == FinalPoint.ID)
                            {
                                CloseList.Add(Cell);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private CellParam GetCellWithMinF()
        {
            CellParam Cell = new CellParam();
            Cell.InitializeCell();
            int F = OpenList[OpenList.Count-1].F;
            Cell = OpenList[OpenList.Count - 1];
            for (int i = OpenList.Count - 1; i > 1; --i)    // OpenList[0] == FinishPoint
            {
                if (OpenList[i].F < F)
                {
                    F = OpenList[i].F;
                    Cell = OpenList[i];
                }
            }
            return Cell;
        }

        private Map GetWorldMap()
        {
            return Maze.Forms.Play.GetWorldMap();
        }
    }
}
