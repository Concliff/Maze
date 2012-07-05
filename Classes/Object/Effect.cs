using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace Maze.Classes
{
    public enum EffectTypes : byte
    {
        None            = 0,
        IncreaseSpeed   = 1,
        Snare           = 2,
        Stun            = 3,
        SlimeDuration   = 4,
        Root            = 5,
        Invisibility    = 6,
        MoveReverse     = 7,
    };

    public enum EffectTargets : byte
    {
        None                = 0,
        Caster              = 1,
        NearestUnit         = 2,
        AllEnemiesInArea    = 3,
    };

    public enum EffectAttributes : ushort
    {
        None                = 0,
        OnlySlug            = 0x001,    // Applies only on Slug
        CanBeSpell          = 0x002,    // Allows to appear at SpellBar
    };

    public enum EffectState
    {
        Applied,
        Updated,
        Expired,
    };

    public struct EffectEntry
    {
        public ushort ID;
        public String EffectName;
        public ushort Attributes;
        public EffectTargets Targets;
        public short Range;
        public EffectTypes EffectType;
        public int Value;
        public short Duration;
        public short ND1;
        public short ND2;
        public short ND3;
        public short ND4;
        public String Description;

        public bool HasAttribute(EffectAttributes attribute)
        {
            return (Attributes & (uint)attribute) != 0;
        }
    }

    public class Effect
    {
        private EffectEntry effectInfo;
        private Unit target;
        private Unit caster;

        public Effect(EffectEntry effectEntry, Unit target, Unit caster)
        {
            this.effectInfo = effectEntry;
            this.target = target;
            this.caster = caster;
        }

        public void Cast()
        {
            if (effectInfo.HasAttribute(EffectAttributes.OnlySlug) && target.GetType() != ObjectType.Slug)
                return;

            EffectHolder effectHolder = new EffectHolder(effectInfo);

            switch (effectInfo.Targets)
            {
                case EffectTargets.Caster:
                    caster.ApplyEffect(effectHolder);
                    break;
                case EffectTargets.NearestUnit:
                    List<Unit> unitsAround = ObjectSearcher.GetUnitsWithinRange(caster, effectInfo.Range);
                    int minDistance = effectInfo.Range;
                    Unit nearestUnit = null;

                    foreach (Unit unit in unitsAround)
                    {
                        if (caster.GetDistance(unit) < minDistance)
                            nearestUnit = unit;
                    }

                    if (nearestUnit != null)
                        nearestUnit.ApplyEffect(effectHolder);
                    break;
                case EffectTargets.AllEnemiesInArea:
                    List<Unit> enemiesAround = ObjectSearcher.GetUnitsWithinRange(caster, effectInfo.Range);
                    foreach (Unit unit in enemiesAround)
                    {
                        if (unit.GetType() == caster.GetType())
                            continue;

                        unit.ApplyEffect(effectHolder);
                    }
                    break;
            }

        }
    }

    public class EffectHolder
    {
        private int duration;
        private EffectState effectState;

        public EffectEntry EffectInfo;
        public int Duration
        {
            get { return duration; }
            private set { }
        }

        public EffectHolder(EffectEntry effectEntry)
        {
            this.EffectInfo = effectEntry;
            this.duration = effectEntry.Duration;

            effectState = EffectState.Applied;
        }

        public void UpdateTime(int timeP)
        {
            if (effectState == EffectState.Applied)
            {
                effectState = EffectState.Updated;
                return;
            }

            if (EffectInfo.Duration == -1)   // One-Tact effect
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

        public void Refresh()
        {
            duration = EffectInfo.Duration;
        }

        public EffectState GetState()
        {
            return effectState;
        }

    }
}
