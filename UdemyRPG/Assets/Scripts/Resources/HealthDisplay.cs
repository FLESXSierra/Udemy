using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resources
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] bool isHealthBar = false;
        Health health;
        
        private void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();    
        }

        private void Update() {
            float initial = health.GetInitialHealthPoints();
            float current = health.GetHealthPoints();
            if(isHealthBar){
                float newPercentage = 100*current/initial;
                GetComponent<RectTransform>().sizeDelta = new Vector2((Mathf.Round(2 * newPercentage)), 20);
            }
            else{
                GetComponent<Text>().text = string.Format("{0:0}/{1:0}",current, initial);
            }
        }
    }
}
