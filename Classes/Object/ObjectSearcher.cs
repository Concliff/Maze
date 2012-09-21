using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Forms;

namespace Maze.Classes
{
    class ObjectSearcher
    {
        private static GridGPS DefaultGridGPS(GPS location)
        {
            GridGPS gridGPS;
            gridGPS.Location = location;
            gridGPS.X = GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2;
            gridGPS.Y = GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2;
            gridGPS.BlockID = 0;

            return gridGPS;
        }

        public static List<GridObject> GetGridObjectsWithinRange(Object searcher, int rangeDistance)
        {
            List<GridObject> gridObjects = GetGridObjectsInArea(searcher.Position, rangeDistance);

            // exclude itself
            if (searcher.GetType() == ObjectType.GridObject)
                gridObjects.Remove((GridObject)searcher);

            return gridObjects;
        }

        public static List<GridObject> GetGridObjectsInArea(GPS centralGPS, int radius)
        {
            return GetGridObjectsInArea(DefaultGridGPS(centralGPS), radius);
        }

        public static List<GridObject> GetGridObjectsInArea(GridGPS centralGridGPS, int radius)
        {
            List<Object> objects = new List<Object>();
            objects = GetObjectsInArea(centralGridGPS, radius);

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
            return GetUnitsWithinRange(searcher, rangeDistance, false, false);
        }

        public static List<Unit> GetUnitsWithinRange(Object searcher, int rangeDistance, bool includeInvisible, bool includeDead)
        {
            List<Unit> units = GetUnitsInArea(searcher.Position, rangeDistance, includeInvisible, includeDead);

            // exclude itself
            if (searcher.GetType() == ObjectType.Unit || searcher.GetType() == ObjectType.Slug)
                units.Remove((Unit)searcher);

            return units;
        }

        public static List<Unit> GetUnitsInArea(GPS centralGPS, int radius, bool includeInvisible, bool includeDead)
        {
            return GetUnitsInArea(DefaultGridGPS(centralGPS), radius, includeInvisible, includeDead);
        }

        public static List<Unit> GetUnitsInArea(GridGPS centralGridGPS, int radius, bool includeInvisible, bool includeDead)
        {
            List<Object> objects = new List<Object>();
            objects = GetObjectsInArea(centralGridGPS, radius);

            List<Unit> result = new List<Unit>();

            foreach (Object obj in objects)
            {
                if (obj.GetType() == ObjectType.Unit ||
                    // In face Slug is Unit but with own ObjectType
                    obj.GetType() == ObjectType.Slug)
                {
                    Unit unit = (Unit)obj;
                    if (!unit.IsVisible() && !includeInvisible)
                        continue;
                    if (!unit.IsAlive() && !includeDead)
                        continue;
                    result.Add(unit);
                }
            }

            return result;
        }

        public static List<Object> GetObjectsWithinRange(Object searcher, int rangeDistance)
        {
            List<Object> objects = GetObjectsInArea(searcher.Position, rangeDistance);

            // exclude itself
            objects.Remove(searcher);

            return objects;
        }

        public static List<Object> GetObjectsInArea(GPS centralGPS, int radius)
        {
            return GetObjectsInArea(DefaultGridGPS(centralGPS), radius);
        }

        public static List<Object> GetObjectsInArea(GridGPS centralGridGPS, int radius)
        {
            List<Object> objects = new List<Object>();
            GPS SearchGPS = centralGridGPS.Location;

            // How much grids use for search
            int GridToNorth = (int)Math.Ceiling(Math.Abs(centralGridGPS.Y - radius) * 1d / GlobalConstants.GRIDMAP_BLOCK_HEIGHT);
            int GridToSouth = (int)Math.Floor(Math.Abs(centralGridGPS.Y + radius) * 1d / GlobalConstants.GRIDMAP_BLOCK_HEIGHT);
            int GridToWest = (int)Math.Ceiling(Math.Abs(centralGridGPS.X - radius) * 1d / GlobalConstants.GRIDMAP_BLOCK_WIDTH);
            int GridToEast = (int)Math.Floor(Math.Abs(centralGridGPS.X + radius) * 1d / GlobalConstants.GRIDMAP_BLOCK_WIDTH);

            for (int width = centralGridGPS.Location.X - GridToWest; width <= centralGridGPS.Location.X + GridToEast; ++width)
                for (int height = centralGridGPS.Location.Y - GridToNorth; height <= centralGridGPS.Location.Y + GridToSouth; ++height)
                {
                    SearchGPS.X = width;
                    SearchGPS.Y = height;
                    objects.AddRange(ObjectContainer.Container.GetAllObjectsByGPS(SearchGPS));
                }

            List<Object> result = new List<Object>();

            foreach (Object obj in objects)
            {
                // Calculate actual distance
                if (Math.Sqrt(Math.Pow(centralGridGPS.X - obj.Position.X + (centralGridGPS.Location.X - obj.Position.Location.X) * GlobalConstants.GRIDMAP_BLOCK_WIDTH, 2)
                    + Math.Pow(centralGridGPS.Y - obj.Position.Y + (centralGridGPS.Location.Y - obj.Position.Location.Y) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT, 2)) < radius)
                {
                    result.Add(obj);
                }
            }

            return result;
        }

    }
}
