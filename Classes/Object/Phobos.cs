using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

        private Directions CurrentDirection;
        private bool IsInMotion;
        private int pathFindingTimer;

        private Algorithm alg;
        private Unit victim;

        private SubDirections subDirection;

        public Phobos(GPS respawnLocation)
        {
            unitType = UnitTypes.Phobos;
            IsInMotion = false;

            this.respawnLocation = respawnLocation;

            // Just for now
            victim = World.GetPlayForm().GetPlayer();

            Position.Location = respawnLocation;
            Position.X = 25;
            Position.Y = 25;
            Position.BlockID = GetWorldMap().GetGridMap(Position.Location).ID;

            IsInMotion = false;
            CurrentDirection = Directions.None;
            alg = new Algorithm();

            currentGridMap = GetWorldMap().GetGridMap(Position.Location);

            pathFindingTimer = 1000;

            // Tested Speed
            // Should be 0.7 or about
            SetBaseSpeed(1.0d);
            speedRate = baseSpeed;
        }

        public override void StartMotion()
        {
            if (victim == null)
                return;

            alg.Way = alg.FindWay(currentGridMap, GetWorldMap().GetGridMap(victim.Position.Location));

            IsInMotion = true;
        }

        public override void UpdateState(int timeP)
        {
            if (IsInMotion)
            {
                if (pathFindingTimer < timeP)
                {
                    alg.Way = alg.FindWay(currentGridMap, GetWorldMap().GetGridMap(victim.Position.Location));
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

                if (alg.Way.Count > 0)
                    MovementAction();


            }

            base.UpdateState(timeP);
        }

        public void StopMotion() { IsInMotion = false; }

        public void MovementAction()
        {
            if (!IsAlive())
                return;

            int movementStep = (int)(GlobalConstants.MOVEMENT_STEP_PX * speedRate);
            if (alg.Way.Contains(currentGridMap))
            {
                switch (CurrentDirection)
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
            base.ReachedGridMap();

            if (alg.Way.Contains(currentGridMap))// && currentGridMap.CanMoveTo(CurrentDirection))
            {
                int index = alg.Way.IndexOf(currentGridMap);

                GPS nextGPS = Position.Location;
                GridMap nextGridMap;

                if (index > 0)
                {
                    nextGridMap = alg.Way[index - 1];


                    int shiftX = nextGridMap.Location.X - currentGridMap.Location.X;
                    int shiftY = nextGridMap.Location.Y - currentGridMap.Location.Y;

                    switch (shiftX * shiftY)
                    {
                        case -1:
                            if (shiftX == -1)
                                subDirection = SubDirections.LeftDown;
                            else
                                subDirection = SubDirections.RightUp;
                            CurrentDirection = Directions.None;
                            break;
                        case 0:
                            switch (shiftX)
                            {
                                case -1:
                                    CurrentDirection = Directions.Left;
                                    break;
                                case 0:
                                    if (shiftY == -1)
                                        CurrentDirection = Directions.Up;
                                    else
                                        CurrentDirection = Directions.Down;
                                    break;
                                case 1:
                                    CurrentDirection = Directions.Right;
                                    break;
                            }
                            subDirection = SubDirections.None;
                            break;
                        case 1:
                            if (shiftX == 1)
                                subDirection = SubDirections.RightDown;
                            else
                                subDirection = SubDirections.LeftUp;
                            CurrentDirection = Directions.None;
                            break;
                    }
                }
                else
                    nextGridMap = alg.Way[index];
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
