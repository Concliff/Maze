using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
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

        public List<Object> GetAllObjectsByGPS(GridLocation iGPS)
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
        public void ClearEnvironment(bool isRemoveSlug)
        {
            foreach (Object objectF in objects)
            {
                if (objectF.GetType() == ObjectType.Slug && !isRemoveSlug)
                    continue;

                objectsToRemove.Push(objectF);
            }

            RemoveTaggedObjects();
        }
    }
}
