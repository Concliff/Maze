using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Forms;

namespace Maze.Classes
{
    /// <summary>
    /// Specifies the type of object (or derived class) that an instance of the <see cref="Object"/> class represents.
    /// </summary>
    public enum ObjectTypes
    {
        /// <summary>
        /// Default type. An <see cref="Object"/> instance is possibly had bad initialization.
        /// </summary>
        Object,
        /// <summary>
        /// An <see cref="Object"/> instance is derived from <see cref="GridObject"/> class.
        /// </summary>
        GridObject,
        /// <summary>
        /// An <see cref="Object"/> instance is derived from <see cref="Unit"/> class (except Slug).
        /// </summary>
        Unit,
        /// <summary>
        /// An instance is <see cref="Slug"/> class instance.
        /// </summary>
        Slug,
    };

    /// <summary>
    /// Defines the determinate state in which an instance of the <see cref="Object"/> class can be.
    /// </summary>
    public enum ObjectStates
    {
        /// <summary>
        /// Set by default
        /// </summary>
        Default,
        /// <summary>
        /// An instance is no longer exist and wating for deletion from <see cref="ObjectContainer"/>.
        /// </summary>
        Removed,
    }

    /// <summary>
    /// Provides data for the <see cref="Object.PositionChanged"/> and <see cref="Object.LocationChanged"/> events.
    /// </summary>
    public class PositionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the previous <see cref="Object"/> position (before changing).
        /// </summary>
        public GPS PrevPosition { get; private set; }

        /// <summary>
        /// Gets the new <see cref="Object"/> position (after changing).
        /// </summary>
        public GPS NewPosition { get; private set; }

        /// <summary>
        /// Initializes a new instance of the PositionEventArgs class.
        /// </summary>
        /// <param name="prevPosition">The previous <see cref="Object"/> position.</param>
        /// <param name="newPosition">The new <see cref="Object"/> position.</param>
        public PositionEventArgs(GPS prevPosition, GPS newPosition)
        {
            PrevPosition = prevPosition;
            NewPosition = newPosition;
        }
    }

    /// <summary>
    /// Specifies the base class for every Object that can be placed on Map (i.e. has any <see cref="Object.Position"/> within any <see cref="Cell"/>)
    /// </summary>
    public abstract class Object
    {
        /// <summary>
        /// Represents an Object size information that is used to prevent collistions and intersections with walls.
        /// </summary>
        protected struct ModelSize
        {
            public int Width;
            public int Height;
        };

        /// <summary>
        /// Represents the method that will handle the <see cref="Object.PositionChanged"/> or <see cref="Object.LocationChanged"/> or <see cref="Unit.Relocated"/> event of an <see cref="Object"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="PositionEventArgs"/> that contains the event data.</param>
        public delegate void PositionHandler(object sender, PositionEventArgs e);
        /// <summary>
        /// Occurs after object changed its Position.
        /// </summary>
        public event PositionHandler PositionChanged;

        /// <summary>
        /// Occurs after object changed its GPS location, i.e moved to other <see cref="Cell"/>
        /// </summary>
        public event PositionHandler LocationChanged;

        protected ObjectTypes pr_objectType;
        /// <summary>
        /// Gets or sets the object essense (based on derived class).
        /// </summary>
        public ObjectTypes ObjectType
        {
            get { return this.pr_objectType; }
            protected set { this.pr_objectType = value; }
        }

        protected ObjectStates pr_objectState;
        /// <summary>
        /// Get or sets the current state of an object.
        /// </summary>
        public ObjectStates ObjectState
        {
            get { return this.pr_objectState; }
            set { this.pr_objectState = value; }
        }

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
                    Cell newCell = Map.Instance.GetCell(newPosition.Location);

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

        /// <summary>
        /// Gets or sets an Object GUID value.
        /// </summary>
        public uint GUID { get; protected set; }

        /// <summary>
        /// The <see cref="Cell"/> object where this Object is belong (located).
        /// </summary>
        protected Cell currentCell;

        /// <summary>
        /// The object size information.
        /// </summary>
        protected ModelSize objectSize;

        /// <summary>
        /// Initializes a new instance of the Object class.
        /// </summary>
        public Object()
        {
            ObjectType = ObjectTypes.Object;
            ObjectState = ObjectStates.Default;

            // Initialize Position by default values
            // Seems not needed
            // All uninitialized values is auto-initialized by 0

            currentCell.Initialize();
        }

        /// <summary>
        /// <see cref="Object.Create"/> an object with the specified position.
        /// </summary>
        /// <param name="position">Object default position</param>
        public virtual void Create(GPS position)
        {
            Position = position;
            Create();
        }

        /// <summary>
        /// Initilize this instance with assigning the GUID value and placing into <see cref="ObjectContainer"/>.
        /// </summary>
        public virtual void Create()
        {
            // Do not create object twice
            if (GUID != 0)
                return;

            GUID = ObjectContainer.Instance.CreateObject(this);
        }

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
        /// Gets a linear distance to another Object.
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
            Cell cell = Map.Instance.GetCell(position.Location);

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

        /// <summary>
        /// Update the object statement due to past time.
        /// </summary>
        /// <param name="timeP">Elapsed time value (in milliseconds).</param>
        public virtual void UpdateState(int timeP) { }

    }
}
