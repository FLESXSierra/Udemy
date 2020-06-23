using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Controller
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos() {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextPatrolPoint(i);
                Vector3 currentVector = GetPatrolPointByIndex(i);
                Gizmos.DrawSphere(currentVector, 0.25f);
                Gizmos.DrawLine(currentVector, GetPatrolPointByIndex(j));
            }
        }

        public Vector3 GetPatrolPointByIndex(int i){
            return transform.GetChild(i).position;
        }
        
        public int GetNextPatrolPoint(int i){
                return (i + 1) >= transform.childCount ? 0 : i+1;
        }
    }    
}
