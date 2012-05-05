using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Phobos : Unit
    {

        public Phobos()
        {
            unitType = UnitTypes.Phobos;

            Position.Location = GetWorldMap().GetFinishPoint();
            Position.X = 10;
            Position.Y = 25;
            Position.BlockID = 0;
            isInMotion = false;
            currentDirection = Directions.None;

            currentGridMap = GetWorldMap().GetGridMap(Position.Location);
        }
        
    }
}
