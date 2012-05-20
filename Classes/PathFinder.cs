using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    class PathFinder
    {
        public struct CellParam
        {
            public int ID;  // Cell's number in grid
            public int MID; // Master cell
            public int G;   // Moving cost from the start piont to the current one
            public int H;   // Moving cost from the current point to the final one
            public int F;   // F= G + H

            public void InitializeCell()
            {
                ID = -1;
                MID = -1;
                G = 0;
                H = 0;
                F = 0;
            }
        };

        private List<CellParam> openList;
        private List<CellParam> closeList;
        public List<GridMap> Path;

        GridMap Block;

        public PathFinder()
        {
            openList = new List<CellParam>();
            closeList = new List<CellParam>();
            Path = new List<GridMap>();

            Map currentMap = GetWorldMap();
        }

        public PathFinder(GridMap StartPoint, GridMap FinishPoint)
            : this()
        {
            GeneratePath(StartPoint, FinishPoint);
        }

        private void AddOpenList(CellParam Cell)
        {
            openList.Add(Cell);
        }

        private void AddCloseList(CellParam Cell)
        {
            closeList.Add(Cell);
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

        public void GeneratePath(GridMap StartPoint, GridMap FinalPoint)
        {
            CellParam currentCell = new CellParam();
            CellParam finalCell = new CellParam();
            List<CellParam> bannedList = new List<CellParam>();
            Path = new List<GridMap>();
            openList = new List<CellParam>();
            closeList = new List<CellParam>();


            currentCell.InitializeCell();
            finalCell.InitializeCell();

            currentCell.ID = StartPoint.ID;
            AddOpenList(currentCell);

            finalCell.ID = FinalPoint.ID;
            AddOpenList(finalCell);

            // return empty Way at points on different levels
            if (StartPoint.Location.Level != FinalPoint.Location.Level)
                return;

            while (currentCell.ID != FinalPoint.ID)
            {
                Block = FindCell(openList, currentCell);
                FindNeighbors(Block, finalCell);

                CellParam CellWithMinF = currentCell;
                if (!SearchInList(closeList, finalCell))
                {
                    CellWithMinF = GetCellWithMinF(bannedList);

                    if (CellWithMinF.Equals(currentCell))
                        bannedList.Add(currentCell);

                    currentCell = GetCellWithMinF(bannedList);

                }
                else
                    break;
            }
            if (closeList.Count > 0)
            {
                currentCell = closeList[closeList.Count - 1];
                Path.Add(FindCell(openList, currentCell));

                for (int i = closeList.Count - 1; i >= 0; --i)   // CloseList[0] == Start Cell
                {
                    if (closeList[i].ID == FindCell(closeList, Path[Path.Count - 1]).MID)
                    {
                        GridMap tempCell = new GridMap();
                        tempCell = FindCell(closeList, closeList[i]);
                        if (!SearchInList(Path, tempCell))
                        {
                            Path.Add(tempCell);
                            i = closeList.Count;
                        }
                    }
                }
            }
        }

        // Find all passable neighbors
        private void FindNeighbors(GridMap Block, CellParam FinalPoint)
        {
            CellParam currentCell = FindCell(openList, Block);

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
                                        locDown.Y++;
                                        GPS locLeft = Block.Location;
                                        locLeft.X--;

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
                                        locUp.Y--;
                                        GPS locRight = Block.Location;
                                        locRight.X++;

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
                                        locUp.Y--;
                                        GPS locLeft = Block.Location;
                                        locLeft.X--;

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
                                        locDown.Y++;
                                        GPS locRight = Block.Location;
                                        locRight.X++;

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

                                Cell.H = (Math.Abs(location.X - FindCell(openList, FinalPoint).Location.X) + Math.Abs(location.Y - FindCell(openList, FinalPoint/*, CurrentMap*/).Location.Y)) * 10;
                                Cell.F = Cell.G + Cell.H;

                                if (!SearchInList(openList, Cell) && !SearchInList(closeList, Cell))
                                {
                                    openList.Add(Cell);

                                    if (!SearchInList(closeList, currentCell))
                                        closeList.Add(currentCell);

                                    if (SearchInList(openList, currentCell))
                                        openList.Remove(currentCell);
                                }

                                if (Cell.ID == FinalPoint.ID)
                                {
                                    if (!SearchInList(closeList, currentCell))
                                        closeList.Add(currentCell);
                                    closeList.Add(Cell);
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
            int i = openList.Count - 1;
            while (i != 0)
            {
                if (!BannedList.Contains(openList[i]))
                {
                    Cell = openList[i];
                    F = Cell.F;
                    break;
                }
                --i;
            }
            for (i = openList.Count - 1; i > 0; --i)    // OpenList[0] == FinishPoint
            {
                if (openList[i].F < F && !BannedList.Contains(openList[i]))
                {
                    F = openList[i].F;
                    Cell = openList[i];
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
