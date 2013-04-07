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

        /// <summary>
        /// Gets reference to the single ObjectContainer instance.
        /// </summary>
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

        public static uint GUIDCounter;

        /// <summary>
        /// safety check for changing objects while they are under Update.
        /// </summary>
        private bool isUpdating;
        private List<Object> objects;
        private Stack<Object> objectsToRemove;
        private Stack<Object> objectsToAdd;
        private Stack<uint> releasedGUIDs;

        private ObjectContainer()
        {
            this.objects = new List<Object>();
            this.objectsToRemove = new Stack<Object>();
            this.objectsToAdd = new Stack<Object>();
            this.releasedGUIDs = new Stack<uint>();
            GUIDCounter = 0;
            this.isUpdating = false;
        }

        /// <summary>
        /// Inserts new object into ObjectContainer and defines its GUID.
        /// </summary>
        /// <param name="newObject">Reference to the recentry created object.</param>
        /// <returns>New object GUID</returns>
        public uint CreateObject(Object newObject)
        {
            if (this.isUpdating)
                this.objectsToAdd.Push(newObject);
            else
                this.objects.Add(newObject);

            // Define GUID of the new object
            if (this.releasedGUIDs.Count > 0)
                return this.releasedGUIDs.Pop();
            else
                return ++GUIDCounter;
        }

        public Object GetObjectByGUID(uint GUID)
        {
            for (int i = 0; i < this.objects.Count; ++i)
                if (this.objects[i].GetGUID() == GUID)
                    return this.objects[i];
            return null;
        }

        public List<Object> GetAllObjectsByGPS(GridLocation iGPS)
        {
            return this.objects.FindAll(p => p.Position.Location.Equals(iGPS));
        }

        /// <summary>
        /// Updates all Objects on current Map.
        /// </summary>
        /// <param name="timeP">Timer Tick Time.</param>
        public void UpdateState(int timeP)
        {
            this.isUpdating = true;

            this.objects.AddRange(objectsToAdd);
            this.objectsToAdd.Clear();

            // Update each object or add to Remove Stack
            foreach (Object objectF in this.objects)
            {
                if (objectF.GetObjectState() == ObjectState.Removed)
                {
                    this.objectsToRemove.Push(objectF);
                }
                else
                {
                    objectF.UpdateState(timeP);
                }
            }

            RemoveTaggedObjects();

            this.isUpdating = false;
        }

        /// <summary>
        /// Delete all objects, which is tagged as 'Removed'.
        /// </summary>
        private void RemoveTaggedObjects()
        {
            // Delete all removed objects
            int removeCount = this.objectsToRemove.Count;
            if (removeCount == 0)       // Skip deletion
                return;

            for (int i = 0; i < removeCount; ++i)
            {
                Object objectToRemove = this.objectsToRemove.Pop();
                // Store free GUIDs
                releasedGUIDs.Push(objectToRemove.GetGUID());
                this.objects.Remove(objectToRemove);
            }

            this.objectsToRemove.Clear();
        }

        /// <summary>
        /// Calls Unit.StartMotion for every unit object.
        /// </summary>
        public void StartMotion()
        {
            foreach (Object unit in this.objects)
            {
                if (unit.GetType() == ObjectType.Unit)
                    ((Unit)unit).StartMotion();
            }
        }
        /// <summary>
        /// Removes all Objects except Slug
        /// </summary>
        /// <param name="?"></param>
        public void ClearEnvironment(bool isRemoveSlug)
        {
            foreach (Object objectF in this.objects)
            {
                if (objectF.GetType() == ObjectType.Slug && !isRemoveSlug)
                    continue;

                this.objectsToRemove.Push(objectF);
            }

            RemoveTaggedObjects();
        }
    }
}
