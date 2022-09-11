using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//script that holds data for the gun that will not need to change on runtime
public class StaticGunData : MonoBehaviour
{
    public static StaticGunData instance;


    [Header("Barrel Priority")]
    //number for the corresponding cursor sprite for that barel type, displayed if its the majority
    [SerializeField]
    public int SMGSpritePriorityAndNumber;
    [SerializeField]
    public int pistolSpritePriorityAndNumber;
    [SerializeField]
    public int machineGunSpritePriorityAndNumber;
    [SerializeField]
    public int shotgunSpritePriorityAndNumber;
    [SerializeField]
    public int sniperSpritePriorityAndNumber;
    [SerializeField]
    public int rocketLauncherSpritePriorityAndNumber;

    /*
    [Header("Barrel Cursor Sprite Number")]
    //priority for the cursor, if theres a two or threeway tie in barrelquantity displays the highest priority cursor
    [SerializeField]
    public int SMGCursorSpriteNumber;
    [SerializeField]
    public int pistolCursorSpriteNumber;
    [SerializeField]
    public int machineGunCursorSpriteNumber;
    [SerializeField]
    public int shotgunCursorSpriteNumber;
    [SerializeField]
    public int sniperCursorSpriteNumber;
    [SerializeField]
    public int rocketLauncherCursorSpriteNumber;
    */
    private void Awake()
    {

        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

    }
}