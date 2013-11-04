using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Represents the storage of all existing Objects in the game.
    /// </summary>
    public sealed class ObjectContainer
    {
        #region Singleton Part

        private static ObjectContainer instance;

        /// <summary>
        /// Gets reference to the ObjectContainer instance.
        /// </summary>
        public static ObjectContainer Instance
        {
            get
            {
                if (instance == null)
                    instance = new ObjectContainer();

                return instance;
            }
            private set { ;}
        }

        #endregion

        public static uint GUIDCounter;

        /// <summary>
        /// Safety check for changing objects while they are under Update.
        /// </summary>
        private bool isUpdating;
        private List<Object> objects;
        private Stack<Object> objectsToRemove;
        private Stack<Object> objectsToAdd;
        private Stack<uint> releasedGUIDs;

        /// <summary>
        /// Contains Unit instances by their location.
        /// </summary>
        private Dictionary<GridLocation, List<Unit>> unitContainer;
        /// <summary>
        /// Contains GridObject instances by their location.
        /// </summary>
        private Dictionary<GridLocation, List<GridObject>> gridObjectContainer;

        private ObjectContainer()
        {
            this.objects = new List<Object>();
            this.objectsToRemove = new Stack<Object>();
            this.objectsToAdd = new Stack<Object>();
            this.releasedGUIDs = new Stack<uint>();
            GUIDCounter = 0;
            this.isUpdating = false;

            this.unitContainer = new Dictionary<GridLocation, List<Unit>>();
            this.gridObjectContainer = new Dictionary<GridLocation, List<GridObject>>();
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
            {
                AddNewObject(newObject);
            }

            // Define GUID of the new object
            if (this.releasedGUIDs.Count > 0)
                return this.releasedGUIDs.Pop();
            else
                return ++GUIDCounter;
        }

        /*
        // Temporary Disable this method
        public Object GetObjectByGUID(uint GUID)
        {
            for (int i = 0; i < this.objects.Count; ++i)
                if (this.objects[i].GetGUID() == GUID)
                    return this.objects[i];
            return null;
        }*/

        /// <summary>
        /// Returns all Objects at specified location.
        /// </summary>
        public List<Object> GetObjects(GridLocation location)
        {
            // Get List<Unit> and List<GridObject>
            // Cast them to List<Object>
            // And then add to summary list
            List<Object> objects = new List<Object>();
            objects.AddRange(GetGridObjects(location).Cast<Object>());
            objects.AddRange(GetUnits(location).Cast<Object>());

            return objects;
        }

        /// <summary>
        /// Returns all GridObjects at specified location.
        /// </summary>
        public List<GridObject> GetGridObjects(GridLocation location)
        {
            List<GridObject> objects;
            if (this.gridObjectContainer.TryGetValue(location, out objects))
            {
                return objects;
            }

            return new List<GridObject>();
        }

        /// <summary>
        /// Returns all Units at specified location.
        /// </summary>
        public List<Unit> GetUnits(GridLocation location)
        {
            List<Unit> objects;
            if (this.unitContainer.TryGetValue(location, out objects))
            {
                return objects;
            }

            return new List<Unit>();
        }

        /// <summary>
        /// Updates all Objects on current Map.
        /// </summary>
        /// <param name="timeP">Timer Tick Time.</param>
        public void UpdateState(int timeP)
        {
            this.isUpdating = true;

            foreach (Object obj in this.objectsToAdd)
            {
                AddNewObject(obj);
            }
            this.objectsToAdd.Clear();

            // Update each object or add to RemoveStack
            foreach (Object objectF in this.objects)
            {
                if (objectF.ObjectState == ObjectStates.Removed)
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
                releasedGUIDs.Push(objectToRemove.GUID);

                // Remove from common List
                this.objects.Remove(objectToRemove);
                // Remove from specific container
                if (objectToRemove.ObjectType == ObjectTypes.GridObject)
                    RemoveObject<GridObject>(this.gridObjectContainer, objectToRemove.Position.Location, (GridObject)objectToRemove);
                else
                    RemoveObject<Unit>(this.unitContainer, objectToRemove.Position.Location, (Unit)objectToRemove);
            }

            this.objectsToRemove.Clear();
        }

        /// <summary>
        /// Places object into appropriate container and common objects List
        /// </summary>
        private void AddNewObject(Object newObject)
        {
            // Add GridObject
            if (newObject.ObjectType == ObjectTypes.GridObject)
            {
                AddNewObject<GridObject>(this.gridObjectContainer, (GridObject)newObject);
            }
            // Add Unit
            else if (newObject.ObjectType == ObjectTypes.Slug || newObject.ObjectType == ObjectTypes.Unit)
            {
                AddNewObject<Unit>(this.unitContainer, (Unit)newObject);
            }
            // Do not add any other object types
            else
                return;

            // Place object into common list
            this.objects.Add(newObject);

            newObject.LocationChanged += OnObjectLocationChanged;
        }

        private void AddNewObject<T>(Dictionary<GridLocation, List<T>> container, T newObject) where T : Object
        {
            List<T> objects;
            // Exists at this Location
            if (container.TryGetValue(newObject.Position.Location, out objects))
            {
                objects.Add((T)newObject);
            }
            // Add new
            else
            {
                objects = new List<T>() { newObject };
                container.Add(newObject.Position.Location, objects);
            }
        }

        private void RemoveObject<T>(Dictionary<GridLocation, List<T>> container, GridLocation location, T obj)
        {
            List<T> objects;
            if (container.TryGetValue(location, out objects))
            {
                objects.Remove(obj);
                // List is empty
                // Remove from dictionary
                if (objects.Count == 0)
                    container.Remove(location);
            }
        }



        /// <summary>
        /// Calls Unit.StartMotion for every unit object.
        /// </summary>
        public void StartMotion()
        {
            foreach (KeyValuePair<GridLocation, List<Unit>> kvp in this.unitContainer)
            {
                foreach(Unit unit in kvp.Value)
                    unit.StartMotion();
            }
        }
        /// <summary>
        /// Removes all Objects except Slug
        /// </summary>
        public void ClearEnvironment(bool isRemoveSlug)
        {
            foreach (Object objectF in this.objects)
            {
                if (objectF.ObjectType == ObjectTypes.Slug && !isRemoveSlug)
                    continue;

                this.objectsToRemove.Push(objectF);
            }
        }

        private void OnObjectLocationChanged(object sender, PositionEventArgs e)
        {
            // Relocate object, changing its Key in the container.
            if (((Object)sender).ObjectType == ObjectTypes.GridObject)
            {
                RelocateObject<GridObject>(this.gridObjectContainer, e.PrevPosition.Location, e.NewPosition.Location, (GridObject)sender);
            }
            else if (((Object)sender).ObjectType == ObjectTypes.Slug || ((Object)sender).ObjectType == ObjectTypes.Unit)
            {
                RelocateObject<Unit>(this.unitContainer, e.PrevPosition.Location, e.NewPosition.Location, (Unit)sender);
            }
        }

        /// <summary>
        /// Move Object to another location Key within its container
        /// </summary>
        private void RelocateObject<T>(Dictionary<GridLocation, List<T>> container, GridLocation prevLocation, GridLocation newLocation, T obj) where T : Object
        {
            // Remove from prev location
            RemoveObject<T>(container, prevLocation, obj);

            // Add this object as a new one
            AddNewObject<T>(container, obj);
        }
    }
}
