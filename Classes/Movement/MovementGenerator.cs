using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public enum MovementGeneratorType
    {
        None,           // Do Not Have Any Movements
        Manual,         // Controlled by Player
        Random,         // Generate random path directions within several map blocks
        PathFinder,     // Searching for the path between start and finish points
    }

    public abstract class MovementGenerator
    {
        protected MovementGeneratorType generatorType;
        protected GridMap startPoint;
        protected GridMap finalPoint;

        public List<GridMap> Path;
        public Map WorldMap;

        public MovementGenerator()
        {
            generatorType = MovementGeneratorType.None;
            Path = new List<GridMap>();
            WorldMap = Map.WorldMap;
        }

        public abstract void GeneratePath();

        public void GeneratePath(GridMap startPoint, GridMap finalPoint)
        {
            SetStartFinish(startPoint, finalPoint);
            GeneratePath();
        }

        public abstract bool ProcessMovement();

        public void SetStartFinish(GridMap startPoint, GridMap finalPoint)
        {
            this.startPoint = startPoint;
            this.finalPoint = finalPoint;
        }

        public virtual new MovementGeneratorType GetType()
        {
            return generatorType;
        }

    }

    public class ManualMovement : MovementGenerator
    {
        public ManualMovement()
        {
            generatorType = MovementGeneratorType.Manual;
        }

        public override bool ProcessMovement()
        {
            return false;
        }

        public override void GeneratePath()
        {
            
        }
    }
}
