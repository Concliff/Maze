using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public enum EffectTypes
    {
        Speed,
        Stun,
    };

    public enum EffectState
    {
        Applied,
        Updated,
        Expired,
    };

    public struct EffectEntry
    {
        public EffectTypes EffectType;
        public int Value;
        public int Duration;
    }

    public class Effect
    {
        private EffectEntry effectEntry;
        private int duration;   // Current effect time
        private Unit target;
        private Object caster;
        private EffectState effectState;

        public int Modifier;


        public Effect(EffectEntry effectEntry, Unit target, Object caster)
        {
            this.effectEntry = effectEntry;
            this.duration = effectEntry.Duration;
            this.target = target;
            this.caster = caster;
            this.Modifier = effectEntry.Value;

            target.ApplyEffect(this);
            effectState = EffectState.Applied;
        }

        public new EffectTypes GetType()
        {
            return effectEntry.EffectType;
        }

        public EffectState GetState()
        {
            return effectState;
        }

        public void UpdateTime(int timeP)
        {
            if (effectState == EffectState.Applied)
            {
                effectState = EffectState.Updated;
                return;
            }

            if (effectEntry.Duration == -1)   // One-Tact effect
            {
                effectState = EffectState.Expired;
                return;
            }
            else if (duration < timeP)
            {
                effectState = EffectState.Expired;
            }
            else
                duration -= timeP;
        }



    }
}
