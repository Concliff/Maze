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
        CreateClone     = 8,
        InstantKill     = 9,
        Replenishment   = 10,
        Shield          = 11,
        SmokeBomb       = 12,
        SmokeCloud      = 13,
        JumpThrough     = 14,
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
        NoAura              = 0x004,    // Effect doesn't apply any visible auras
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
        private List<Unit> targetsList;

        public Effect(EffectEntry effectEntry, Unit target, Unit caster)
        {
            this.effectInfo = effectEntry;
            this.target = target;
            this.caster = caster;

            targetsList = new List<Unit>();
        }

        public void Cast()
        {
            // Add current target
            if (effectInfo.Targets == EffectTargets.None)
                AddTarget(target);

            // Fill Targets List
            switch (effectInfo.Targets)
            {
                case EffectTargets.Caster:
                    AddTarget(caster);
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
                        AddTarget(nearestUnit);
                    break;
                case EffectTargets.AllEnemiesInArea:
                    List<Unit> enemiesAround = ObjectSearcher.GetUnitsWithinRange(caster, effectInfo.Range);
                    foreach (Unit unit in enemiesAround)
                    {
                        if (unit.GetType() == caster.GetType())
                            continue;

                        AddTarget(unit);
                    }
                    break;
            }

            if (targetsList.Count == 0)
                return;

            if (effectInfo.HasAttribute(EffectAttributes.NoAura))
                ApplyEffect();
            else
                ApplyAura();

        }
        /// <summary>
        /// Used when Effect dosn't have Aura
        /// and therefore do not need to add EffectHolder to Unit
        /// </summary>
        private void ApplyEffect()
        {
            switch (effectInfo.EffectType)
            {
                case EffectTypes.CreateClone:
                    if (caster.UnitType != UnitTypes.Slug)
                        break;
                    Slug slug = (Slug)target;
                    slug.CreateClone();
                    break;
                case EffectTypes.InstantKill:
                    foreach (Unit unitTarget in targetsList)
                        caster.KillUnit(unitTarget);
                    break;
                case EffectTypes.SmokeBomb:
                    // Create Smoke Cloud at caster position
                    new SmokeCloud(caster.Position);
                    break;
                case EffectTypes.JumpThrough:
                    caster.JumpThroughDistance(effectInfo.Range);
                    break;
            }
        }

        /// <summary>
        /// Used when need to apply EffectHolder and make an Aura for Unit
        /// </summary>
        private void ApplyAura()
        {
            if (effectInfo.HasAttribute(EffectAttributes.OnlySlug) && target.GetType() != ObjectType.Slug)
                return;

            EffectHolder effectHolder = new EffectHolder(effectInfo);

            foreach (Unit unitTarget in targetsList)
                unitTarget.ApplyEffect(effectHolder);
        }

        private bool AddTarget(Unit target)
        {
            if (targetsList.Contains(target))
            {
                return false;
            }
            else
            {
                targetsList.Add(target);
                return true;
            }
        }
    }

    public class EffectHolder
    {
        private int pr_Duration;
        private EffectState pr_EffectState;

        /// <summary>
        /// Gets state of the effect
        /// </summary>
        public EffectState EffectState
        {
            get { return pr_EffectState; }
            private set { pr_EffectState = value;}
        }

        public EffectEntry EffectInfo;

        /// <summary>
        /// Returns the duration of the effect
        /// </summary>
        public int Duration
        {
            get { return pr_Duration; }
            private set { ;}
        }

        public EffectHolder(EffectEntry effectEntry)
        {
            this.EffectInfo = effectEntry;
            this.pr_Duration = effectEntry.Duration;

            EffectState = EffectState.Applied;
        }

        public void UpdateTime(int timeP)
        {
            if (EffectState == EffectState.Applied)
            {
                EffectState = EffectState.Updated;
                return;
            }

            if (EffectInfo.Duration == -1)   // One-Tact effect
            {
                EffectState = EffectState.Expired;
                return;
            }
            else if (pr_Duration < timeP)
            {
                EffectState = EffectState.Expired;
            }
            else
                pr_Duration -= timeP;
        }

        public void Refresh()
        {
            pr_Duration = EffectInfo.Duration;
        }
    }
}
