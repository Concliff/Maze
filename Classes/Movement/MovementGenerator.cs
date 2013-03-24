﻿using System;
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
        protected bool cellReached;
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

        public void StartMotion()
        {
            IsInMotion = true;

            if (nextGPS.BlockID == 0)
                nextGPS = this.mover.Position;

            remainDistance = this.mover.Position.GetDistance(nextGPS);
        }

        public void StopMotion()
        {
            IsInMotion = false;
        }

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

        private void mover_Relocated(object sender, PositionEventArgs e)
        {
            OnDestinationReached();
        }

        protected virtual void OnDestinationReached() { ;}

        protected virtual void OnLocationChanged(object sender, PositionEventArgs e)
        {
            this.cellReached = false;
        }

        public GPS DefineNextGPS(Direction Currenetdirection)
        {
            nextGPS = new GPS(mover.Position, 25, 25);

            switch (CurrentDirection.First)
            {
                case Directions.Up: --nextGPS.Location.Y; break;
                case Directions.Down: ++nextGPS.Location.Y; break;
                case Directions.Left: --nextGPS.Location.X; break;
                case Directions.Right: ++nextGPS.Location.X; break;
            }
            return nextGPS;
        }
    }

}
