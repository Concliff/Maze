using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Phobos : Unit
    {
        private enum SubDirections
        {
            None,
            LeftUp,
            LeftDown,
            RightUp,
            RightDown,
        };

        private int pathFindingTimer;
        private int refindingTimer;
        private PathFinder pathFinder;
        private Unit victim;

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

            pathFindingTimer = 1000;
            refindingTimer = 5000;

            SetBaseSpeed(0.4d);
        }

        public override void StartMotion()
        {
            victim = World.GetPlayForm().GetPlayer();

            if (victim == null)
                return;

            pathFinder.GeneratePath(currentGridMap, GetWorldMap().GetGridMap(victim.Position.Location));

            isInMotion = true;
        }

        public override void UpdateState(int timeP)
        {
            if (isInMotion)
            {
                if (pathFindingTimer < timeP)
                {
                    pathFinder.GeneratePath(currentGridMap, GetWorldMap().GetGridMap(victim.Position.Location));
                    pathFindingTimer = 1000;
                }
                else
                {
                    pathFindingTimer -= timeP;
                }

                if (IsAlive())
                {
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
                                    SetDeathState(DeathStates.Dead);
                                    return;
                                }
                            }
                        }
                    }
                }

                if (pathFinder.Path.Count > 0)
                    MovementAction();
                else
                    isInMotion = false;
            }
            // every 5 seconds try to find a way
            else
            {
                if (refindingTimer < 0)
                {
                    refindingTimer = 5000;
                    pathFinder.GeneratePath(currentGridMap, GetWorldMap().GetGridMap(victim.Position.Location));
                    if (pathFinder.Path.Count > 0)
                        isInMotion = true;
                }
                else
                    refindingTimer -= timeP;
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
            pathFinder.GeneratePath(currentGridMap, GetWorldMap().GetGridMap(victim.Position.Location));

            if (pathFinder.Path.Contains(currentGridMap))// && currentGridMap.CanMoveTo(currentDirection))
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
            base.ReachedGridMap();
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
