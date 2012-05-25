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
    };

    public enum EffectTargets : byte
    {
        None                = 0,
        Caster              = 1,
        NearestUnit         = 2,
        AllEnemiesInArea    = 3,
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
    }

    public class Effect
    {
        private EffectEntry effectEntry;
        private int duration;   // Current effect time
        private Unit target;
        private Object caster;
        private EffectState effectState;

        public int Modifier;

        private string BaseDirectoryPath = "Data\\Base\\";

        public Effect(EffectEntry effectEntry, Unit target, Object caster)
        {
            this.effectEntry = effectEntry;
            this.duration = effectEntry.Duration;
            this.target = target;
            this.caster = caster;
            this.Modifier = effectEntry.Value;

            target.ApplyEffect(this);
            effectState = EffectState.Applied;

            //WorkDBF WDBF = new WorkDBF(BaseDirectoryPath);
            //string TableName = "EFFECTSD";
            //WDBF.CreateTable(TableName);
            //WDBF.FillTable(TableName2, "1", "Wall", "1", "10", "10", "The player creates a barrier in front of himself from the slime through which the monster can not pass");
            //WDBF.FillTable(TableName2, "2", "Invulnerability", "2", "6", "5", "The player becomes immune to monsters attacks for some time");
            //WDBF.FillTable(TableName2, "3", "Accelerator", "3", "4", "10", "The player gets a bonus to the speed of movement for X seconds");
            //DataTable table3 = WDBF.Execute("CREATE TABLE Clients (EffectId char(4), EffectName char(64), EffectType char(3), EffectValue char(10), Duration char(5), Description char(254))");
            //DataTable table = WDBF.GetAll(TableName);
            //DataTable table2 = WDBF.Execute("SELECT EFFECTID, EFFECTNAME FROM " + TableName);

            //WDBF.FillTable(TableName, 2, "Invulnerability", 2, 3, 2, 1, 4, 10, "The player becomes immune to monsters attacks for some time");
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
