using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Phobos : Unit
    {
        private Directions CurrentDirection;
        private bool IsInMotion;

        public Phobos()
        {
            unitType = UnitTypes.Phobos;
            IsInMotion = false;

            Position.Location = GetWorldMap().GetFinishPoint();
            Position.X = 10;
            Position.Y = 25;
            Position.BlockID = 0;
            IsInMotion = false;
            CurrentDirection = Directions.None;

            currentGridMap = GetWorldMap().GetGridMap(Position.Location);
        }
        
    }
}
