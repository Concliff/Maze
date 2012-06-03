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
            SetFlag(GridObjectFlags.Disposable);
            timeToLive = 5000; // LifeTime = 5 sec

            effectID = 0;
            isOpen = true;
        }

        public void SetEffect(ushort effectID, bool isOpen)
        {
            this.isOpen = isOpen;
            this.effectID = effectID;
        }

        public ushort GetEffect()
        {
            if (isOpen)
                return effectID;
            else
                return 0;
        }

        public override void Use(Unit user)
        {
            user.CastEffect(effectID, user);

            base.Use(user);
        }

    }
}
