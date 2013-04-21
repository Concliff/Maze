using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Forms;

namespace Maze.Classes
{
    class ObjectSearcher
    {
        private static GPS DefaultGPS(GridLocation location)
        {
            GPS position = new GPS(location, GlobalConstants.CELL_WIDTH / 2, GlobalConstants.CELL_HEIGHT);
            position.BlockID = 0;

            return position;
        }

        public static List<GridObject> GetGridObjectsWithinRange(Object searcher, int rangeDistance)
        {
            List<GridObject> gridObjects = GetGridObjectsInArea(searcher.Position, rangeDistance);

            // exclude itself
            if (searcher.ObjectType == ObjectTypes.GridObject)
                gridObjects.Remove((GridObject)searcher);

            return gridObjects;
        }

        public static List<GridObject> GetGridObjectsInArea(GridLocation centralLocation, int radius)
        {
            return GetGridObjectsInArea(DefaultGPS(centralLocation), radius);
        }

        public static List<GridObject> GetGridObjectsInArea(GPS centralPosition, int radius)
        {
            List<Object> objects = new List<Object>();
            objects = GetObjectsInArea(centralPosition, radius);

            List<GridObject> result = new List<GridObject>();

            foreach (Object obj in objects)
            {
                if (obj.ObjectType == ObjectTypes.GridObject)
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
            if (searcher.ObjectType == ObjectTypes.Unit || searcher.ObjectType == ObjectTypes.Slug)
                units.Remove((Unit)searcher);

            return units;
        }

        public static List<Unit> GetUnitsInArea(GridLocation centralLocation, int radius, bool includeInvisible, bool includeDead)
        {
            return GetUnitsInArea(DefaultGPS(centralLocation), radius, includeInvisible, includeDead);
        }

        public static List<Unit> GetUnitsInArea(GPS centralPosition, int radius, bool includeInvisible, bool includeDead)
        {
            List<Object> objects = new List<Object>();
            objects = GetObjectsInArea(centralPosition, radius);

            List<Unit> result = new List<Unit>();

            foreach (Object obj in objects)
            {
                if (obj.ObjectType == ObjectTypes.Unit ||
                    // In face Slug is Unit but with own ObjectType
                    obj.ObjectType == ObjectTypes.Slug)
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

        public static List<Object> GetObjectsInArea(GridLocation centralLocation, int radius)
        {
            return GetObjectsInArea(DefaultGPS(centralLocation), radius);
        }

        public static List<Object> GetObjectsInArea(GPS centralPosition, int radius)
        {
            List<Object> objects = new List<Object>();
            GridLocation searchLocation = centralPosition.Location;

            // How much cells use for search
            int cellsToNorth = (int)Math.Ceiling(Math.Abs(centralPosition.Y - radius) * 1d / GlobalConstants.CELL_HEIGHT);
            int cellsToSouth = (int)Math.Floor(Math.Abs(centralPosition.Y + radius) * 1d / GlobalConstants.CELL_HEIGHT);
            int cellsToWest = (int)Math.Ceiling(Math.Abs(centralPosition.X - radius) * 1d / GlobalConstants.CELL_WIDTH);
            int cellsToEast = (int)Math.Floor(Math.Abs(centralPosition.X + radius) * 1d / GlobalConstants.CELL_WIDTH);

            for (int width = centralPosition.Location.X - cellsToWest; width <= centralPosition.Location.X + cellsToEast; ++width)
                for (int height = centralPosition.Location.Y - cellsToNorth; height <= centralPosition.Location.Y + cellsToSouth; ++height)
                {
                    searchLocation.X = width;
                    searchLocation.Y = height;
                    objects.AddRange(ObjectContainer.Container.GetAllObjectsByGPS(searchLocation));
                }

            List<Object> result = new List<Object>();

            foreach (Object obj in objects)
            {
                // Calculate actual distance
                if (Math.Sqrt(Math.Pow(centralPosition.X - obj.Position.X + (centralPosition.Location.X - obj.Position.Location.X) * GlobalConstants.CELL_WIDTH, 2)
                    + Math.Pow(centralPosition.Y - obj.Position.Y + (centralPosition.Location.Y - obj.Position.Location.Y) * GlobalConstants.CELL_HEIGHT, 2)) < radius)
                {
                    result.Add(obj);
                }
            }

            return result;
        }

    }
}
