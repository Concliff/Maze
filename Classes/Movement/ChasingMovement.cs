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

        private Unit victim;
        private PathFinder pathFinder;
        private const int PATHFINDING_TIME = 3000;
        private int pathFindingTimer;
        private MotionStates state;

        public ChasingMovement(Unit unit)
            : base(unit)
        {
            pathFindingTimer = PATHFINDING_TIME;
            this.state = MotionStates.None;
            victim = World.PlayForm.Player;
            pathFinder = new PathFinder(WorldMap.GetCell(this.mover.Position.Location),
                WorldMap.GetCell(this.victim.Position.Location));
            FindPath();
        }

        public override void UpdateState(int timeP)
        {
            if (!IsInMotion || this.mover.HasEffectType(EffectTypes.Root))
                return;

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
            FindPath();
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
            if (!this.pathFinder.Path.Contains(WorldMap.GetCell(mover.Position.Location)))
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
            Cell currentCell = WorldMap.GetCell(mover.Position.Location);

            if (this.victim == null)
                return;

            if (!this.victim.IsAlive() || this.victim.IsAtHome || !this.victim.IsVisible())
            {
                state = MotionStates.ReturningHome;
            }
            else if (state == MotionStates.ReturningHome)
                state = MotionStates.StandingBy;

            bool isHome = state == MotionStates.ReturningHome;

            this.pathFinder.GeneratePath(currentCell,
                isHome ? WorldMap.GetCell(mover.Home) : WorldMap.GetCell(this.victim.Position.Location));
            if (this.pathFinder.Path.Count == 0)
            {
                state = MotionStates.StandingBy;
            }
            else
            {
                state = isHome ? MotionStates.ReturningHome : MotionStates.Chasing;

                if (this.pathFinder.Path.Contains(currentCell))
                {
                    int index = this.pathFinder.Path.IndexOf(currentCell);

                    GridLocation nextGPS = mover.Position.Location;
                    Cell nextCell;

                    if (index > 0)
                    {
                        nextCell = this.pathFinder.Path[index - 1];

                        int shiftX = nextCell.Location.X - currentCell.Location.X;
                        int shiftY = nextCell.Location.Y - currentCell.Location.Y;

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
                        nextCell = this.pathFinder.Path[index];

                    DefineNextGPS();
                }
            }
        }

        protected override void OnDestinationReached()
        {
            if (state != MotionStates.Chasing && state != MotionStates.ReturningHome)
                return;

            FindPath();

            // TODO: Define state PhobosStates.DestinationReached

        }
    }
}
