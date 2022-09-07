using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//script that holds data for the gun that will not need to change on runtime
public class StaticGunData : MonoBehaviour
{
    public static StaticGunData instance;

    [Header("Barrel Priority")]
    //number for the corresponding cursor sprite for that barel type, displayed if its the majority
    [SerializeField]
    public int pistolPriority;
    [SerializeField]
    public int machineGunPriority;
    [SerializeField]
    public int shotgunPriority;
    [SerializeField]
    public int rockerLauncherPriority;

    [Header("Barrel Cursor Sprite Number")]
    //priority for the cursor, if theres a two or threeway tie in barrelquantity displays the highest priority cursor
    [SerializeField]
    public int pistolCursorSpriteNumber;
    [SerializeField]
    public int machineGunCursorSpriteNumber;
    [SerializeField]
    public int shotgunCursorSpriteNumber;
    [SerializeField]
    public int rocketLauncherCursorSpriteNumber;

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
