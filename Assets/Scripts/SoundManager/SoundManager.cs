﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance;

    [Header("Audio Sources")]
    public GameObject testSound;
    public GameManager secondSound;

    private void Awake() {
        instance = this;
    }


    //this method needs a prefab which have an audio source
    //when call this method, audio source needs to be checked null or not to avoid having two same references of audio source
    public AudioSource audioSourceToPlay(GameObject audioSource, float timeToStop, bool loop, float volume, Transform parentTransform, Vector3 newPosition) {   

        GameObject audioObject = (GameObject)Instantiate(audioSource, newPosition, new Quaternion());
        AudioSource temp = audioObject.GetComponent<AudioSource>();

        if(parentTransform != null) {
            audioObject.transform.SetParent(parentTransform);
        }
        temp.loop = loop;
        temp.volume = volume;
        temp.Play();

        if (timeToStop > 0)                                     //if time = 0, do not stop audio
            StartCoroutine(waitForSeconds(temp, timeToStop));

        StartCoroutine(DestroyWhenStop(temp));                 //destroy game object

        return temp;
    }


    //stop audio

    public void stopAudio(AudioSource audio) {      
        if(audio != null) {
            audio.Stop();
        }               
    }


    //wait audio to stop then destroy game object which has audio source
    IEnumerator DestroyWhenStop(AudioSource audio) { 

        yield return new WaitUntil(() => !audio.isPlaying);

        if(audio != null)
            Destroy(audio.gameObject);
    }

    IEnumerator waitForSeconds(AudioSource audio, float time) {

        yield return new WaitForSeconds(time);
        if(audio != null)
            audio.Stop();
    }
}
