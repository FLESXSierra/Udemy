using UnityEngine;
using RPG.Resources;
using RPG.Controller;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class TargetCombat : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType(PlayerController callingController)
        {
            CombatController combatController = callingController.GetComponent<CombatController>();
            if(combatController != null){
                return combatController.GetWeaponCursorType();
            }
            else{
                return CursorType.Combat;
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            bool isInCombat = false;
            CombatController combatController = callingController.GetComponent<CombatController>();
            if (combatController != null && !combatController.IsTargetDead(gameObject))
            {
                isInCombat = true;
                if (Input.GetMouseButtonDown(0))
                {
                    combatController.Attack(gameObject);
                }
            }
            return isInCombat;
        }
    }
}
