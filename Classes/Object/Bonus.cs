using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public class Bonus : GridObject
    {
        private ushort effectID;
        private bool isOpen;

        public Bonus(GridGPS currentGridGPS)
        {
            Position = currentGridGPS;
            currentGridMap = GetWorldMap().GetGridMap(currentGridGPS.Location);

            gridObjectType = GridObjectType.Bonus;
            SetFlag(GridObjectFlags.Temporal);
            timeToLive = 10000; // LifeTime = 10 sec

            effectID = 0;
            isOpen = true;
        }

        public void SetEffect(ushort effectID, bool isOpen)
        {
            this.isOpen = isOpen;
            this.effectID = effectID;
        }

        public override void Use(Unit user)
        {
            /* Apply Effect
             * 
             * 
             * 

             * */

            base.Use(user);
        }

    }
}
