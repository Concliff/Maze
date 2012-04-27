using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Forms;

namespace Maze.Classes
{
    class ObjectSearcher
    {
        public static List<GridObject> GetGridObjectsWithinRange(Object searcher, int rangeDistance)
        {
            List<Object> objects = new List<Object>();
            objects = GetObjectsWithinRange(searcher, rangeDistance);

            List<GridObject> result = new List<GridObject>();

            foreach (Object obj in objects)
            {
                if (obj.GetType() == ObjectType.GridObject)
                {
                    result.Add((GridObject)obj);
                }
            }

            return result;
        }

        public static List<Unit> GetUnitsWithinRange(Object searcher, int rangeDistance)
        {
            List<Object> objects = new List<Object>();
            objects = GetObjectsWithinRange(searcher, rangeDistance);

            List<Unit> result = new List<Unit>();

            foreach (Object obj in objects)
            {
                if (obj.GetType() == ObjectType.Unit)
                {
                    result.Add((Unit)obj);
                }
            }

            return result;
        }

        public static List<Object> GetObjectsWithinRange(Object searcher, int rangeDistance)
        {
            List<Object> objects = new List<Object>();
            GPS SearchGPS = searcher.Position.Location;

            // How much grids use for search
            int GridToNorth = (int)Math.Ceiling(Math.Abs(searcher.Position.Y - rangeDistance) * 1d / GlobalConstants.GRIDMAP_BLOCK_HEIGHT);
            int GridToSouth = (int)Math.Floor(Math.Abs(searcher.Position.Y + rangeDistance) * 1d / GlobalConstants.GRIDMAP_BLOCK_HEIGHT);
            int GridToWest = (int)Math.Ceiling(Math.Abs(searcher.Position.X - rangeDistance) * 1d / GlobalConstants.GRIDMAP_BLOCK_WIDTH);
            int GridToEast = (int)Math.Floor(Math.Abs(searcher.Position.X + rangeDistance) * 1d / GlobalConstants.GRIDMAP_BLOCK_WIDTH);

            for (int width = searcher.Position.Location.X - GridToWest; width <= searcher.Position.Location.X + GridToEast; ++width)
                for (int height = searcher.Position.Location.Y - GridToNorth; height <= searcher.Position.Location.Y + GridToSouth; ++height)
                {
                    SearchGPS.X = width;
                    SearchGPS.Y = height;
                    objects.AddRange(Play.GetObjectContainer().GetAllObjectsByGPS(SearchGPS));
                }

            // exclude itself
            objects.Remove(searcher);

            List<Object> result = new List<Object>();

            foreach (Object obj in objects)
            {
                // Calculate actual distance
                if (Math.Sqrt(Math.Pow(searcher.Position.X - obj.Position.X + (searcher.Position.Location.X - obj.Position.Location.X) * GlobalConstants.GRIDMAP_BLOCK_WIDTH, 2)
                    + Math.Pow(searcher.Position.Y - obj.Position.Y + (searcher.Position.Location.Y - obj.Position.Location.Y) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT, 2)) < rangeDistance)
                {
                    result.Add(obj);
                }
            }

            return result;
        }
    }
}
