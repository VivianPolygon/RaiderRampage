using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExplosionAudioManager : MonoBehaviour
{
    public static ExplosionAudioManager instance;

    [SerializeField] private AudioMixerGroup output;
    [SerializeField] private float volume;

    [SerializeField] private int explosionSoundsMax;

    private GameObject sourceObject;

    public Transform[] audioSourceTransforms;
    public AudioSource[] explosionAudioSource; 

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        {
            instance = this;
        }
    }

    private void Start()
    {
        volume = Mathf.Clamp01(volume);

        InitilizeAudios();
    }
    private void InitilizeAudios()
    {
        //dosent run if there is no audios to be made
        if (explosionSoundsMax > 0)
        {
            sourceObject = new GameObject();

            audioSourceTransforms = new Transform[explosionSoundsMax];
            explosionAudioSource = new AudioSource[explosionSoundsMax];

            GameObject newAudioSource;
            for (int i = 0; i < explosionSoundsMax; i++)
            {
                //instantiets the empty
                newAudioSource = Instantiate(sourceObject, transform.position, transform.rotation);
                //childs it to this
                newAudioSource.transform.parent = this.transform;
                //adds an audio source
                newAudioSource.AddComponent(typeof(AudioSource));

                //sets it to the arrays
                audioSourceTransforms[i] = newAudioSource.transform;
                explosionAudioSource[i] = newAudioSource.GetComponent<AudioSource>();
            }
            //sets up the audio sources
            for (int i = 0; i < explosionAudioSource.Length; i++)
            {
                explosionAudioSource[i].outputAudioMixerGroup = output;
                explosionAudioSource[i].volume = volume;
                explosionAudioSource[i].playOnAwake = false;
                explosionAudioSource[i].loop = false;
            }
        }
        else
        {
            Debug.LogWarning(name + "please set explosionSoundsMax to greater than zero for explosion SFX to work propperly");
            Destroy(this);
        }
    }

    //plays the explosion sound at the given point using the audio source most avalaible to play it
    public void PlayExplosion(AudioClip explosionAudioClip, Vector3 explosionPosition)
    {
        if(instance != null)
        {
            int index = -1;

            //will find the first audio thats not playing
            for (int i = 0; i < explosionAudioSource.Length; i++)
            {
                if (!explosionAudioSource[i].isPlaying)
                {
                    index = i;
                    break;
                }
            }

            //returns out if there is no available audio
            if(index < 0)
            {
                return;
            }

            //sets the objects position to the point of the explosion
            audioSourceTransforms[index].position = explosionPosition;

            //plays the audio
            explosionAudioSource[index].time = 0;
            explosionAudioSource[index].clip = explosionAudioClip;
            explosionAudioSource[index].Play();
        }
    }
}
