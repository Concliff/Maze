using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class ChasingMovement : MovementGenerator
    {
        private enum MotionStates
        {
            None,                   // Initialized
            StandingBy,             // victim is unreachable or not found
            Chasing,                // Chases the victim
            ReturningHome,          // Returns to spawn location (victim is dead or at its spawn location)
            DestinationReached,     // Reached final destination
        };

        /// <summary>
        /// Reference to a unit we are chasing
        /// </summary>
        private Unit victim;
        private PathFinder pathFinder;
        private const int PATHFINDING_TIME = 3000;
        private int pathFindingTimer;

        /// <summary>
        /// Gets or Sets current motion state.
        /// (Finite State Machine implementaion)
        /// </summary>
        private MotionStates state;

        public ChasingMovement(Unit unit)
            : base(unit)
        {
            pathFindingTimer = PATHFINDING_TIME;
            this.state = MotionStates.None;
            victim = World.PlayForm.Player;
            pathFinder = new PathFinder(Map.Instance.GetCell(this.mover.Position.Location),
                Map.Instance.GetCell(this.victim.Position.Location));
        }

        public override void UpdateState(int timeP)
        {
            if (!IsInMotion || this.mover.HasEffectType(EffectTypes.Root))
                return;

            // Select behavior to the current state
            switch (state)
            {
                case MotionStates.Chasing:
                case MotionStates.ReturningHome:
                    MovementAction();
                    break;
                case MotionStates.DestinationReached:
                case MotionStates.StandingBy:
                    if (pathFindingTimer < timeP)
                    {
                        FindPath();
                        pathFindingTimer = PATHFINDING_TIME;
                    }
                    else
                        pathFindingTimer -= timeP;
                    break;
            }

        }

        public override void StartMotion()
        {
            this.state = MotionStates.StandingBy;
            base.StartMotion();
        }

        private void MovementAction()
        {
            if (this.remainDistance <= 0)
            {
                OnDestinationReached();
                return;
            }

            // Generate Path if located at unknown grid
            if (!this.pathFinder.Path.Contains(Map.Instance.GetCell(mover.Position.Location)))
                FindPath();

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * this.mover.SpeedRate;
            if (CurrentDirection.Second != Directions.None)
                movementStepD = Math.Sqrt(2 * movementStepD);
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            this.remainDistance -= movementStep;
            Move(movementStep);
        }

        private void FindPath()
        {
            // when there is no victim unit
            if (this.victim == null)
                return;

            Cell currentCell = Map.Instance.GetCell(mover.Position.Location);

            // Stop pursuit
            // when victim is not available
            if (!this.victim.IsAlive() || this.victim.IsAtHome || !this.victim.IsVisible())
            {
                state = MotionStates.ReturningHome;
            }
            // otherwise stop motion when returning home
            // after a time the (pathFindingTimer) the pursuit of the victim will be continued
            else if (state == MotionStates.ReturningHome)
                state = MotionStates.StandingBy;

            bool isHome = state == MotionStates.ReturningHome;

            // Generate new path:
            // if ReturningHome - path to the respawn location
            // else - path to the victim location
            this.pathFinder.GeneratePath(currentCell,
                isHome ? Map.Instance.GetCell(mover.Home) : Map.Instance.GetCell(this.victim.Position.Location));

            // PathFinding failed or victim and mover are at the same location
            if (this.pathFinder.Path.Count == 0)
            {
                state = MotionStates.StandingBy;
            }
            else
            {
                state = isHome ? MotionStates.ReturningHome : MotionStates.Chasing;

                if (this.pathFinder.Path.Contains(currentCell))
                {
                    // Index number of the current Cell in Path
                    int index = this.pathFinder.Path.IndexOf(currentCell);

                    GridLocation nextGPS = mover.Position.Location;
                    Cell nextCell;

                    if (index > 0)
                    {
                        nextCell = this.pathFinder.Path[index - 1];

                        // Difine coords difference between current location and next one.
                        int shiftX = nextCell.Location.X - currentCell.Location.X;
                        int shiftY = nextCell.Location.Y - currentCell.Location.Y;

                        // shiftX or shiftY == 0 -> moving is unidirectional
                        // shiftX == -1 -> Left; == 1 -> Right;
                        // shiftY == -1 -> Up; == 1 -> Down;
                        switch (shiftX * shiftY)
                        {
                            case -1:
                                if (shiftX == -1)
                                    CurrentDirection = new Direction(Directions.Left, Directions.Down);
                                else
                                    CurrentDirection = new Direction(Directions.Right, Directions.Up);
                                break;
                            case 0:
                                switch (shiftX)
                                {
                                    case -1:
                                        CurrentDirection = new Direction(Directions.Left);
                                        break;
                                    case 0:
                                        if (shiftY == -1)
                                            CurrentDirection = new Direction(Directions.Up);
                                        else
                                            CurrentDirection = new Direction(Directions.Down);
                                        break;
                                    case 1:
                                        CurrentDirection = new Direction(Directions.Right);
                                        break;
                                }
                                break;
                            case 1:
                                if (shiftX == 1)
                                    CurrentDirection = new Direction(Directions.Right, Directions.Down);
                                else
                                    CurrentDirection = new Direction(Directions.Left, Directions.Up);
                                break;
                        }
                    }
                    else
                        // TODO: know why we are doing that code
                        nextCell = this.pathFinder.Path[index];

                    DefineNextGPS();
                }
            }
        }

        protected override void OnDestinationReached()
        {
            if (state != MotionStates.Chasing && state != MotionStates.ReturningHome)
                return;

            // After reaching next Cell, the victim could move to another location
            // need generate new Path
            // TODO: do this ONLY when victim changed location while the mover unit was moving
            FindPath();

            // TODO: Define state PhobosStates.DestinationReached

        }
    }
}
