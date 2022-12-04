using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSFXAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;

    public void PlayClip(int clipNum)
    {
        if(audioSource != null)
        {
            //allows for the clip to play fully and not get overwritten by the same clip, needed for sliders
            if (audioSource.clip != null)
            {
                if (audioSource.isPlaying && audioSource.clip == clips[Mathf.Clamp(clipNum, 0, clips.Length)])
                {
                    return;
                }
            }

            audioSource.clip = clips[Mathf.Clamp(clipNum, 0, clips.Length)];
            audioSource.time = 0;
            audioSource.Play();
        }
    }
}
