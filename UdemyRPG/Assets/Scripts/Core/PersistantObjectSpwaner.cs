using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Core
{
    public class PersistantObjectSpwaner : MonoBehaviour
    {
        [SerializeField] GameObject persistantObjectPrefab;
        static bool hasSpwaned;

        private void Awake() {
            if(!hasSpwaned){
                spawnPersistantObject();
                hasSpwaned = true;
            }
        }

        private void spawnPersistantObject()
        {
            GameObject persistantObject = Instantiate(persistantObjectPrefab);
            DontDestroyOnLoad(persistantObject);
        }
    }
}