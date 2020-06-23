using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Controller;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.Scene
{
    public class Portal : MonoBehaviour
    {

        enum DestinationIdentifier{
            A,B,C,D,E,F
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPortal;
        [SerializeField] DestinationIdentifier destination = DestinationIdentifier.A;
        [SerializeField] float waitInFadeOut = 1f;

        private void OnTriggerEnter(Collider other) {
            if(other.tag == "Player"){
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition(){
            if(sceneToLoad < 0){
                Debug.LogError("Scene to load is not set.");
                yield break;
            }
            DontDestroyOnLoad(gameObject);

            // Disable Player
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<ActionScheduler>().DisableCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<Mover>().Cancel();

            Fader fader = FindObjectOfType<Fader>();

            yield return fader.FadeOut(0.5f);

            //save current lvl
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            // Disable Player
            player = GameObject.FindWithTag("Player");
            player.GetComponent<ActionScheduler>().DisableCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<Mover>().Cancel();

            //load lvl
            wrapper.Load();
            Portal otherPortal = GetOtherPortal();
            updatePlayer(otherPortal);
            //save current lvl of new scene
            wrapper.Save();
            yield return new WaitForSeconds(waitInFadeOut);
            //Enable Player
            player.GetComponent<PlayerController>().enabled = true;
            yield return fader.FadeIn(0.5f);


            Destroy(gameObject);
        }

        private void updatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPortal.position);
            player.transform.rotation = otherPortal.spawnPortal.rotation;
        }

        private Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>()){
                if(portal == this || portal.destination != destination){
                    continue;
                }
                return portal;
            }
            return null;
        }
    }   
}