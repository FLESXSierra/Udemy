using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizerSounds : MonoBehaviour
{
    [SerializeField] AudioClip[] sounds = null;
    [SerializeField] AudioSource source = null;
    
    public void PlayRandom(){
        if(sounds != null){
            int index = Random.Range(0, sounds.Length);
            AudioClip sound = sounds[index];
            GenerateSound(sound);
        }
    }

    private void GenerateSound(AudioClip sound)
    {
        source.PlayOneShot(sound, 1);
        source.Play();
    }
}
