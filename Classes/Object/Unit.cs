﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Maze.Forms;

namespace Maze.Classes
{
    public enum UnitTypes
    {
        Unit,
        Deimos,
        Phobos,
    };

    public enum DeathStates
    {
        Alive,
        Dead,
    };

    public class Unit : Object
    {
        protected bool gridMapReached;
        protected DeathStates deathState;
        protected double SpeedRate;
        protected GPS respawnLocation;
        protected UnitTypes unitType;

        public Unit()
        {
            objectType = ObjectType.Unit;
            unitType = UnitTypes.Unit;
            SpeedRate = 1.0d;
        }

        public UnitTypes GetUnitType() { return unitType; }

        /// <summary>
        /// Change Unit's location by given block count in a given direction
        /// </summary>
        protected void ChangeGPSDueDirection(int BlockPassCount, Directions Direction)
        {
            switch (Direction)
            {
                case Directions.Up: Position.Location.Y -= BlockPassCount; break;
                case Directions.Down: Position.Location.Y += BlockPassCount; break;
                case Directions.Left: Position.Location.X -= BlockPassCount; break;
                case Directions.Right: Position.Location.X += BlockPassCount; break;
            }

            currentGridMap = GetWorldMap().GetGridMap(Position.Location);
            gridMapReached = false;
        }

        protected virtual void ReachedGridMap()
        {
            if (gridMapReached)
                return;

            List<GridObject> objects = GetGridObjectsWithinRange(GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2);
            if (objects.Count != 0)
            {
                foreach (GridObject obj in objects)
                {
                    if (obj.IsActive())
                        obj.Use(this);
                }
            }

            gridMapReached = true;
        }


        public double GetSpeedRate() { return SpeedRate; }
        public void SetSpeedRate(double SpeedRate) { this.SpeedRate = SpeedRate; }
        public bool IsAlive() { return deathState == DeathStates.Alive; }
        public virtual void SetDeathState(DeathStates deathState)
        {
            this.deathState = deathState;
        }

        virtual public void StartMotion() { }
    }
}
