using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;

        public void StartAction(IAction action){
            if(action != currentAction){
                if(currentAction !=null){
                    currentAction.Cancel();
                }
                currentAction = action;
            }
        }

        public void DisableCurrentAction(){
            StartAction(null);
        }
    }
}
