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

    public class ObjectContainer
    {
        private List<Object> objects;
        private Stack<Object> objectsToRemove;
        private Stack<uint> releasedGUIDs;
        public static uint GUIDCounter;

        public ObjectContainer()
        {
            objects = new List<Object>();
            objectsToRemove = new Stack<Object>();
            releasedGUIDs = new Stack<uint>();
            GUIDCounter = 0;
        }

        public uint CreateObject(Object newObject)
        {
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
    public class Object
    {
        protected uint GUID;
        protected ObjectType objectType;
        protected ObjectState objectState;
        protected GridMap currentGridMap;

        public GridGPS Position;

        public Object()
        {
            objectType = ObjectType.Object;
            objectState = ObjectState.Default;

            //Initialize Position by default values
            Position.Location.X = 0;
            Position.Location.Y = 0;
            Position.Location.Z = 0;
            Position.Location.Level = 0;
            Position.X = 0;
            Position.Y = 0;
            Position.BlockID = 0;

            currentGridMap.Initialize();

            GUID = Play.GetObjectContainer().CreateObject(this);
        }

        protected Map GetWorldMap() { return Play.GetWorldMap(); }

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

        public double GetDistance(Object target)
        {
            double distance = Math.Sqrt(Math.Pow(this.Position.X - target.Position.X + (this.Position.Location.X - target.Position.Location.X) * GlobalConstants.GRIDMAP_BLOCK_WIDTH, 2)
                    + Math.Pow(this.Position.Y - target.Position.Y + (this.Position.Location.Y - target.Position.Location.Y) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT, 2));

            return distance;
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
