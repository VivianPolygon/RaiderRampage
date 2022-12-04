using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//picks a random audio clip and sets and plays it in start
public class AudioClipRandomSelector : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    void Start()
    {
        SelectClip();
    }

    private void SelectClip()
    {
        if(audioClips != null)
        {
            audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
            audioSource.time = 0;
            audioSource.Play();
        }
    }
}
