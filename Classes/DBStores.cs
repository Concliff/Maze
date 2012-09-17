﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Maze.Classes
{
    public static class DBStores
    {
        private static String DBDirectory = "Data\\Base\\";

        public static List<EffectEntry> EffectStore;

        public static void InitializeComponents()
        {
            EffectStore = new List<EffectEntry>();
        }

        public static void Load()
        {
            // Load stores from files

            // Load Effects
            StreamReader effectStream = File.OpenText(DBDirectory + "Effects.db");
            string row;

            // Insert empty entry with index 0
            EffectStore.Insert(0, new EffectEntry());
            while ((row = effectStream.ReadLine()) != null)
            {
                if (row == string.Empty)
                    continue;

                string[] entryStruct = new string[13];
                entryStruct = row.Split('|');

                EffectEntry entry;

                entry.ID = Convert.ToUInt16(entryStruct[0]);
                entry.EffectName = Convert.ToString(entryStruct[1]);
                entry.Attributes = Convert.ToUInt16(entryStruct[2]);
                entry.Targets = (EffectTargets)Convert.ToByte(entryStruct[3]);
                entry.Range = Convert.ToInt16(entryStruct[4]);
                entry.EffectType = (EffectTypes)Convert.ToByte(entryStruct[5]);
                entry.Value = Convert.ToInt32(entryStruct[6]);
                entry.Duration = Convert.ToInt16(entryStruct[7]);
                entry.ND1 = Convert.ToInt16(entryStruct[8]);
                entry.ND2 = Convert.ToInt16(entryStruct[9]);
                entry.ND3 = Convert.ToInt16(entryStruct[10]);
                entry.ND4 = Convert.ToInt16(entryStruct[11]);
                entry.Description = Convert.ToString(entryStruct[12]);

                EffectStore.Insert(entry.ID, entry);
            }

            effectStream.Close();
        }
    }
}
