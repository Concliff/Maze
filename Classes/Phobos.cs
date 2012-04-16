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
            UnitType = UnitTypes.Phobos;
            IsInMotion = false;

            /*
            Position.Location = World.GetWorldMap().GetFinishPoint();
            Position.X = 25;
            Position.Y = 25;
            Position.BlockID = 0;
            CurrentDirection = Directions.None;

            CurrentGridMap = World.GetWorldMap().GetGridMapByGPS(Position.Location);
            */
        }
    }
}
