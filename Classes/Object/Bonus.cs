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

        public Bonus()
        {
            GridObjectType = GridObjectTypes.Bonus;
            SetFlag(GridObjectFlags.Temporal);
            SetFlag(GridObjectFlags.Disposable);
            timeToLive = 5000; // LifeTime = 5 sec

            bonusEffect = new BonusEffect(0, false);
        }

        public void Create(GPS position, BonusEffect effect)
        {
            this.bonusEffect = effect;
            base.Create(position);
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
            if (!user.IsVisible() || !user.IsAlive())
                return;

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
