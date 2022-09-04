using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunData : MonoBehaviour
{
    public static GunData instance;

    public GameObject gunModel;
    public int initialCursorSpriteNumber;

    public Image cursorImage;
    public Sprite[] cursorSprites;

    [HideInInspector]
    public Vector3 cursorPositon;

    public Transform slotParent;
    public Transform[] BarrelSlots;

    public bool[] slotAvailability;

    public bool firing;

    private void Awake()
    {
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
        UpdateCursorSprite(initialCursorSpriteNumber);
        UpdateSlotCount();
    }

    public void PointGunAtCursor()
    {
        gunModel.transform.LookAt(cursorPositon, Camera.main.transform.up);
    }
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

    public void UpdateSlotCount()
    {
        BarrelSlots = new Transform[slotParent.childCount];

        for (int i = 0; i < BarrelSlots.Length; i++)
        {
            BarrelSlots[i] = slotParent.GetChild(i);
        }

    }

    public void startFiring()
    {
        firing = true;
    }
    public void StopFiring()
    {
        firing = false;
    }

}
