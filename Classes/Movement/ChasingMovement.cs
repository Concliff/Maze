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
            pathFinder = new PathFinder(WorldMap.GetGridMap(this.unit.Position.Location),
                WorldMap.GetGridMap(this.victim.Position.Location));
            FindPath();
        }

        public override void UpdateState(int timeP)
        {
            if (!IsInMotion || this.unit.HasEffectType(EffectTypes.Root))
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

        private void MovementAction()
        {
            // Generate Path if located at unknown grid
            if (!this.pathFinder.Path.Contains(WorldMap.GetGridMap(unit.Position.Location)))
                FindPath();

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * this.unit.SpeedRate;
            if (CurrentDirection.Second != Directions.None)
                movementStepD = Math.Sqrt(2 * movementStepD);
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            MoveToDirection(movementStep, CurrentDirection);

        }

        private void FindPath()
        {
            GridMap currentGridMap = WorldMap.GetGridMap(unit.Position.Location);

            if (this.victim == null)
                return;

            if (!this.victim.IsAlive() || this.victim.IsAtHome || !this.victim.IsVisible())
            {
                state = MotionStates.ReturningHome;
            }
            else if (state == MotionStates.ReturningHome)
                state = MotionStates.StandingBy;

            bool isHome = state == MotionStates.ReturningHome;

            this.pathFinder.GeneratePath(currentGridMap,
                isHome ? WorldMap.GetGridMap(unit.Home) : WorldMap.GetGridMap(this.victim.Position.Location));
            if (this.pathFinder.Path.Count == 0)
            {
                state = MotionStates.StandingBy;
            }
            else
            {
                state = isHome ? MotionStates.ReturningHome : MotionStates.Chasing;

                if (this.pathFinder.Path.Contains(currentGridMap))
                {
                    int index = this.pathFinder.Path.IndexOf(currentGridMap);

                    GPS nextGPS = unit.Position.Location;
                    GridMap nextGridMap;

                    if (index > 0)
                    {
                        nextGridMap = this.pathFinder.Path[index - 1];

                        int shiftX = nextGridMap.Location.X - currentGridMap.Location.X;
                        int shiftY = nextGridMap.Location.Y - currentGridMap.Location.Y;

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
                        nextGridMap = this.pathFinder.Path[index];
                }
            }
        }

        protected override void OnPositionChanged(object sender, PositionEventArgs e)
        {
            if (state != MotionStates.Chasing && state != MotionStates.ReturningHome)
                return;

            // HACK: Ignore if not the center of the block
            int movementStep = (int)(GlobalConstants.MOVEMENT_STEP_PX * unit.SpeedRate) + 1;
            if (!(unit.Position.X >= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 - movementStep / 2 &&
                unit.Position.X <= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 + movementStep / 2 &&
                unit.Position.Y >= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 - movementStep / 2 &&
                unit.Position.Y <= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 + movementStep / 2 &&
                !this.gridMapReached))
                return;

            unit.Position = new GridGPS(unit.Position, 25, 25);
            this.gridMapReached = true;
            FindPath();

            // TODO: Define state PhobosStates.DestinationReached

        }
    }
}
