using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Scene{
    public class SavingWrapper : MonoBehaviour
    {
        const string saveFile = "save";
        [SerializeField] float fadeInTime = 1f;
        Fader fader;        
        private void Awake() {
            fader = FindObjectOfType<Fader>();
            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene() {
            fader.FadeOutInmediate();
            yield return GetComponent<SavingSystem>().LoadLastScene(saveFile);
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.L)){
                Load();
            }
            if(Input.GetKeyDown(KeyCode.S)){
                Save();
            }
            if(Input.GetKeyDown(KeyCode.Delete)){
                Delete();
            }
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(saveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(saveFile);
        }

        public void Delete() {
            GetComponent<SavingSystem>().Delete(saveFile);
        }
    }   
}
