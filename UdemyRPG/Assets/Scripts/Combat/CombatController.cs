using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Resources;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Controller;
using System;

namespace RPG.Combat
{
    public class CombatController : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {

        private float timeSinceLastAttack = 0f;

        [SerializeField] float timeBetweenAttack = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        Transform target;
        Health targetHealth;
        Mover mover;
        ActionScheduler actionScheduler;
        Animator animator;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        private void Awake() {
            actionScheduler = GetComponent<ActionScheduler>();
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(EquipDefaultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if(target != null && !targetHealth.IsDead()){
                if(Vector3.Distance(transform.position, target.position) > currentWeaponConfig.GetWeaponRange()){
                    mover.MoveCharTo(target.position, 1f);
                }
                else{
                    mover.Cancel();
                    AttackBehavior();
                }
            }
        }

        private Weapon EquipDefaultWeapon(){
            return EquipWeapon(defaultWeapon);
        }

        public Weapon EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = weapon.Spawn(rightHandTransform, leftHandTransform, animator);
            return currentWeapon.value;
        }

        private void AttackBehavior()
        {
            transform.LookAt(target.transform);
            if(timeSinceLastAttack>timeBetweenAttack){
                animator.ResetTrigger("stopAttack");
                animator.SetTrigger("attack");
                timeSinceLastAttack = 0;
            }
        }

        // Animator Event
        void Hit(){
            if (targetHealth != null)
            {
                float totalDamage = GetComponent<BaseStats>().GetStat(Stat.Damage);
                if(currentWeapon.value != null){
                    currentWeapon.value.OnHit();
                }

                if(currentWeaponConfig.HasProjectile()){
                    currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, targetHealth, GetComponent<Health>(), totalDamage);
                }
                else{
                    targetHealth.TakeDamage(totalDamage, gameObject);
                }
            }
        }

        void Shoot(){
            Hit();
        }

        public bool IsTargetDead(GameObject enemy){
            return enemy == null || enemy.GetComponent<Health>() == null || enemy.GetComponent<Health>().IsDead();
        }

        public void Attack(GameObject enemy){
            actionScheduler.StartAction(this);
            if(enemy != null){
                target = enemy.transform;
                targetHealth = target.GetComponent<Health>();
            }
        }

        public void ResetTarget(){
            target = null;
            targetHealth = null;
        }

        public void Cancel()
        {
            animator.SetTrigger("stopAttack");
            ResetTarget();
        }

        private WeaponConfig LoadWeaponFromResources(string weaponName)
        {
            return UnityEngine.Resources.Load<WeaponConfig>(weaponName);
        }

        public object CaptureState()
        {
            return currentWeaponConfig != null ? currentWeaponConfig.name : "Unarmed";
        }

        public void RestoreState(object state)
        {
            EquipWeapon(LoadWeaponFromResources((string)state));
        }

        public Health GetTargetHealth(){
            return targetHealth;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
           if(stat == Stat.Damage){
               yield return currentWeaponConfig.GetWeaponDamage();
           }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonusDamage();
            }
        }

        public CursorType GetWeaponCursorType()
        {
            return currentWeaponConfig.GetWeaponCursorType();
        }
    }
}
