﻿using System;
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

        Algorithm alg;
        Map currentMap;
        GridMap StartPoint;
        GridMap FinishPoint;
        GridGPS CurrentPosition;
        Slug player;

        SubDirections subDirection;

        public Phobos(GridMap StartPoint, GridMap FinishPoint)
        {
            unitType = UnitTypes.Phobos;
            IsInMotion = false;
            
            this.StartPoint = StartPoint;
            this.FinishPoint = FinishPoint;

            Position.Location = this.StartPoint.Location;
            Position.X = 25;
            Position.Y = 5;
            Position.BlockID = this.StartPoint.ID;
            IsInMotion = false;
            CurrentDirection = Directions.None;
            CurrentPosition = Position;
            alg = new Algorithm(StartPoint, FinishPoint);

            currentMap = alg.GetWorldMap();

            currentGridMap = StartPoint;

            player = World.GetPlayForm().GetPlayer();
        }

        public override void StartMotion()
        {
            IsInMotion = true;
            
                if (CurrentDirection == Directions.None)
                {
                    if (currentGridMap.CanMoveTo(Directions.Up))
                    {
                        CurrentDirection = Directions.Up;
                        return;
                    }
                    else if (currentGridMap.CanMoveTo(Directions.Left))
                    {
                        CurrentDirection = Directions.Left;
                        return;
                    }
                    else if (currentGridMap.CanMoveTo(Directions.Down))
                    {
                        CurrentDirection = Directions.Down;
                        return;
                    }
                    else if (currentGridMap.CanMoveTo(Directions.Right))
                    {
                        CurrentDirection = Directions.Right;
                        return;
                    }
                }
           
        }

        public override void UpdateState(int timeP)
        {
            if (IsInMotion)
            {
                alg.Way = alg.FindWay(currentGridMap, FinishPoint);

                MovementAction();
            }
            base.UpdateState(timeP);
        }

        public void StopMotion() { IsInMotion = false; }

        public void MovementAction()
        {
               if (CurrentDirection == Directions.None)
                    return;

                int speedModifier = 0;
                Effect speedEffect = GetEffectByType(EffectTypes.Speed);
                if (speedEffect != null)
                    speedModifier = speedEffect.Modifier;
                int movementStep = (int)(GlobalConstants.MOVEMENT_STEP_PX * speedRate);

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
                            if (Position.Y <0)
                            {
                                Position.Y += GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
                                ChangeGPSDueDirection(1, Directions.Up);
                            }
                            if (Position.X <0)
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

                if ((Position.X <= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 + movementStep / 2) &&
                    (Position.X >= GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2 - movementStep / 2) &&
                    (Position.Y <= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 + movementStep / 2) &&
                    (Position.Y >= GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2 - movementStep / 2))
                    ReachedGridMap();
            
        }

        protected override void ReachedGridMap()
        {
            base.ReachedGridMap();

            if (alg.Way.Contains(currentGridMap) && currentGridMap.CanMoveTo(CurrentDirection))
            {
                int index = alg.Way.IndexOf(currentGridMap);

                GPS nextGPS = Position.Location;
                GridMap nextGridMap;

                nextGridMap = alg.Way[index-1];

                int shiftX = nextGridMap.Location.X - currentGridMap.Location.X;
                int shiftY = nextGridMap.Location.Y - currentGridMap.Location.Y;

                switch (shiftX * shiftY)
                {
                    case -1:
                        if (shiftX == -1)
                            subDirection = SubDirections.LeftDown;
                        else
                            subDirection = SubDirections.RightUp;
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
                        break;
                    case 1:
                        if (shiftX == 1)
                            subDirection = SubDirections.RightDown;
                        else
                            subDirection = SubDirections.LeftUp;
                        break;
                }

                if (nextGridMap.HasAttribute(GridMapAttributes.IsStart))
                {
                    SelectNewDirection(false);
                }
                return;
            }
        }

        private void SelectNewDirection()
        {
            SelectNewDirection(true);
        }

        private void SelectNewDirection(bool includeCurrent)
        {
            Directions newDirection = Directions.None;
            int maxIterations = 10;

            for (int i = 0; i < maxIterations; ++i)
            {
                switch (Random.Int(4) + 1)
                {
                    case 1: newDirection = Directions.Right; break;
                    case 2: newDirection = Directions.Down; break;
                    case 3: newDirection = Directions.Left; break;
                    case 4: newDirection = Directions.Up; break;
                }
                if (!includeCurrent && newDirection == CurrentDirection)
                    continue;

                // Ignore Opposite Direction if there is another one
                if (currentGridMap.CanMoveTo(newDirection) &&
                    (newDirection != GetOppositeDirection(CurrentDirection) || CurrentDirection == Directions.None))
                {
                    CurrentDirection = newDirection;
                    return;
                }
            }

            // Go opposite Direction if no choice to go
            if (currentGridMap.CanMoveTo(GetOppositeDirection(CurrentDirection)))
                CurrentDirection = GetOppositeDirection(CurrentDirection);
            else
                CurrentDirection = Directions.None;
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
