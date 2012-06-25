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

        private enum SubDirections
        {
            None,
            LeftUp,
            LeftDown,
            RightUp,
            RightDown,
        };

        private const int PATHFINDING_TIME = 3000;

        private int pathFindingTimer;
        private PathFinder pathFinder;
        private Unit victim;
        private PhobosStates state;

        private SubDirections subDirection;

        public Phobos(GPS respawnLocation)
        {
            unitType = UnitTypes.Phobos;
            isInMotion = false;

            this.respawnLocation = respawnLocation;

            Position.Location = respawnLocation;
            Position.X = 25;
            Position.Y = 25;
            Position.BlockID = GetWorldMap().GetGridMap(Position.Location).ID;

            isInMotion = false;
            currentDirection = Directions.None;
            pathFinder = new PathFinder();

            currentGridMap = GetWorldMap().GetGridMap(Position.Location);

            pathFindingTimer = PATHFINDING_TIME;
            state = PhobosStates.None;

            SetBaseSpeed(0.4d);
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
                    MovementAction();
                    List<Object> Objects = GetObjectsWithinRange(30);
                    if (Objects != null && Objects.Count != 0)
                    {
                        foreach (Object obj in Objects)
                        {
                            if (obj.GetType() == ObjectType.Slug)
                            {
                                Unit unit = (Unit)obj;
                                if (unit.IsAlive())
                                {
                                    unit.SetDeathState(DeathStates.Dead);
                                    state = PhobosStates.ReturningHome;
                                    return;
                                }
                            }
                        }
                    }
                    break;
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
            if (GetEffectsByType(EffectTypes.Root).Count != 0)
                return;

            double movementStepD = GlobalConstants.MOVEMENT_STEP_PX * this.speedRate;
            int movementStep = (int)(movementStepD);
            stepRemainder += movementStepD - movementStep;
            if (stepRemainder > 1d)
            {
                ++movementStep;
                stepRemainder -= 1;
            }

            if (pathFinder.Path.Contains(currentGridMap))
            {
                switch (currentDirection)
                {
                    case Directions.Up:
                        {
                            Position.X = 25;
                            Position.Y -= movementStep;
                            if (Position.Y < 0)
                            {
                                Position.Y += GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Up);
                            }

                            break;
                        }
                    case Directions.Down:
                        {
                            Position.X = 25;
                            Position.Y += movementStep;
                            if (Position.Y > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                            {
                                Position.Y -= GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Down);
                            }
                            break;
                        }
                    case Directions.Left:
                        {
                            Position.Y = 25;
                            Position.X -= movementStep;
                            if (Position.X < 0)
                            {
                                Position.X += GlobalConstants.GRIDMAP_BLOCK_WIDTH;
                                ChangeGPSDueDirection(1, Directions.Left);
                            }
                            break;
                        }
                    case Directions.Right:
                        {
                            Position.Y = 25;
                            Position.X += movementStep;
                            if (Position.X > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                            {
                                Position.X -= GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Right);
                            }
                            break;
                        }

                }

                switch (subDirection)
                {
                    case SubDirections.LeftDown:
                        {
                            Position.X -= movementStep;
                            Position.Y += movementStep;
                            if (Position.X < 0)
                            {
                                Position.X += GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Left);
                            }
                            if (Position.Y > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                            {
                                Position.Y -= GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Down);
                            }
                            break;
                        }
                    case SubDirections.LeftUp:
                        {
                            Position.X -= movementStep;
                            Position.Y -= movementStep;
                            if (Position.Y < 0)
                            {
                                Position.Y += GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Up);
                            }
                            if (Position.X < 0)
                            {
                                Position.X += GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Left);
                            }
                            break;
                        }
                    case SubDirections.RightDown:
                        {
                            Position.X += movementStep;
                            Position.Y += movementStep;
                            if (Position.X > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                            {
                                Position.X -= GlobalConstants.GRIDMAP_BLOCK_WIDTH;
                                ChangeGPSDueDirection(1, Directions.Right);
                            }
                            if (Position.Y > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                            {
                                Position.Y -= GlobalConstants.GRIDMAP_BLOCK_WIDTH;
                                ChangeGPSDueDirection(1, Directions.Down);
                            }
                            break;
                        }
                    case SubDirections.RightUp:
                        {
                            Position.X += movementStep;
                            Position.Y -= movementStep;
                            if (Position.X > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
                            {
                                Position.X -= GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Right);
                            }
                            if (Position.Y < 0)
                            {
                                Position.Y += GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Up);
                            }
                            break;
                        }
                }
            }
            if ((Position.X <= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 + movementStep / 2) &&
                (Position.X >= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 - movementStep / 2) &&
                (Position.Y <= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 + movementStep / 2) &&
                (Position.Y >= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 - movementStep / 2))
                ReachedGridMap();

        }

        protected override void ReachedGridMap()
        {
            FindPath();

            // TODO: Define state PhobosStates.DestinationReached

            base.ReachedGridMap();
        }

        private void FindPath()
        {
            if (!victim.IsAlive() || victim.IsAtRespawnLocation())
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
                                    subDirection = SubDirections.LeftDown;
                                else
                                    subDirection = SubDirections.RightUp;
                                currentDirection = Directions.None;
                                break;
                            case 0:
                                switch (shiftX)
                                {
                                    case -1:
                                        currentDirection = Directions.Left;
                                        break;
                                    case 0:
                                        if (shiftY == -1)
                                            currentDirection = Directions.Up;
                                        else
                                            currentDirection = Directions.Down;
                                        break;
                                    case 1:
                                        currentDirection = Directions.Right;
                                        break;
                                }
                                subDirection = SubDirections.None;
                                break;
                            case 1:
                                if (shiftX == 1)
                                    subDirection = SubDirections.RightDown;
                                else
                                    subDirection = SubDirections.LeftUp;
                                currentDirection = Directions.None;
                                break;
                        }
                    }
                    else
                        nextGridMap = pathFinder.Path[index];
                }
            }
        }

        private Directions GetOppositeDirection(Directions Direction)
        {
            switch (Direction)
            {
                case Directions.Left: return Directions.Right;
                case Directions.Right: return Directions.Left;
                case Directions.Down: return Directions.Up;
                case Directions.Up: return Directions.Down;
                default: return Directions.None;
            }
        }
    }
}
