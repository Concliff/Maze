using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public enum MovementGeneratorType
    {
        None,           // Do Not Have Any Movements
        Manual,         // Controlled by Player
        Random,         // Generate random path directions within several map blocks
        PathFinder,     // Searching for the path between start and finish points
    }

    public abstract class MovementGenerator : Movement
    {
        protected bool gridMapReached;
        protected MovementGeneratorType generatorType;
        protected Unit unit;

        public MovementGenerator(Unit unit)
        {
            this.unit = unit;
            generatorType = MovementGeneratorType.None;

            this.unit.PositionChanged += OnPositionChanged;
            this.unit.LocationChanged += OnLocationChanged;
        }

        /// <summary>
        /// Updates the motion processing by one step.
        /// </summary>
        /// <returns>New Unit Position</returns>
        public abstract void UpdateState(int timeP);

        public new MovementGeneratorType GetType()
        {
            return generatorType;
        }

        protected void MoveToDirection(int movementStep, Movement.Direction direction)
        {
            GridGPS newPosition = this.unit.Position;

            for (int i = 0; i < 2; ++i)
                switch (i == 0 ? direction.First : direction.Second)
                {
                    case Directions.Up:
                        newPosition.Y -= movementStep;
                        break;
                    case Directions.Down:
                        newPosition.Y += movementStep;
                        break;
                    case Directions.Left:
                        newPosition.X -= movementStep;
                        break;
                    case Directions.Right:
                        newPosition.X += movementStep;
                        break;
                }

            this.unit.Position = newPosition;
        }

        protected virtual void OnPositionChanged(object sender, PositionEventArgs e) { ;}

        protected virtual void OnLocationChanged(object sender, PositionEventArgs e)
        {
            this.gridMapReached = false;
        }

    }

}
