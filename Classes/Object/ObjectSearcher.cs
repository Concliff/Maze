using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Forms;

namespace Maze.Classes
{
    /// <summary>
    /// Provides static methods to search the <see cref="Object"/>s and its derived classes in <see cref="ObjectContainer"/>.
    /// </summary>
    public static class ObjectSearcher
    {
        //
        // Within Range Methods:
        //

        public static List<Object> GetObjectsWithinRange(Object searcher, int rangeDistance)
        {
            List<Object> objects = GetObjectsInArea(searcher.Position, rangeDistance);

            // exclude itself
            objects.Remove(searcher);

            return objects;
        }

        public static List<GridObject> GetGridObjectsWithinRange(Object searcher, int rangeDistance)
        {
            List<GridObject> gridObjects = GetGridObjectsInArea(searcher.Position, rangeDistance);

            // exclude itself
            if (searcher.ObjectType == ObjectTypes.GridObject)
                gridObjects.Remove((GridObject)searcher);

            return gridObjects;
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

        //
        // In Area Methods:
        //

        public static List<GridObject> GetGridObjectsInArea(GPS centralPosition, int radius)
        {
            List<GridLocation> locations = GetMatchedLocations(centralPosition, radius);
            List<GridObject> objects = new List<GridObject>();

            foreach (GridLocation location in locations)
            {
                List<GridObject> foundObject = ObjectContainer.Instance.GetGridObjects(location);
                if (foundObject.Count == 0)
                    continue;
                foundObject = FilterInRangeObjects<GridObject>(foundObject, centralPosition, radius);
                if (foundObject.Count > 0)
                    objects.AddRange(foundObject);
            }

            return objects;
        }

        public static List<Unit> GetUnitsInArea(GPS centralPosition, int radius, bool includeInvisible, bool includeDead)
        {
            List<GridLocation> locations = GetMatchedLocations(centralPosition, radius);
            List<Unit> units = new List<Unit>();

            foreach (GridLocation location in locations)
            {
                List<Unit> foundUnits = ObjectContainer.Instance.GetUnits(location);
                if (foundUnits.Count == 0)
                    continue;
                foundUnits = FilterInRangeObjects<Unit>(foundUnits, centralPosition, radius);
                if (foundUnits.Count > 0)
                    if (!includeInvisible && !includeDead)
                    {
                        foreach (Unit unit in foundUnits)
                            if (unit.IsAlive && !includeDead || unit.IsVisible && !includeInvisible)
                                units.Add(unit);
                    }
                    else
                    {
                        units.AddRange(foundUnits);
                    }
            }

            return units;
        }

        public static List<Object> GetObjectsInArea(GPS centralPosition, int radius)
        {
            List<GridLocation> locations = GetMatchedLocations(centralPosition, radius);
            List<Object> objects = new List<Object>();

            foreach (GridLocation location in locations)
            {
                List<Object> foundObjects = ObjectContainer.Instance.GetObjects(location);
                if (foundObjects.Count == 0)
                    continue;
                foundObjects = FilterInRangeObjects<Object>(foundObjects, centralPosition, radius);
                if (foundObjects.Count > 0)
                    objects.AddRange(foundObjects);
            }

            return objects;
        }

        //
        // Helper Methods:
        //

        private static List<GridLocation> GetMatchedLocations(GPS position, int radius)
        {
            List<GridLocation> locations = new List<GridLocation>();
            // How much cells use for search
            int cellsToNorth = (int)Math.Ceiling(Math.Abs(position.Location.Y - radius) * 1d / GlobalConstants.CELL_HEIGHT);
            int cellsToSouth = (int)Math.Floor(Math.Abs(position.Location.Y + radius) * 1d / GlobalConstants.CELL_HEIGHT);
            int cellsToWest = (int)Math.Ceiling(Math.Abs(position.Location.X - radius) * 1d / GlobalConstants.CELL_WIDTH);
            int cellsToEast = (int)Math.Floor(Math.Abs(position.Location.X + radius) * 1d / GlobalConstants.CELL_WIDTH);
            GridLocation searchLocation = position.Location;

            for (int width = position.Location.X - cellsToWest; width <= position.Location.X + cellsToEast; ++width)
                for (int height = position.Location.Y - cellsToNorth; height <= position.Location.Y + cellsToSouth; ++height)
                {
                    searchLocation.X = width;
                    searchLocation.Y = height;
                    locations.Add(searchLocation);
                }

            return locations;
        }

        private static List<T> FilterInRangeObjects<T>(List<T> objects, GPS position, int range) where T : Object
        {
            List<T> matchedObjects = new List<T>();
            foreach (T obj in objects)
                if (obj.Position.GetDistance(position) <= range)
                    matchedObjects.Add(obj);

            return matchedObjects;
        }

    }
}
