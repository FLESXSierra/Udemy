using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Scene
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentActiveCoroutine = null;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public Coroutine FadeOut(float time){
            return Fade(1, time);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        private Coroutine Fade(int target, float time)
        {
            if (currentActiveCoroutine != null)
            {
                StopCoroutine(currentActiveCoroutine);
            }
            currentActiveCoroutine = StartCoroutine(FadeRoutine(target, time));
            return currentActiveCoroutine;
        }

        private IEnumerator FadeRoutine(float target, float time){
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

        public void FadeOutInmediate(){
            canvasGroup.alpha = 1;
        }
    }   
}