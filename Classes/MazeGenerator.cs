using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze;
using System.IO;

namespace Maze.Classes
{
    /// <summary>
    /// Represents the rules and methods of the algorithm to create grids for a maze.
    /// </summary>
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
        // CONFIG: Maze parameters
        //
        private const int MAINPATH_LENGTH_MIN = 30;
        private const int MAINPATH_LENGTH_MAX = 40;
        // SEGMENT is section of mainpath before change the direction
        private const int SEGMENT_LENGTH_MIN = 2;   
        private const int SEGMENT_LENGTH_MAX = 4;
        // BRANCH_SEGMENT is section of branch path before change the direction
        private const int BRANCH_SEGMENT_MIN = 3;
        private const int BRANCH_SEGMENT_MAX = 4;
        // Section of mainpath(points count) between every branch
        private const int BRANCH_FREQUENCY = 4;
        // Ouput created maze to a file (for Testing)
        private const bool IS_WRITE_MAZE_FILE = false;

        /// <summary>
        /// Contains the collection of points of the whole maze.
        /// </summary>
        private List<Point> maze;

        /// <summary>
        /// Contains the collection of points of the main Maze path.
        /// </summary>
        private List<Point> mainPath;

        public Cell StartPoint;
        public Cell FinishPoint;

        /// <summary>
        /// Initializes a new instance of the MazeGenerator class.
        /// </summary>
        public MazeGenerator()
        {
            this.maze = new List<Point>();
            this.mainPath = new List<Point>();
        }

        /// <summary>
        /// Creates new random Maze with specified seed value
        /// </summary>
        /// <returns>Collection of the Maze Cells</returns>
        public List<Cell> Generate(ushort seed)
        {
            Random.Int(100);
            Random.Int(100);

            // Keeps maze size
            // Boundaries coords (for table creation)
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

            this.mainPath.Add(start);

            // Priority - in what direction finishPoint(relative of startPoint) will be located
            DecDirections priorityDirection = (DecDirections)(Random.Int(4) + 1);
            DecDirections currentDirection = (DecDirections)(Random.Int(4) + 1);

            //
            // Generate Main Path
            //
            currentPoint = start;

            // Main path length
            int pathLength = Random.Int(MAINPATH_LENGTH_MIN, MAINPATH_LENGTH_MAX);
            int currentLenth = 0;
            while (currentLenth < pathLength)
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
                    ++currentLenth;
                    if (currentLenth > pathLength)
                        break;
                    // Next Point on direction
                    currentPoint = MoveTo(currentPoint, currentDirection);
                    // Recalc maze size
                    bounds.Recheck(currentPoint);

                    this.mainPath.Add(currentPoint);
                }

            }

            finish = currentPoint;

            // Add Main path to all maze points
            this.maze.AddRange(mainPath);

            //
            // Generate Branch Paths
            //
            int branchCount = pathLength / BRANCH_FREQUENCY - 1;
            for (int i = 0; i < branchCount; ++i)
            {
                // Total branches length relative to mainPath length
                int branchLenth = Random.Int(pathLength / 3, pathLength / 2);

                currentPoint = this.mainPath[(i + 1) * BRANCH_FREQUENCY];
                priorityDirection = (DecDirections)(Random.Int(4) + 1);

                int currentBranchLenth = 0;

                while (currentBranchLenth < branchLenth)
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

                    int segmentLenth = Random.Int(BRANCH_SEGMENT_MIN, BRANCH_SEGMENT_MAX);
                    // Reduce the length of long straight segments
                    if (oldDirection == currentDirection)
                        segmentLenth /= 2;
                    for (int j = 0; j < segmentLenth; ++j)
                    {
                        currentPoint = MoveTo(currentPoint, currentDirection);

                        ++currentBranchLenth;
                        if (currentBranchLenth > branchLenth)
                            break;

                        bounds.Recheck(currentPoint);

                        this.maze.Add(currentPoint);
                    }

                }
            }

            //
            // Create Maze Table
            // 0 - Wall (no path)
            // 1 - Path;
            // 2 - Start Point
            // 3 - Finish Point
            int width = bounds.MaxX - bounds.MinX + 1 + 2;
            int height = bounds.MaxY - bounds.MinY + 1 + 2;
            int[,] mainPathMatrix = new int[width, height];

            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                    mainPathMatrix[i, j] = 0;

            int number;
            foreach(Point point in this.maze)
            {
                if (point.Equals(start))
                    number = 2;
                else if (point.Equals(finish))
                    number = 3;
                else
                    number = 1;

                mainPathMatrix[point.X - bounds.MinX + 1, point.Y - bounds.MinY + 1] = number;
            }

            // =====================
            // Test Part:
            // Record Maze into file
            if (IS_WRITE_MAZE_FILE)
            {
                StreamWriter fileStream = new StreamWriter("maze.txt", false);
                StringBuilder record;
                for (int j = 0; j < height; ++j)
                {
                    record = new StringBuilder(String.Empty);

                    for (int i = 0; i < width; ++i)
                    {
                        switch (mainPathMatrix[i, j])
                        {
                            case 1:
                                record.Append(" ");
                                break;
                            case 2:
                                record.Append("S");
                                break;
                            case 3:
                                record.Append("F");
                                break;
                            default:
                                record.Append("█");
                                break;
                        }
                    }
                    fileStream.WriteLine(record.ToString());

                }
                fileStream.Close();
            }
            // ======================


            //
            // FINAL: Map Cells Construct
            // Convert list of Points into list of cell blocks
            //
            List<Cell> mapBlocks = new List<Cell>();
            int idCounter = 1;
            foreach (Point point in this.maze)
            {
                Cell block = new Cell();
                block.Initialize();

                block.ID = idCounter;
                block.Location.X = point.X;
                block.Location.Y = point.Y;
                block.Location.Level = Map.Instance.CurrentLevel;

                block.Type = 0;
                // Up
                if (mainPathMatrix[point.X - bounds.MinX + 1, point.Y - bounds.MinY] > 0)
                    block.Type += (uint)Maze.Classes.Directions.Up;
                // Down
                if (mainPathMatrix[point.X - bounds.MinX + 1, point.Y - bounds.MinY + 2] > 0)
                    block.Type += (uint)Maze.Classes.Directions.Down;
                // Left
                if (mainPathMatrix[point.X - bounds.MinX, point.Y - bounds.MinY + 1] > 0)
                    block.Type += (uint)Maze.Classes.Directions.Left;
                // Right
                if (mainPathMatrix[point.X - bounds.MinX + 2, point.Y - bounds.MinY + 1] > 0)
                    block.Type += (uint)Maze.Classes.Directions.Right;

                // Start Point
                if (point.Equals(start))
                {
                    block.Attribute += (uint)CellAttributes.IsStart;
                    StartPoint = block;
                }
                // Finish Point
                if (point.Equals(finish))
                {
                    block.Attribute += (uint)CellAttributes.IsFinish;
                    FinishPoint = block;
                }

                ++idCounter;

                mapBlocks.Add(block);
            }

            return mapBlocks;
            
        }

        /// <summary>
        /// This method replaces two identical code parts
        /// </summary>
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
