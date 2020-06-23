using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Movement;
using RPG.Controller;

namespace RPG.Cinematics
{
    public class CinematicsControllerRemover : MonoBehaviour
    {
        private GameObject player;
        private PlayableDirector playableDirector;

        private void Awake() {
            playableDirector = GetComponent<PlayableDirector>();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void OnEnable() {
            playableDirector.played += DisableControl;
            playableDirector.stopped += EnableControl;
        }

        private void OnDisable() {
            playableDirector.played -= DisableControl;
            playableDirector.stopped -= EnableControl;
        }

        void DisableControl(PlayableDirector playableDir){
            if (player != null)
            {
                player.GetComponent<ActionScheduler>().DisableCurrentAction();
                player.GetComponent<PlayerController>().enabled = false;
                player.GetComponent<Mover>().Cancel();
            }
        }

        void EnableControl(PlayableDirector playableDir){
            if(player != null) {
                player.GetComponent<PlayerController>().enabled = true;
            }
        }
    }   
}