using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Phobos : Unit
    {
        private enum PhobosStates
        {
            None,           // Initialized
            StandingBy,     // victim is unreachable or not found
            Chasing,        // Chases the victim
            ReturningHome,  // Returns to spawn location (victim is dead or at its spawn location)
            DestinationReached,    // Reached final destination
        };

        private const int PATHFINDING_TIME = 3000;

        private int pathFindingTimer;
        private PathFinder pathFinder;
        private Unit victim;
        private PhobosStates state;

        public Phobos(GPS respawnLocation)
        {
            unitType = UnitTypes.Phobos;
            SetUnitFlags(UnitFlags.CanNotBeKilled);
            isInMotion = false;

            this.respawnLocation = respawnLocation;

            Position = new GridGPS(respawnLocation, 25, 25);

            pathFinder = new PathFinder();

            currentGridMap = GetWorldMap().GetGridMap(Position.Location);

            pathFindingTimer = PATHFINDING_TIME;
            state = PhobosStates.None;

            BaseSpeed = 0.4d;
        }

        public override void StartMotion()
        {
            victim = World.GetPlayForm().GetPlayer();

            if (victim == null)
            {
                state = PhobosStates.StandingBy;
                return;
            }

            FindPath();
        }

        public override void UpdateState(int timeP)
        {
            // Nothing to do if is dead
            if (!IsAlive())
                return;

            switch (state)
            {
                case PhobosStates.Chasing:
                case PhobosStates.ReturningHome:
                    MovementAction();
                    break;
                case PhobosStates.DestinationReached:
                case PhobosStates.StandingBy:
                    if (pathFindingTimer < timeP)
                    {
                        FindPath();
                        pathFindingTimer = PATHFINDING_TIME;
                    }
                    else
                        pathFindingTimer -= timeP;
                    break;
                default:
                    return;
            }

            base.UpdateState(timeP);
        }

        public void StopMotion() { isInMotion = false; }

        public void MovementAction()
        {
            if (!IsAlive())
                return;
            if (HasEffectType(EffectTypes.Root))
                return;

            // Generate Path if located at unknown grid
            if (!pathFinder.Path.Contains(currentGridMap))
                FindPath();

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * SpeedRate;
            if (this.currentDirection.Second != Directions.None)
                movementStepD = Math.Sqrt(2 * movementStepD);
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            MoveToDirection(movementStep, currentDirection);

        }

        protected override void OnPositionChanged(object sender, PositionEventArgs e)
        {
            if (state != PhobosStates.Chasing && state != PhobosStates.ReturningHome)
                return;

            // HACK: Ignore if not the center of the block
            int movementStep = (int)(GlobalConstants.MOVEMENT_STEP_PX * SpeedRate) + 1;
            if (!(Position.X >= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 - movementStep / 2 &&
                Position.X <= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 + movementStep / 2 &&
                Position.Y >= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 - movementStep / 2 &&
                Position.Y <= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 + movementStep / 2 &&
                !this.gridMapReached))
                return;

            Position = new GridGPS(Position, 25, 25);
            this.gridMapReached = true;
            FindPath();

            // TODO: Define state PhobosStates.DestinationReached

        }

        private void FindPath()
        {
            if (victim == null)
                return;

            if (!victim.IsAlive() || victim.IsAtRespawnLocation() || !victim.IsVisible())
            {
                state = PhobosStates.ReturningHome;
            }
            else if (state == PhobosStates.ReturningHome)
                state = PhobosStates.StandingBy;

            bool isHome = state == PhobosStates.ReturningHome;

            pathFinder.GeneratePath(currentGridMap,
                isHome ? GetWorldMap().GetGridMap(respawnLocation) : GetWorldMap().GetGridMap(victim.Position.Location));
            if (pathFinder.Path.Count == 0)
            {
                state = PhobosStates.StandingBy;
            }
            else
            {
                state = isHome ? PhobosStates.ReturningHome : PhobosStates.Chasing;

                if (pathFinder.Path.Contains(currentGridMap))
                {
                    int index = pathFinder.Path.IndexOf(currentGridMap);

                    GPS nextGPS = Position.Location;
                    GridMap nextGridMap;

                    if (index > 0)
                    {
                        nextGridMap = pathFinder.Path[index - 1];


                        int shiftX = nextGridMap.Location.X - currentGridMap.Location.X;
                        int shiftY = nextGridMap.Location.Y - currentGridMap.Location.Y;

                        switch (shiftX * shiftY)
                        {
                            case -1:
                                if (shiftX == -1)
                                {
                                    currentDirection.First = Directions.Left;
                                    currentDirection.Second = Directions.Down;
                                }
                                else
                                {
                                    currentDirection.First = Directions.Right;
                                    currentDirection.Second = Directions.Up;
                                }
                                 break;
                            case 0:
                                switch (shiftX)
                                {
                                    case -1:
                                        currentDirection.First = Directions.Left;
                                        break;
                                    case 0:
                                        if (shiftY == -1)
                                            currentDirection.First = Directions.Up;
                                        else
                                            currentDirection.First = Directions.Down;
                                        break;
                                    case 1:
                                        currentDirection.First = Directions.Right;
                                        break;
                                }
                                currentDirection.Second = Directions.None;
                                break;
                            case 1:
                                if (shiftX == 1)
                                {
                                    currentDirection.First = Directions.Right;
                                    currentDirection.Second = Directions.Down;
                                }
                                else
                                {
                                    currentDirection.First = Directions.Left;
                                    currentDirection.Second = Directions.Up;
                                }
                                break;
                        }
                    }
                    else
                        nextGridMap = pathFinder.Path[index];
                }
            }
        }
    }
}
