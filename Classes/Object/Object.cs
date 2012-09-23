using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Forms;

namespace Maze.Classes
{
    public enum ObjectType
    {
        Object,
        GridObject,
        Unit,
        Slug,
    };

    public enum ObjectState
    {
        Default,    // Setted by Default
        Removed,     // Waiting deletion form container
    }

    public sealed class ObjectContainer
    {
        // Singleton
        private static ObjectContainer instance;

        public static ObjectContainer Container
        {
            get
            {
                if (instance == null)
                    instance = new ObjectContainer();

                return instance;
            }
            private set { ;}
        }

        private bool isUpdating;    // safety check for multi access into container
        private List<Object> objects;
        private Stack<Object> objectsToRemove;
        private Stack<Object> objectsToAdd;
        private Stack<uint> releasedGUIDs;
        public static uint GUIDCounter;

        private ObjectContainer()
        {
            objects = new List<Object>();
            objectsToRemove = new Stack<Object>();
            objectsToAdd = new Stack<Object>();
            releasedGUIDs = new Stack<uint>();
            GUIDCounter = 0;
            isUpdating = false;
        }

        public uint CreateObject(Object newObject)
        {
            if (isUpdating)
                objectsToAdd.Push(newObject);
            else
                objects.Add(newObject);

            // Define GUID of the new object
            if (releasedGUIDs.Count > 0)
                return releasedGUIDs.Pop();
            else
                return ++GUIDCounter;
        }

        public Object GetObjectByGUID(uint GUID)
        {
            for (int i = 0; i < objects.Count; ++i)
                if (objects[i].GetGUID() == GUID)
                    return objects[i];
            return null;
        }

        public List<Object> GetAllObjectsByGPS(GPS iGPS)
        {
            return objects.FindAll(p => p.Position.Location.Equals(iGPS));
        }

        public void UpdateState(int timeP)
        {
            isUpdating = true;

            objects.AddRange(objectsToAdd);
            objectsToAdd.Clear();

            // Update each object or add to Remove Stack
            foreach (Object objectF in objects)
            {
                if (objectF.GetObjectState() == ObjectState.Removed)
                {
                    objectsToRemove.Push(objectF);
                }
                else
                {
                    objectF.UpdateState(timeP);
                }
            }

            RemoveTaggedObjects();

            isUpdating = false;
        }

        private void RemoveTaggedObjects()
        {
            // Delete all removed objects
            int removeCount = objectsToRemove.Count;
            if (removeCount == 0)       // Skip deletion
                return;

            for (int i = 0; i < removeCount; ++i)
            {
                Object objectToRemove = objectsToRemove.Pop();
                // Store free GUIDs
                releasedGUIDs.Push(objectToRemove.GetGUID());
                objects.Remove(objectToRemove);
            }

            objectsToRemove.Clear();
        }

        // Motion only for Units
        public void StartMotion()
        {
            foreach (Object unit in objects)
            {
                if (unit.GetType() == ObjectType.Unit)
                    ((Unit)unit).StartMotion();
            }
        }
        /// <summary>
        /// Remove all Object except Slug
        /// </summary>
        /// <param name="?"></param>
        public void ClearEnvironment()
        {
            foreach (Object objectF in objects)
                if (objectF.GetType() != ObjectType.Slug)
                    objectsToRemove.Push(objectF);

            RemoveTaggedObjects();
        }
    }

    public class PositionEventArgs : EventArgs
    {
        public GridGPS NewPosition;

        public PositionEventArgs(GridGPS position)
        {
            NewPosition = position;
        }
    }

    public class Object
    {
        protected struct ModelSize
        {
            public int Width;
            public int Height;
        };

        protected uint GUID;
        protected ObjectType objectType;
        protected ObjectState objectState;
        protected GridMap currentGridMap;
        protected ModelSize objectSize;

        private GridGPS pr_position;

        public GridGPS Position
        {
            get
            {
                return pr_position;
            }
            protected set
            {
                if (pr_position.Equals(value))
                    return;

                GridGPS newPosition = value;
                bool locationChanged = false;

                // Coordinates are out of bound of the cell
                // Change Location

                if (newPosition.X < 0)
                {
                    newPosition.Location.X -= 1;
                    newPosition.X += GlobalConstants.GRIDMAP_BLOCK_WIDTH;
                }
                else if (newPosition.X > GlobalConstants.GRIDMAP_BLOCK_WIDTH)
                {
                    newPosition.Location.X += 1;
                    newPosition.X -= GlobalConstants.GRIDMAP_BLOCK_WIDTH;
                }

                if (newPosition.Y < 0)
                {
                    newPosition.Location.Y -= 1;
                    newPosition.Y += GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                }
                else if (newPosition.Y > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                {
                    newPosition.Location.Y += 1;
                    newPosition.Y -= GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                }

                if (newPosition.Location != pr_position.Location)
                {
                    locationChanged = true;
                    currentGridMap = GetWorldMap().GetGridMap(newPosition.Location);
                    newPosition.BlockID = currentGridMap.ID;
                }

                // Fix Position, including object bounds and map border
                newPosition = NormalizePosition(newPosition);

                // Apply new Position
                pr_position = newPosition;

                // Call events
                if (locationChanged && LocationChanged != null)
                    LocationChanged(this, new PositionEventArgs(pr_position));

                if (PositionChanged != null)
                    PositionChanged(this, new PositionEventArgs(pr_position));


            }
        }

        public delegate void PositionHandler(object sender, PositionEventArgs e);
        public event PositionHandler PositionChanged;
        public event PositionHandler LocationChanged;

        public Object()
        {
            objectType = ObjectType.Object;
            objectState = ObjectState.Default;

            // Initialize Position by default values
            // Seems not needed
            // All uninitialized values is auto-initialized by 0

            currentGridMap.Initialize();

            GUID = ObjectContainer.Container.CreateObject(this);
        }

        protected Map GetWorldMap() { return Map.WorldMap; }

        new public ObjectType GetType() { return objectType; }

        // ObjectSearcher simplified
        protected List<Object> GetObjectsWithinRange(int rangeDistance)
        {
            return ObjectSearcher.GetObjectsWithinRange(this, rangeDistance);
        }
        protected List<GridObject> GetGridObjectsWithinRange(int rangeDistance)
        {
            return ObjectSearcher.GetGridObjectsWithinRange(this, rangeDistance);
        }
        protected List<Unit> GetUnitsWithinRange(int rangeDistance)
        {
            return ObjectSearcher.GetUnitsWithinRange(this, rangeDistance);
        }

        protected List<Unit> GetUnitsWithinRange(int rangeDistance, bool includeInvisible, bool includeDead)
        {
            return ObjectSearcher.GetUnitsWithinRange(this, rangeDistance, includeInvisible, includeDead);
        }

        public double GetDistance(Object target)
        {
            double distance = Math.Sqrt(Math.Pow(this.Position.X - target.Position.X + (this.Position.Location.X - target.Position.Location.X) * GlobalConstants.GRIDMAP_BLOCK_WIDTH, 2)
                    + Math.Pow(this.Position.Y - target.Position.Y + (this.Position.Location.Y - target.Position.Location.Y) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT, 2));

            return distance;
        }

        /// <summary>
        /// Set Position.X and Position.Y considering object model size and current GridMap block
        /// </summary>
        protected GridGPS NormalizePosition(GridGPS position)
        {
            GridMap gridMap = GetWorldMap().GetGridMap(position.Location);

            int lowerXBound = GlobalConstants.GRIDMAP_BORDER_PX + objectSize.Width / 2;
            int upperXBound = GlobalConstants.GRIDMAP_BLOCK_WIDTH - lowerXBound;
            int lowerYBound = GlobalConstants.GRIDMAP_BORDER_PX + objectSize.Height / 2;
            int upperYBound = GlobalConstants.GRIDMAP_BLOCK_HEIGHT - lowerYBound;

            if (position.X < lowerXBound)
                if (!gridMap.CanMoveTo(Directions.Left))
                    position.X = lowerXBound;

            if (position.X > upperXBound)
                if (!gridMap.CanMoveTo(Directions.Right))
                    position.X = upperXBound;

            if (position.Y < lowerYBound)
                if (!gridMap.CanMoveTo(Directions.Up))
                    position.Y = lowerYBound;

            if (position.Y > upperYBound)
                if (!gridMap.CanMoveTo(Directions.Down))
                    position.Y = upperYBound;

            return position;
        }

        public uint GetGUID() { return GUID; }

        public ObjectState GetObjectState() { return objectState; }
        public void SetObjectState(ObjectState objectState)
        {
            this.objectState = objectState;
        }

        public virtual void UpdateState(int timeP) { }

    }
}
