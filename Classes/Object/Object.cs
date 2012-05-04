﻿using System;
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

        public ObjectContainer()
        {
            objects = new List<Object>();
            objectsToRemove = new Stack<Object>();
        }

        public int GetNextGuid() { return objects.Count; }

        public int CreateObject(Object newObject)
        {
            objects.Add(newObject);
            return objects.Count;
        }

        public Object GetObjectByGUID(int GUID)
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

            // Delete all removed objects
            int removeCount = objectsToRemove.Count;
            if (removeCount == 0)       // Skip deletion
                return;

            for (int i = 0; i < removeCount; ++i)
            {
                objects.Remove(objectsToRemove.Pop());
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
    }
    public class Object
    {
        protected int GUID;
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

        public int GetGUID() { return GUID; }

        public ObjectState GetObjectState() { return objectState; }
        public void SetObjectState(ObjectState objectState)
        {
            this.objectState = objectState;
        }

        public virtual void UpdateState(int timeP) { }

    }


}