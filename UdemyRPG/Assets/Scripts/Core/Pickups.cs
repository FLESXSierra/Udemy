using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Controller;
using UnityEngine;

namespace RPG.Core
{
    public class Pickups : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float respawnTime = 5f;
        bool removeAfterPickup = false;

        private void OnTriggerEnter(Collider other) {
            if(other.tag == "Player")
            {
                Pickup(other.GetComponent<CombatController>());
            }
        }

        internal void RemoveAfterPickup()
        {
            removeAfterPickup = true;
        }

        private void Pickup(CombatController combatController)
        {
            combatController.EquipWeapon(weapon);
            if(removeAfterPickup){
                DestroyAfterTime(0f);
            }
            else{
                StartCoroutine(HidedForSeconds(respawnTime));
            }
        }

        private IEnumerator HidedForSeconds(float seconds){
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool show)
        {
            gameObject.GetComponent<Collider>().enabled = show;
            foreach(Transform child in transform){
                child.gameObject.SetActive(show);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if(Input.GetMouseButtonDown(0)){
                Pickup(callingController.GetComponent<CombatController>());
            }
            return true;
        }

        public CursorType GetCursorType(PlayerController callingGameObject)
        {
            return CursorType.Pickup;
        }

        public void DestroyAfterTime(float time){
            Destroy(gameObject, time);
        }
    } 
}