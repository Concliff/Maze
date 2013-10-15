using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    /// <summary>
    /// Contains information about an effect that a <see cref="Bonus"/> applies.
    /// </summary>
    public struct BonusEffect
    {
        /// <summary>
        /// <see cref="EffectEntry.ID"/>
        /// </summary>
        public ushort EffectID;
        /// <summary>
        /// Inidicating whether a <see cref="Bonus"/> is displayed with effect icon or just a question mark.
        /// </summary>
        public bool IsOpen;

        /// <summary>
        /// Initializes a new instance of the BonusEffect class.
        /// </summary>
        /// <param name="effectID">ID of the applying effect.</param>
        /// <param name="isOpen"><c>true</c> if the Bonus is displayed with effect icon; otherwise, <c>false</c>.</param>
        public BonusEffect(ushort effectID, bool isOpen)
        {
            EffectID = effectID;
            IsOpen = isOpen;
        }
    };

    /// <summary>
    /// Represents a gridobject that randomly appears on the Map every several seconds and applies at its "user" the specific effect.
    /// </summary>
    public class Bonus : GridObject
    {
        /// <summary>
        /// The effect information that this Bonus contains.
        /// </summary>
        private BonusEffect bonusEffect;

        /// <summary>
        /// Initializes a new instance of the Bonus class.
        /// </summary>
        public Bonus()
        {
            this.gridObjectType = GridObjectTypes.Bonus;
            this.gridObjectsFlags |= GridObjectFlags.Temporal | GridObjectFlags.Disposable;
            timeToLive = 5000; // LifeTime = 5 sec

            bonusEffect = new BonusEffect(0, false);
        }

        public void Create(GPS position, BonusEffect effect)
        {
            this.bonusEffect = effect;
            base.Create(position);
        }

        /// <summary>
        /// Gets a <see cref="BonusEffect.EffectID"/> value of the effect that this bonus contains.
        /// </summary>
        /// <returns>Effect ID if the bonus is open; otherwise, returns 0.</returns>
        public ushort GetEffect()
        {
            if (bonusEffect.IsOpen)
                return bonusEffect.EffectID;
            else
                return 0;
        }

        /// <summary>
        /// Ovverrides <see cref="GridObject.Use"/>.
        /// </summary>
        public override void Use(Unit user)
        {
            if (!user.IsVisible || !user.IsAlive)
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
