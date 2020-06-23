using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Resources;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 100)]
        [SerializeField] int initialLevel = 1;
        [SerializeField] CharacterClass characterClass = CharacterClass.Player;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelupParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = true;
        Experience experience;

        public event Action onLevelup;

        LazyValue<int> currentLevel;

        private void Awake() {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start() {
            currentLevel.ForceInit();  
        }

        private void OnEnable() {
            if(experience !=null){
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable() {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel() {
            int newLevel = CalculateLevel();
            if(currentLevel.value<newLevel){
                currentLevel.value = newLevel;
                DeployLevelupParticleEffect();
                onLevelup();
            }
        }

        private void DeployLevelupParticleEffect()
        {
            if(levelupParticleEffect !=null){
                Instantiate(levelupParticleEffect, transform);
            } 
        }

        public float GetHealth(){
            return progression.GetStat(Stat.Health, characterClass, GetLevel());
        }

        public float GetExperience(){
            return progression.GetStat(Stat.ExperienceReward, characterClass, GetLevel());
        }

        public float GetExperienceForNextLevel()
        {
            int currentLvl = GetLevel();
            if(currentLvl >= (progression.GetLevels(Stat.ExperienceToLevelUp, characterClass)+1)){
                return float.MaxValue;
            }
            return progression.GetStat(Stat.ExperienceToLevelUp, characterClass, currentLvl);
        }

        public float GetExperienceForGivenLevel(int givenLevel){
            if(givenLevel<=0){
                return 0;
            }
            return progression.GetStat(Stat.ExperienceToLevelUp, characterClass, givenLevel);
        }

        public int CalculateLevel(){
            if(experience != null){
                float currentXP = experience.GetCurrentExperience();
                int maxLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
                for (int level = 1; level <= maxLevel; level++)
                {
                    float xpToLvlUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                    if(currentXP<xpToLvlUp){
                        return level;
                    }
                }
                return maxLevel + 1;
            }
            return this.initialLevel;
        }

        public int GetLevel(){
            return currentLevel.value;
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1+ GetPercertangeModifier(stat)/100);
        }

        private float GetPercertangeModifier(Stat stat)
        {
            float total = 0;
            if(shouldUseModifiers){
                foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
                {
                    foreach (float modifier in provider.GetPercentageModifiers(stat))
                    {
                        total += modifier;
                    }
                }
            }
            return total;
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel()); ;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            float total =0;
            if (shouldUseModifiers)
            {
                foreach(IModifierProvider provider in GetComponents<IModifierProvider>()){
                    foreach (float modifier in provider.GetAdditiveModifiers(stat))
                    {
                        total += modifier;
                    }
                }
            }
           return total;
        }
    }
}
