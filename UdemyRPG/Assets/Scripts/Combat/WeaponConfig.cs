using System;
using RPG.Controller;
using RPG.Resources;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] float percentageBonusDamage = 0f;
        [SerializeField] CursorType cursorType = CursorType.Combat;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator){
            Weapon weapon = null;
            if (rightHand != null || leftHand != null)
            {
                DestroyOldWeapon(rightHand, leftHand);
                if(weaponPrefab != null){
                    weapon = Instantiate(weaponPrefab, isRightHanded ? rightHand:leftHand);
                    weapon.name = weaponName;
                }
            }
            if (animator != null)
            {
                var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                if(weaponOverride != null){
                    animator.runtimeAnimatorController = weaponOverride;
                }
                else if(overrideController != null){
                    animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                }
            }
            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null){
                oldWeapon = leftHand.Find(weaponName);
            }
            if(oldWeapon !=null){
                oldWeapon.name = "DESTROYED WEAPON";
                Destroy(oldWeapon.gameObject);
            }
        }

        public bool HasProjectile() {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, Health casterHealth, float damage){
            if(HasProjectile()){
                Projectile  projectileInstance = Instantiate(projectile, isRightHanded ? rightHand.position : leftHand.position, Quaternion.identity);
                projectileInstance.SetTarget(target, casterHealth, damage);
            }
        }

        public float GetWeaponDamage() {
            return weaponDamage;
        }

        public float GetPercentageBonusDamage(){
            return percentageBonusDamage;
        }

        public float GetWeaponRange() {
            return weaponRange;
        }

        public CursorType GetWeaponCursorType()
        {
            return cursorType;
        }

    }
}