using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public struct BonusEffect
    {
        public ushort EffectID;
        public bool IsOpen;

        public BonusEffect(ushort effectID, bool isOpen)
        {
            EffectID = effectID;
            IsOpen = isOpen;
        }
    };

    public class Bonus : GridObject
    {
        private BonusEffect bonusEffect;


        public Bonus(GridGPS currentGridGPS)
        {
            Position = currentGridGPS;
            currentGridMap = GetWorldMap().GetGridMap(currentGridGPS.Location);

            gridObjectType = GridObjectType.Bonus;
            SetFlag(GridObjectFlags.Temporal);
            SetFlag(GridObjectFlags.Disposable);
            timeToLive = 5000; // LifeTime = 5 sec

            bonusEffect = new BonusEffect(0, false);
        }

        public void SetEffect(BonusEffect bonusEffect)
        {
            this.bonusEffect = bonusEffect;
        }

        public ushort GetEffect()
        {
            if (bonusEffect.IsOpen)
                return bonusEffect.EffectID;
            else
                return 0;
        }

        public override void Use(Unit user)
        {
            if (!bonusEffect.IsOpen)
            {

                if (user.UnitType == UnitTypes.Slug)
                    ((Slug)user).CollectHiddenBonus(bonusEffect.EffectID);
                else
                    return;
            }
            else
            {
                user.CastEffect(bonusEffect.EffectID, user);
            }

            base.Use(user);
        }
    }
}
