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
        protected MovementGeneratorType generatorType;
        protected Unit mover;

        /// <summary>
        /// Remain distance to the next block
        /// </summary>
        protected double remainDistance;

        protected GPS nextGPS; // Next block to move in the current direction


        protected bool pr_IsInMotion;
        public bool IsInMotion
        {
            get
            {
                return this.pr_IsInMotion;
            }
            protected set
            {
                pr_IsInMotion = value;
            }
        }

        public MovementGenerator(Unit unit)
        {
            this.mover = unit;
            generatorType = MovementGeneratorType.None;
            IsInMotion = false;

            this.mover.LocationChanged += OnLocationChanged;
            this.mover.Relocated += mover_Relocated;
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

        public virtual void StartMotion()
        {
            IsInMotion = true;
        }

        public virtual void StopMotion()
        {
            IsInMotion = false;
        }

        protected void Move(int movementStep)
        {
            GPS newPosition = this.mover.Position;

            newPosition.X += (int)Math.Cos(Orientation) * movementStep;
            newPosition.Y -= (int)Math.Sin(Orientation) * movementStep;

            this.mover.Position = newPosition;
        }

        private void mover_Relocated(object sender, PositionEventArgs e)
        {
            OnDestinationReached();
        }

        protected virtual void OnDestinationReached() { ;}

        protected virtual void OnLocationChanged(object sender, PositionEventArgs e) { ; }

        protected void DefineNextGPS()
        {
            this.nextGPS = new GPS(mover.Position, 25, 25);

            this.nextGPS.Location.X += (int)Math.Cos(Orientation);
            this.nextGPS.Location.Y -= (int)Math.Sin(Orientation);

            this.remainDistance = this.mover.Position.GetDistance(this.nextGPS);

        }
    }

}
