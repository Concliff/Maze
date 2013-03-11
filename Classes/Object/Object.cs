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
        Default,     // Setted by Default
        Removed,     // Waiting for deletion from container
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
        /// <summary>
        /// Gets or sets Object location on the Map.
        /// </summary>
        public GridGPS Position
        {
            get
            {
                return pr_position;
            }
            set
            {
                if (pr_position.Equals(value))
                    return;

                GridGPS newPosition = value;
                bool locationChanged = false;

                // if coordinates are outside the cell
                // then change Location
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

                // Object moved to other GPS
                if (newPosition.Location != pr_position.Location)
                {
                    // HACK:
                    // Do not enter the nonexistent block
                    // Need revert after NormalizePosition rework
                    GridMap newGridMap = GetWorldMap().GetGridMap(newPosition.Location);

                    if (newGridMap.ID != -1)
                    {
                        currentGridMap = newGridMap;
                        newPosition.BlockID = currentGridMap.ID;
                        locationChanged = true;
                    }
                    else
                        newPosition = pr_position;
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
        /// <summary>
        /// Occurs when object changed its Location.
        /// </summary>
        
        public event PositionHandler PositionChanged;
        /// <summary>
        /// Occurs when object changed its GPS location, i.e moved to other GridMap block
        /// </summary>
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

        /// <summary>
        /// Defines a linear distance to another Object.
        /// </summary>
        public double GetDistance(Object target)
        {
            return this.Position.GetDistance(target.Position);
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
