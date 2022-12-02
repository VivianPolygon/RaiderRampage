using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScreenSFX : MonoBehaviour
{

    [SerializeField] private AudioSource _inventoryScreenSFXAudioSource;

    [SerializeField] private AudioClip _mergeSound;
    [SerializeField] private AudioClip _purchaseSound;
    [SerializeField] private AudioClip _dropInSlotSound;

    public static AudioSource inventoryScreenSFXAudioSource;

    public static AudioClip mergeSound;
    public static AudioClip purchaseSound;
    public static AudioClip dropInSlotSound;

    // Start is called before the first frame update
    void Start()
    {
        SetStaticClipsFromInstance();
    }

    //function called elsewhere to play a clip
    public static void PlayInventorySFXClip(AudioClip clip)
    {
        if (clip == null) { return; }

        if(inventoryScreenSFXAudioSource != null)
        {
            inventoryScreenSFXAudioSource.clip = clip;
            inventoryScreenSFXAudioSource.time = 0;
            inventoryScreenSFXAudioSource.Play();
        }
    }

    private void SetStaticClipsFromInstance()
    {
        //sets static audiosource from instance
        if (_inventoryScreenSFXAudioSource != null) { inventoryScreenSFXAudioSource = _inventoryScreenSFXAudioSource; }

        //sets static clips from the clips on this instance
        if (_mergeSound != null) { mergeSound = _mergeSound; }
        if (_purchaseSound != null) { purchaseSound = _purchaseSound; }
        if (_dropInSlotSound != null) { dropInSlotSound = _dropInSlotSound; }
    }
}
