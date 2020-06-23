using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPreFab = null;

        public void Spawn(float damage){
            if(damageTextPreFab != null) {
                DamageText instance = Instantiate<DamageText>(damageTextPreFab, transform);
                instance.SetValue(damage);
            }
        }
    }
}