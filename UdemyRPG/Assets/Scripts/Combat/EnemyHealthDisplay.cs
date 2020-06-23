using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resources
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] bool isHealthBar = false;
        [SerializeField] float initialYPos = 500;
        CombatController combatController;
        bool notHided = true;
        
        private void Awake() {
            combatController = GameObject.FindWithTag("Player").GetComponent<CombatController>();    
        }

        private void Update() {
            string healthValue = "N/A";
            bool isVisible = combatController != null && combatController.GetTargetHealth() != null;
            if(isVisible){
                showEnemyUI(true);
                notHided = true;
                float initial = combatController.GetTargetHealth().GetInitialHealthPoints();
                float current = combatController.GetTargetHealth().GetHealthPoints();
                if(isHealthBar){
                    float newPercentage = 100 * current / initial;
                    GetComponent<RectTransform>().sizeDelta = new Vector2((Mathf.Round(14 * newPercentage/10)), 16);
                }
                else{
                    healthValue = string.Format("{0:0}/{1:0}", current, initial);
                    GetComponent<Text>().text = healthValue;
                }
            }
            else if(notHided){
                showEnemyUI(false);
                notHided = false;
            }
        }

        private void showEnemyUI(bool show)
        {
            RectTransform transformUIEnemy = GameObject.FindWithTag("UI_ENEMY").GetComponent<RectTransform>();
            if(show){
                transformUIEnemy.position = new Vector3(transformUIEnemy.position.x, initialYPos, transformUIEnemy.position.z);
            }
            else{
                transformUIEnemy.position = new Vector3(transformUIEnemy.position.x, 100000, transformUIEnemy.position.z);
            }
        }
    }
}
