using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Script that holds data of the gun as a whole
public class GunData : MonoBehaviour
{
    //singleton instance
    public static GunData instance;

    //base model for the gun
    public GameObject gunModel;
    //starting cursor in the array
    public int initialCursorSpriteNumber;

    //cursor image, and the sprites for it to hold
    public Image cursorImage;
    public Sprite[] cursorSprites;

    //position of the cursor in worldspace
    [HideInInspector]
    public Vector3 cursorPositon;

    //slots parent
    public Transform slotParent;
    //slots
    public Transform[] BarrelSlots;
    //will check if slots are available (not implemented yet)
    public bool[] slotAvailability;
    //flag for if the gun is firing
    public bool firing;

    private void Awake()
    {
        //establishes singleton
        if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        //sets the cursor sprite and gets the current slots at start
        UpdateCursorSprite(initialCursorSpriteNumber);
        UpdateSlotCount();
    }

    //orients the gun to point towards the cursor
    public void PointGunAtCursor()
    {
        gunModel.transform.LookAt(cursorPositon, Camera.main.transform.up);
    }
    //updates the cursor sprite
    public void UpdateCursorSprite(int spriteNum)
    {
        if(spriteNum > (cursorSprites.Length - 1))
        {
            spriteNum = cursorSprites.Length - 1;
            Debug.LogWarning("Inputed Cursor Sprite number was above the top of the array and has been set to the highest number in the array");
        }
        if(spriteNum < 0)
        {
            spriteNum = 0;
            Debug.LogWarning("Inputed Cursor Sprite number was below 0, and has been set to 0");
        }
        cursorImage.sprite = cursorSprites[spriteNum];
    }

    //updates the slotcount, make sure to only put slots in slotparent
    public void UpdateSlotCount()
    {
        BarrelSlots = new Transform[slotParent.childCount];

        for (int i = 0; i < BarrelSlots.Length; i++)
        {
            BarrelSlots[i] = slotParent.GetChild(i);
        }

    }

    //used by the button to set flag for firing
    public void startFiring()
    {
        firing = true;
    }
    public void StopFiring()
    {
        firing = false;
    }

}
