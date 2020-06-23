﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level) {
            BuildLookupTable();
            float[] levels = lookupTable[characterClass][stat];
            if(levels == null || levels.Length < level){
                return 0;
            }
            return levels[level -1];
        }

        private void BuildLookupTable()
        {
            if(lookupTable == null){
                lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
                foreach (ProgressionCharacterClass progressionClass in characterClasses)
                {
                    var statLookupTable = new Dictionary<Stat, float[]>();
                    foreach (ProgressionStat progressionStat in progressionClass.stats)
                    {
                        statLookupTable[progressionStat.stat] = progressionStat.levels;
                    }
                    lookupTable[progressionClass.characterClass] = statLookupTable; 
                }
            }
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookupTable();
            return lookupTable[characterClass][stat].Length;
        }

        [System.Serializable]
        public class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        public class ProgressionStat{
            public Stat stat;
            public float[] levels;
        }
    }
}