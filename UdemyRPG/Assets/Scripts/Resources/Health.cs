using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;
using System;
using UnityEngine.AI;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] TakeDamageEvent takeDamageTextSpawner;
        [SerializeField] UnityEvent spawnItem = null;
        [SerializeField] UnityEvent onDie = null;
        LazyValue<float> healthPoints;
        bool isDead=false;
        BaseStats baseStats;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> {
        }

        private void Awake() {
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealthPoints);
        }
        private void Start() {
            healthPoints.ForceInit();
        }

        private void OnEnable() {
            baseStats.onLevelup += RefreshHealthOnLevelup;
        }

        private void OnDisable() {
            baseStats.onLevelup -= RefreshHealthOnLevelup;
        }

        private void RefreshHealthOnLevelup()
        {
            healthPoints.value = baseStats.GetHealth();
        }

        public void TakeDamage(float damage, GameObject inistigator)
        {
            healthPoints.value = Mathf.Max(0, healthPoints.value - damage);
            takeDamageTextSpawner.Invoke(damage);
            CheckIfDead(inistigator);
        }

        private void CheckIfDead(GameObject inistigator)
        {
            if (!isDead && healthPoints.value == 0)
            {
                if(onDie !=null){
                    onDie.Invoke();
                }
                DropItem();
                Die();
                AwardExperience(inistigator);
            }
        }

        private void DropItem()
        {
            spawnItem.Invoke();
        }

        private void AwardExperience(GameObject inistigator)
        {
            if (inistigator != null)
            {
                Experience experience = inistigator.GetComponent<Experience>();
                if(experience != null){
                    experience.GainExperience(baseStats.GetExperience());
                }
            }
        }

        public void Die(){
            GetComponent<Animator>().SetTrigger("die");
            isDead = true;
            GetComponent<ActionScheduler>().DisableCurrentAction();
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            if(capsuleCollider != null){
                capsuleCollider.enabled = false;
            }
        }

        public bool IsDead(){
            return isDead;
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            this.healthPoints.value = (float) state;
            CheckIfDeadFromRestore();
        }

        private void CheckIfDeadFromRestore()
        {
            if (healthPoints.value == 0)
            {
                Die();
            }
        }

        public float GetHealthPoints(){
            return healthPoints.value;
        }

        public float GetInitialHealthPoints()
        {
            return baseStats.GetHealth();
        }
    }
}
