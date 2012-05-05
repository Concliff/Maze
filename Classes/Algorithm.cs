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

        private List<CellParam> OpenList;
        private List<CellParam> CloseList;
        public List<GridMap> Way;

        GridMap Block;

        public Algorithm()
        {
            OpenList = new List<CellParam>();
            CloseList = new List<CellParam>();
            Way = new List<GridMap>();

            Map currentMap = GetWorldMap();
        }

        public Algorithm(GridMap StartPoint, GridMap FinishPoint)
            : this()
        {

            // for test
            //GridMap StartPoint = new GridMap();
            //StartPoint.ID = 75;
            //StartPoint = GetWorldMap().GetGridMap(StartPoint.ID);

            //GridMap FinishPoint = new GridMap();
            //FinishPoint.ID = 73;
            //FinishPoint = GetWorldMap().GetGridMap(FinishPoint.ID);
            Way = FindWay(StartPoint, FinishPoint);
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

        private CellParam FindCell(List<CellParam> List, GridMap Block)
        {
            CellParam Cell = new CellParam();
            Cell.InitializeCell();
            for (int i = 0; i < List.Count; ++i)
            {
                if (Block.ID == List[i].ID)
                    return List[i];
            }
            return Cell;
        }

        private GridMap FindCell(List<CellParam> List, CellParam Cell)
        {
            GridMap Block = new GridMap();
            
            Block.Initialize();
            for (int i = 0; i < List.Count; ++i)
            {
                if (Cell.ID == List[i].ID)
                {
                    Block = GetWorldMap().GetGridMap(List[i].ID);
                    break;
                }
            }
            return Block;
        }

        private CellParam FindInList(List<CellParam> List, int X, int Y)
        {
            CellParam defaultElement = new CellParam();
            defaultElement.InitializeCell();
            foreach (CellParam element in List)
            {
                if (FindCell(List, element).Location.X == X ||
                    FindCell(List, element).Location.Y == Y)
                    return element;
            }
            return defaultElement;
        }

        public List<GridMap> FindWay(GridMap StartPoint, GridMap FinalPoint)
        {
            CellParam currentCell = new CellParam();
            CellParam finalCell = new CellParam();
            List<CellParam> bannedList = new List<CellParam>();
            Way = new List<GridMap>();
            OpenList = new List<CellParam>();
            CloseList = new List<CellParam>();


            currentCell.InitializeCell();
            finalCell.InitializeCell();

            currentCell.ID = StartPoint.ID;
            AddOpenList(currentCell);
            
            finalCell.ID = FinalPoint.ID;
            AddOpenList(finalCell);

            //int circleCount = 0;
            while(currentCell.ID != FinalPoint.ID)
            {
                Block = FindCell(OpenList, currentCell);
                FindNeighbors(Block, finalCell);

                CellParam CellWithMinF = currentCell;
                if (!SearchInList(CloseList, finalCell))
                {
                    CellWithMinF = GetCellWithMinF(bannedList);
                    
                    if(CellWithMinF.Equals(currentCell))
                        bannedList.Add(currentCell);
                    
                    currentCell = GetCellWithMinF(bannedList);
                    
                }
                else
                    break;
            }
            if (CloseList.Count > 0)
            {
                currentCell = CloseList[CloseList.Count - 1];
                Way.Add(FindCell(OpenList, currentCell));

                for (int i = CloseList.Count - 1; i >= 0; --i)   // CloseList[0] == Start Cell
                {
                    if (CloseList[i].ID == FindCell(CloseList, Way[Way.Count - 1]).MID)
                    {
                        GridMap tempCell = new GridMap();
                        tempCell = FindCell(CloseList, CloseList[i]);
                        if (!SearchInList(Way, tempCell))
                        {
                            Way.Add(tempCell);
                            i = CloseList.Count;
                        }
                    }
                }
            }
            return Way;
        }

        // Find all passable neighbors
        private void FindNeighbors(GridMap Block, CellParam FinalPoint)
        {
            CellParam currentCell = FindCell(OpenList, Block);
                       
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
                        if (GetWorldMap().GetGridMap(location).Type != 16)
                        {
                            bool flag = false;

                            switch (i * j)
                            {
                                case -1:
                                    if (i == -1)    // diagonal moving - left and down
                                    {
                                        GPS locDown = Block.Location;
                                        locDown.Y ++;
                                        GPS locLeft = Block.Location;
                                        locLeft.X --;

                                        // Check cell's passability
                                        if ((Block.CanMoveTo(Directions.Down) &&
                                            GetWorldMap().GetGridMap(locDown).CanMoveTo(Directions.Left)) &&
                                            (Block.CanMoveTo(Directions.Left) &&
                                            GetWorldMap().GetGridMap(locLeft).CanMoveTo(Directions.Down)))
                                        {
                                            flag = true;
                                        }
                                    }
                                    else    // diagonal moving - right and up
                                    {
                                        GPS locUp = Block.Location;
                                        locUp.Y --;
                                        GPS locRight = Block.Location;
                                        locRight.X ++;

                                        if ((Block.CanMoveTo(Directions.Up) &&
                                            GetWorldMap().GetGridMap(locUp).CanMoveTo(Directions.Right)) &&
                                            (Block.CanMoveTo(Directions.Right) &&
                                            GetWorldMap().GetGridMap(locRight).CanMoveTo(Directions.Up)))
                                        {
                                            flag = true;
                                        }
                                    }
                                    break;
                                case 1:
                                    if (i == -1)    // diagonal moving - left and up
                                    {
                                        GPS locUp = Block.Location;
                                        locUp.Y --;
                                        GPS locLeft = Block.Location;
                                        locLeft.X --;

                                        if ((Block.CanMoveTo(Directions.Left) &&
                                            GetWorldMap().GetGridMap(locLeft).CanMoveTo(Directions.Up)) &&
                                            (Block.CanMoveTo(Directions.Up) &&
                                            GetWorldMap().GetGridMap(locUp).CanMoveTo(Directions.Left)))
                                        {
                                            flag = true;
                                        }
                                    }
                                    else    // diagonal moving - right and down
                                    {
                                        GPS locDown = Block.Location;
                                        locDown.Y ++;
                                        GPS locRight = Block.Location;
                                        locRight.X ++;

                                        if ((Block.CanMoveTo(Directions.Down) &&
                                            GetWorldMap().GetGridMap(locDown).CanMoveTo(Directions.Right)) &&
                                            (Block.CanMoveTo(Directions.Right) &&
                                            GetWorldMap().GetGridMap(locRight).CanMoveTo(Directions.Down)))
                                        {
                                            flag = true;
                                        }
                                    }
                                    break;
                                case 0:
                                    if (i == 0)    // moving up or down
                                    {
                                        if ((j == -1 && Block.CanMoveTo(Directions.Up)) ||
                                            (j == 1 && Block.CanMoveTo(Directions.Down)))
                                        {
                                            flag = true;
                                        }
                                    }
                                    else    // moving - right or left
                                    {
                                        if ((i == 1 && Block.CanMoveTo(Directions.Right)) ||
                                            (i == -1 && Block.CanMoveTo(Directions.Left)))
                                        {
                                            flag = true;
                                        }
                                    }
                                    break;
                            }

                            if (flag)
                            {
                                CellParam Cell = new CellParam();
                                Cell.ID = GetWorldMap().GetGridMap(location).ID;
                                Cell.MID = Block.ID;
                                if (i == 0 || j == 0)
                                    Cell.G = currentCell.G + 10;    // Vertical or horizontal movement
                                else
                                    Cell.G = currentCell.G + 14;    // Diagonal movement

                                Cell.H = (Math.Abs(location.X - FindCell(OpenList, FinalPoint).Location.X) + Math.Abs(location.Y - FindCell(OpenList, FinalPoint/*, CurrentMap*/).Location.Y)) * 10;
                                Cell.F = Cell.G + Cell.H;

                                if (!SearchInList(OpenList, Cell) && !SearchInList(CloseList, Cell))
                                {
                                    OpenList.Add(Cell);

                                    if (!SearchInList(CloseList, currentCell))
                                        CloseList.Add(currentCell);

                                    if (SearchInList(OpenList, currentCell))
                                        OpenList.Remove(currentCell);
                                }

                                    if (Cell.ID == FinalPoint.ID)
                                    {
                                        if(!SearchInList(CloseList, currentCell))
                                            CloseList.Add(currentCell);
                                        CloseList.Add(Cell);
                                        return;
                                    }
                                
                            }
                        }
                    }
                }
            }
        }

        private CellParam GetCellWithMinF(List<CellParam> BannedList)
        {
            CellParam Cell = new CellParam();
            Cell.InitializeCell();
            int F = Cell.F;
            int i = OpenList.Count - 1;
            while(i!=0)
            {
                if(!BannedList.Contains(OpenList[i]))
                {
                    Cell = OpenList[i];
                    F = Cell.F;
                    break;
                }
                --i;
            }
            for (i = OpenList.Count - 1; i > 0; --i)    // OpenList[0] == FinishPoint
            {
                if (OpenList[i].F < F && !BannedList.Contains(OpenList[i]))
                {
                    F = OpenList[i].F;
                    Cell = OpenList[i];
                }
            }
            return Cell;
        }

        public Map GetWorldMap()
        {
            return Maze.Forms.Play.GetWorldMap();
        }
    }
}
