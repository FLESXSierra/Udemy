using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Controller
{
    public class SpawnDropItems : MonoBehaviour, IDropItem
    {
        [SerializeField] Pickups dropItem = null;
        [SerializeField] float destroyAfterTime = 60f;

        public void DropItem(){
            if(dropItem !=null){
                Pickups pickup = Instantiate(dropItem, transform);
                pickup.RemoveAfterPickup();
                pickup.DestroyAfterTime(destroyAfterTime);
            } 
        }
    }
}
