using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Specifies the type of generation of the movement behaviours.
    /// </summary>
    public enum MovementGeneratorType
    {
        /// <summary>
        /// Movement is not specified.
        /// </summary>
        None,
        /// <summary>
        /// Movement is controlled with the keyboard (Player control).
        /// </summary>
        Manual,
        /// <summary>
        /// Movement is randomly generated path of directions within several map blocks.
        /// </summary>
        Random,
        /// <summary>
        /// Movement is a pathfinding between initial and final points.
        /// </summary>
        PathFinder,
    }

    /// <summary>
    /// Represents the base class that provides motion handling with specified movement algorithm.
    /// </summary>
    public abstract class MovementGenerator : Movement
    {
        /// <summary>
        /// The type of generator. (What derived class is this).
        /// </summary>
        protected MovementGeneratorType generatorType;
        /// <summary>
        /// A Unit who this generator belongs to.
        /// </summary>
        protected Unit mover;
        /// <summary>
        /// Indicating whether the generator is processing movement.
        /// </summary>
        protected bool isInMotion;

        /// <summary>
        /// Remain distance to the next block.
        /// </summary>
        protected double remainDistance;

        /// <summary>
        /// Next position to move base on the current direction.
        /// </summary>
        protected GPS nextGPS;

        /// <summary>
        /// Initializes a new instance of the MovementGenerator class.
        /// </summary>
        /// <param name="unit">The owner of the generator instance.</param>
        public MovementGenerator(Unit unit)
        {
            this.mover = unit;
            generatorType = MovementGeneratorType.None;

            this.mover.LocationChanged += OnLocationChanged;
            this.mover.Relocated += mover_Relocated;
        }

        /// <summary>
        /// Gets a value indicating whether the generator is processing movement.
        /// </summary>
        public bool IsInMotion
        {
            get
            {
                return this.isInMotion;
            }
        }

        /// <summary>
        /// Gets the type of a generator.
        /// </summary>
        public new MovementGeneratorType GeneratorType
        {
            get
            {
                return this.generatorType;
            }
        }

        /// <summary>
        /// Updates the motion processing by one step.
        /// </summary>
        /// <returns>New Unit Position</returns>
        public abstract void UpdateState(int timeP);

        /// <summary>
        /// Indicates to the generator to start precessing movement.
        /// </summary>
        public virtual void StartMotion()
        {
            this.isInMotion = true;
        }

        /// <summary>
        /// Indicates to the generator to stop precessing movement.
        /// </summary>
        public virtual void StopMotion()
        {
            this.isInMotion = false;
        }

        /// <summary>
        /// Changes the mover position by specified step length in the current direction.
        /// </summary>
        /// <param name="movementStep">Step length.</param>
        protected void Move(int movementStep)
        {
            GPS newPosition = this.mover.Position;

            for (int i = 0; i < 2; ++i)
                switch (i == 0 ? CurrentDirection.First : CurrentDirection.Second)
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

            this.mover.Position = newPosition;
        }

        /// <summary>
        /// Calls when the mover has been relocated not by moving.
        /// </summary>
        private void mover_Relocated(object sender, PositionEventArgs e)
        {
            OnDestinationReached();
        }

        /// <summary>
        /// Calls when the mover reaches nextGPS point.
        /// </summary>
        protected virtual void OnDestinationReached() { ;}

        /// <summary>
        /// Calls when the mover moved to another <see cref="Cell"/>.
        /// </summary>
        protected virtual void OnLocationChanged(object sender, PositionEventArgs e) { ; }

        /// <summary>
        /// Defines the next <see cref="GPS"/> (or <see cref="Cell"/>) where the mover is directed.
        /// </summary>
        protected void DefineNextGPS()
        {
            this.nextGPS = new GPS(mover.Position, 25, 25);

            for (int i = 0; i < 2; ++i)
                switch (i == 0 ? CurrentDirection.First : CurrentDirection.Second)
                {
                    case Directions.Up: --this.nextGPS.Location.Y; break;
                    case Directions.Down: ++this.nextGPS.Location.Y; break;
                    case Directions.Left: --this.nextGPS.Location.X; break;
                    case Directions.Right: ++this.nextGPS.Location.X; break;
                }

            this.remainDistance = this.mover.Position.GetDistance(this.nextGPS);
        }
    }

}
