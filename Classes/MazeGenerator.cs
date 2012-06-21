using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze;
using System.IO;

namespace Maze.Classes
{
    public class MazeGenerator
    {
        struct Point
        {
            public int X;
            public int Y;
        };

        enum DecDirections : byte
        {
            Up = 1,
            Down = 2,
            Left = 3,
            Right = 4,
        };

        struct CoordBounds
        {
            public int MinX;
            public int MaxX;
            public int MinY;
            public int MaxY;

            public void Recheck(Point point)
            {
                if (point.X > MaxX)
                    MaxX = point.X;
                else if (point.X < MinX)
                    MinX = point.X;

                if (point.Y > MaxY)
                    MaxY = point.Y;
                else if (point.Y < MinY)
                    MinY = point.Y;
            }
        };

        //
        // Maze parameters
        //
        private const int MAINPATH_LENGTH_MIN = 40;
        private const int MAINPATH_LENGTH_MAX = 50;
        // SEGMENT is section of mainpath before change the direction
        private const int SEGMENT_LENGTH_MIN = 2;   
        private const int SEGMENT_LENGTH_MAX = 4;
        // BRANCH_SEGMENT is section of branch path before change the direction
        private const int BRANCH_SEGMENT_MIN = 3;
        private const int BRANCH_SEGMENT_MAX = 4;
        private const int BRANCH_FREQUENCY = 4;     // Section of mainpath between every branch


        private List<Point> maze;
        private List<Point> mainPath;
        private CoordBounds bounds;     // Boundaries coords (for table creation)

        public GridMap StartPoint;
        public GridMap FinishPoint;

        public MazeGenerator()
        {
            maze = new List<Point>();
            mainPath = new List<Point>();

            bounds.MaxX = 0;
            bounds.MaxY = 0;
            bounds.MinX = 0;
            bounds.MinY = 0;
        }

        public List<GridMap> Generate(ushort seed)
        {
            Random.Int(100);
            Random.Int(100);

            CoordBounds bounds;
            bounds.MaxX = 0;
            bounds.MaxY = 0;
            bounds.MinX = 0;
            bounds.MinY = 0;

            Point start;
            Point finish;
            Point currentPoint;

            start.X = 0;
            start.Y = 0;

            mainPath.Add(start);

            // Priority - in what direction finishPoint(relative of startPoint) will be located
            DecDirections priorityDirection = (DecDirections)(Random.Int(4) + 1);

            DecDirections currentDirection = (DecDirections)(Random.Int(4) + 1);
            //
            // Generate Main Path
            //
            currentPoint = start;

            int pathLenth = Random.Int(MAINPATH_LENGTH_MIN, MAINPATH_LENGTH_MAX);
            int currentLenth = 0;
            while (currentLenth < pathLenth)
            {
                DecDirections oldDirection = currentDirection;
                do
                {
                    // Choose new direction (but NOT oppoite of current one)
                    // 60% chance - priority
                    // 30% chance - left or rights side of current
                    // 10% chance - opposite of priority direction
                    int rnd = Random.Int(100);
                    if (rnd >= 40)
                    {
                        currentDirection = priorityDirection;
                    }
                    else if (rnd > 10 && rnd < 40)
                    {
                        currentDirection = GetNeighborDirection(priorityDirection, Random.Int(100) > 50 ? 1 : 2);
                    }
                    
                    else
                    {
                        currentDirection = GetOppositeDirection(priorityDirection);
                    }
                } while (GetOppositeDirection(oldDirection) == currentDirection);

                // Generate segment
                int segmentLenth = Random.Int(SEGMENT_LENGTH_MIN, SEGMENT_LENGTH_MAX);
                for (int j = 0; j < segmentLenth; ++j)
                {
                    currentPoint = MoveTo(currentPoint, currentDirection);

                    ++currentLenth;
                    if (currentLenth > pathLenth)
                        break;

                    bounds.Recheck(currentPoint);

                    mainPath.Add(currentPoint);
                }

            }

            finish = currentPoint;

            //
            // Generate Branch Paths
            //
            int branchCount = pathLenth / BRANCH_FREQUENCY - 1;
            for (int i = 0; i < branchCount; ++i)
            {
                // Total branch relative to mainPath length
                int branchLenth = Random.Int(pathLenth / 3, pathLenth / 2);

                currentPoint = mainPath[(i + 1) * BRANCH_FREQUENCY];
                priorityDirection = (DecDirections)(Random.Int(4) + 1);

                int currentBranchLenth = 0;

                while (currentBranchLenth < branchLenth)
                {
                    DecDirections oldDirection = currentDirection;
                    do
                    {
                        int rnd = Random.Int(100);
                        if (rnd >= 40)
                        {
                            currentDirection = priorityDirection;
                        }
                        else if (rnd > 10 && rnd < 40)
                        {
                            currentDirection = GetNeighborDirection(priorityDirection, Random.Int(100) > 50 ? 1 : 2);
                        }
                        else
                        {
                            currentDirection = GetOppositeDirection(priorityDirection);
                        }
                    } while (GetOppositeDirection(oldDirection) == currentDirection);

                    int segmentLenth = Random.Int(BRANCH_SEGMENT_MIN, BRANCH_SEGMENT_MAX);
                    for (int j = 0; j < segmentLenth; ++j)
                    {
                        currentPoint = MoveTo(currentPoint, currentDirection);

                        ++currentBranchLenth;
                        if (currentBranchLenth > branchLenth)
                            break;

                        bounds.Recheck(currentPoint);

                        maze.Add(currentPoint);
                    }

                }
            }

            maze.AddRange(mainPath);

            //
            // Create Maze Table
            // 1 - Path; 0 - Wall (no path)
            int width = bounds.MaxX - bounds.MinX + 1 + 2;
            int height = bounds.MaxY - bounds.MinY + 1 + 2;
            int[,] mainPathMatrix = new int[width, height];

            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                    mainPathMatrix[i, j] = 0;

            for (int i = 0; i < maze.Count; ++i)
            {
                //mainPathMatrix[mainPath[i].X - bounds.MinX + 1, mainPath[i].Y - bounds.MinY + 1] = 1;.
                mainPathMatrix[maze[i].X - bounds.MinX + 1, maze[i].Y - bounds.MinY + 1] = 1;

            }


            // =====================
            // Test Part:
            // Record Maze into file
            
            StreamWriter fileStream = new StreamWriter("1.txt", false);

            for (int j = 0; j < height; ++j)
            {
                String record = "";
                
                    for (int i = 0; i < width; ++i)
                {
                    record += mainPathMatrix[i, j] == 0 ? "8" : " ";
                }
                fileStream.WriteLine(record);

            }
            fileStream.Close();
            // ======================


            // Convert list of Points into list of GridMap blocks
            List<GridMap> mapBlocks = new List<GridMap>();
            int idCounter = 1;
            foreach (Point point in maze)
            {
                GridMap block = new GridMap();
                block.Initialize();

                block.ID = idCounter;
                block.Location.X = point.X;
                block.Location.Y = point.Y;

                block.Type = 0;
                // Up
                if (mainPathMatrix[point.X - bounds.MinX + 1, point.Y - bounds.MinY] == 1)
                    block.Type += (uint)Maze.Classes.Directions.Up;
                // Down
                if (mainPathMatrix[point.X - bounds.MinX + 1, point.Y - bounds.MinY + 2] == 1)
                    block.Type += (uint)Maze.Classes.Directions.Down;
                // Left
                if (mainPathMatrix[point.X - bounds.MinX, point.Y - bounds.MinY + 1] == 1)
                    block.Type += (uint)Maze.Classes.Directions.Left;
                // Right
                if (mainPathMatrix[point.X - bounds.MinX + 2, point.Y - bounds.MinY + 1] == 1)
                    block.Type += (uint)Maze.Classes.Directions.Right;

                if (point.Equals(start))
                {
                    block.Attribute += (uint)GridMapAttributes.IsStart;
                    StartPoint = block;
                }
                if (point.Equals(finish))
                {
                    block.Attribute += (uint)GridMapAttributes.IsFinish;
                    FinishPoint = block;
                }

                ++idCounter;

                mapBlocks.Add(block);
            }

            return mapBlocks;
            
        }

        // This method replaces two identical code parts
        private Point MoveTo(Point currentPoint, DecDirections direction)
        {
            Point result = currentPoint;
            switch (direction)
            {
                case DecDirections.Down:
                    result.Y++;
                    break;
                case DecDirections.Up:
                    result.Y--;
                    break;
                case DecDirections.Left:
                    result.X--;
                    break;
                case DecDirections.Right:
                    result.X++;
                    break;
            }

            return result;
        }

        private DecDirections GetOppositeDirection(DecDirections direction)
        {
            switch (direction)
            {
                case DecDirections.Right:
                    return DecDirections.Left;
                case DecDirections.Left:
                    return DecDirections.Right;
                case DecDirections.Up:
                    return DecDirections.Down;
                case DecDirections.Down:
                    return DecDirections.Up;
            }

            return DecDirections.Left;
        }

        private DecDirections GetNeighborDirection(DecDirections direction, int number)
        {
            switch (direction)
            {
                case DecDirections.Right:
                    if (number == 1)
                        return DecDirections.Up;
                    else
                        return DecDirections.Down;
                case DecDirections.Left:
                    if (number == 1)
                        return DecDirections.Up;
                    else
                        return DecDirections.Down;
                case DecDirections.Up:
                    if (number == 1)
                        return DecDirections.Left;
                    else
                        return DecDirections.Right;
                case DecDirections.Down:
                    if (number == 1)
                        return DecDirections.Left;
                    else
                        return DecDirections.Right;
            }
            return DecDirections.Left;
        }
    }
}
