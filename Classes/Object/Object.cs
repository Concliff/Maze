using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Forms;

namespace Maze.Classes
{
    public enum ObjectTypes
    {
        Object,
        GridObject,
        Unit,
        Slug,
    };

    public enum ObjectStates
    {
        Default,     // Set by Default
        Removed,     // Waiting for deletion from container
    }

    public class PositionEventArgs : EventArgs
    {
        public GPS PrevPosition;
        public GPS NewPosition;

        public PositionEventArgs(GPS prevPosition, GPS newPosition)
        {
            PrevPosition = prevPosition;
            NewPosition = newPosition;
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

        protected ObjectTypes pr_objectType;
        public ObjectTypes ObjectType
        {
            get { return this.pr_objectType; }
            protected set { this.pr_objectType = value; }
        }

        protected ObjectStates pr_objectState;
        public ObjectStates ObjectState
        {
            get { return this.pr_objectState; }
            set { this.pr_objectState = value; }
        }

        protected Cell currentCell;
        protected ModelSize objectSize;

        private GPS pr_position;
        /// <summary>
        /// Gets or sets Object location on the Map.
        /// </summary>
        public GPS Position
        {
            get
            {
                return pr_position;
            }
            set
            {
                if (pr_position.Equals(value))
                    return;

                GPS newPosition = value;
                bool locationChanged = false;

                // if coordinates are outside the cell
                // then change Location
                if (newPosition.X < 0)
                {
                    newPosition.Location.X -= 1;
                    newPosition.X += GlobalConstants.CELL_WIDTH;
                }
                else if (newPosition.X > GlobalConstants.CELL_WIDTH)
                {
                    newPosition.Location.X += 1;
                    newPosition.X -= GlobalConstants.CELL_WIDTH;
                }

                if (newPosition.Y < 0)
                {
                    newPosition.Location.Y -= 1;
                    newPosition.Y += GlobalConstants.CELL_HEIGHT;
                }
                else if (newPosition.Y > GlobalConstants.CELL_HEIGHT)
                {
                    newPosition.Location.Y += 1;
                    newPosition.Y -= GlobalConstants.CELL_HEIGHT;
                }

                // Object moved to other GPS
                if (newPosition.Location != pr_position.Location)
                {
                    // HACK:
                    // Do not enter the nonexistent block
                    // Need revert after NormalizePosition rework
                    Cell newCell = GetWorldMap().GetCell(newPosition.Location);

                    if (newCell.ID != -1)
                    {
                        currentCell = newCell;
                        newPosition.BlockID = currentCell.ID;
                        locationChanged = true;
                    }
                    else
                        newPosition = pr_position;
                }

                // Fix Position, including object bounds and map border
                newPosition = NormalizePosition(newPosition);

                // Apply new Position
                GPS prevPosition = pr_position;
                pr_position = newPosition;

                // Call events
                if (locationChanged && LocationChanged != null)
                    LocationChanged(this, new PositionEventArgs(prevPosition, pr_position));

                if (PositionChanged != null)
                    PositionChanged(this, new PositionEventArgs(prevPosition, pr_position));
            }
        }

        public delegate void PositionHandler(object sender, PositionEventArgs e);
        /// <summary>
        /// Occurs when object changed its Location.
        /// </summary>
        public event PositionHandler PositionChanged;

        /// <summary>
        /// Occurs when object changed its GPS location, i.e moved to other cell
        /// </summary>
        public event PositionHandler LocationChanged;

        public Object()
        {
            ObjectType = ObjectTypes.Object;
            ObjectState = ObjectStates.Default;

            // Initialize Position by default values
            // Seems not needed
            // All uninitialized values is auto-initialized by 0

            currentCell.Initialize();

            GUID = ObjectContainer.Container.CreateObject(this);
        }

        protected Map GetWorldMap() { return Map.WorldMap; }

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
        /// Set Position.X and Position.Y considering object model size and current Cell
        /// </summary>
        protected GPS NormalizePosition(GPS position)
        {
            Cell cell = GetWorldMap().GetCell(position.Location);

            int lowerXBound = GlobalConstants.CELL_BORDER_PX + objectSize.Width / 2;
            int upperXBound = GlobalConstants.CELL_WIDTH - lowerXBound;
            int lowerYBound = GlobalConstants.CELL_BORDER_PX + objectSize.Height / 2;
            int upperYBound = GlobalConstants.CELL_HEIGHT - lowerYBound;

            if (position.X < lowerXBound)
                if (!cell.CanMoveTo(Directions.Left))
                    position.X = lowerXBound;

            if (position.X > upperXBound)
                if (!cell.CanMoveTo(Directions.Right))
                    position.X = upperXBound;

            if (position.Y < lowerYBound)
                if (!cell.CanMoveTo(Directions.Up))
                    position.Y = lowerYBound;

            if (position.Y > upperYBound)
                if (!cell.CanMoveTo(Directions.Down))
                    position.Y = upperYBound;

            return position;
        }

        public uint GetGUID() { return GUID; }

        public virtual void UpdateState(int timeP) { }

    }
}
