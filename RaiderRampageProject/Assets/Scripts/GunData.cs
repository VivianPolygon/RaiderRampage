using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Script that holds data of the gun as a whole
public class GunData : MonoBehaviour
{
    //singleton instance
    public static GunData instance;

    //base body model for the gun
    public GameObject gunModelBody;


    //Reload delay in second
    public float reloadTime;
    [SerializeField]
    private int startingGunHead;


    [Header("Cursor Information")]
    //cursor image, and the sprites for it to hold
    public Image cursorImage;
    public Sprite[] cursorSprites;
    //cursor color tints
    [SerializeField]
    private Color cursorInactiveColor;
    [SerializeField]
    private Color cursorFiringColor;

    //position of the cursor in worldspace
    [HideInInspector]
    public Vector3 cursorPositon;

    //flag for if the gun is firing
    [HideInInspector]
    public bool firing;
    //flag for reloading, used to prevent shooting while reloading
    [HideInInspector]
    public bool reloading;


    //used by functions
    private GameObject newBarrel;

    //x value is the quantity of barrels, y value is the coresponding cursor for if that barrel is the majority, Z is the cursor priority
    private Vector3 SMGBarrelData;
    private Vector3 pistolBarrelData;
    private Vector3 machineGunBarrelData;
    private Vector3 shotgunBarrelData;
    private Vector3 sniperBarrelData;
    private Vector3 rocketLauncherBarrelData;


    //ordered list based on the x values of the above vector2s
    [SerializeField]
    private List<Vector3> barrelQuantitiesOrdered;

    [Header("Related to Gun Spin")]
    //object to spin
    public GameObject spinningGunPiece;
    //spin speed for the gun
    public float maxSpinSpeed;
    //used for spin speed caculations dependent on barrel ammount
    private float spinSpeedPercent;
    [SerializeField]
    private float spinSpeedStartupTime;

    private float currentSpinSpeed;

    [HideInInspector]
    public int currentHeadNumber;



    private void Awake()
    {
        //establishes singleton
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        currentHeadNumber = startingGunHead;
    }

    private void Update()
    {
        SpinBarrels();
    }

    private void Start()
    {
        //sets the cursor sprite and gets the current slots at start

        ApplyStaticGunData();
        StopFiring();

    }


    //FUNCTIONS RELATING TO THE TRACKING OF SLOTS AND THEIR CONTENTS <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


    //FUNCTIONS RELATING TO THE CURSOR DISPLAY SYSTEM <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

    //refreshes the quantity of barrel List (used when a barrel is added to or removed from the gun)

    private void RefreshBarrelQuantitiesList()
    {
        barrelQuantitiesOrdered.Clear();

        SMGBarrelData.x = 0;
        pistolBarrelData.x = 0;
        machineGunBarrelData.x = 0;
        shotgunBarrelData.x = 0;
        sniperBarrelData.x = 0;
        rocketLauncherBarrelData.x = 0;

        for (int i = 0; i < StaticGunData.instance.workshopGunHeads[currentHeadNumber].headSlots.Length; i++)
        {
            switch (StaticGunData.instance.workshopGunHeads[currentHeadNumber].headSlots[i].slotType)
            {
                case BarrelType.SMG:
                    SMGBarrelData.x++;
                    break;
                case BarrelType.Pistol:
                    pistolBarrelData.x++;
                    break;

                case BarrelType.Shotgun:
                    shotgunBarrelData.x++;
                    break;

                case BarrelType.MachineGun:
                    machineGunBarrelData.x++;
                    break;
                case BarrelType.Sniper:
                    sniperBarrelData.x++;
                    break;

                case BarrelType.RocketLauncher:
                    rocketLauncherBarrelData.x++;
                    break;
                case BarrelType.Empty:
                    break;
                default:
                    break;
            }
        }

        barrelQuantitiesOrdered.Add(SMGBarrelData);
        barrelQuantitiesOrdered.Add(pistolBarrelData);
        barrelQuantitiesOrdered.Add(machineGunBarrelData);
        barrelQuantitiesOrdered.Add(shotgunBarrelData);
        barrelQuantitiesOrdered.Add(sniperBarrelData);
        barrelQuantitiesOrdered.Add(rocketLauncherBarrelData);

        barrelQuantitiesOrdered.Sort(BarrelQuantitiesSortComparer);

        UpdateGunCursor();
        CaculateSpinSpeedPercent();
    }

    //comparer function that sorts by quantity and priority
    private int BarrelQuantitiesSortComparer(Vector3 a, Vector3 b)
    {
        if (a.x > b.x)
        {
            return -1;
        }
        else if (a.x < b.x)
        {
            return 1;
        }
        if (a.z > b.z)
        {
            return -1;
        }
        else if (a.z < b.z)
        {
            return 1;
        }
        return 0;
    }


    //determines what to update the sprite too depending on the barrelQuantitiesOrdered list
    private void UpdateGunCursor()
    {
        //checks if the bottom and top of list are the same, if they are uses the default sprite
        if (barrelQuantitiesOrdered[0].x == barrelQuantitiesOrdered[barrelQuantitiesOrdered.Count - 1].x)
        {
            UpdateCursorSprite(0);
        }
        //checks the top of the lists y component, which coresponds to a sprite on the cursorSprites array
        else
        {
            UpdateCursorSprite((int)barrelQuantitiesOrdered[0].y);
        }

    }
    //updates the cursor sprite
    public void UpdateCursorSprite(int spriteNum)
    {
        if (spriteNum > (cursorSprites.Length - 1))
        {
            spriteNum = cursorSprites.Length - 1;
            Debug.LogWarning("Inputed Cursor Sprite number was above the top of the array and has been set to the highest number in the array");
        }
        if (spriteNum < 0)
        {
            spriteNum = 0;
            Debug.LogWarning("Inputed Cursor Sprite number was below 0, and has been set to 0");
        }
        cursorImage.sprite = cursorSprites[spriteNum];
    }

    //sets coresponding sprite for each guntype
    private void SetCorespoondingSpritesAndPriority(int SMGSpriteNum, int pistolSpriteNum, int machineGunSpriteNum, int shotGunSpriteNum, int sniperSpriteNum, int rocketLauncherSpriteNum)
    {
        SMGBarrelData.y = SMGSpriteNum;
        pistolBarrelData.y = pistolSpriteNum;
        machineGunBarrelData.y = machineGunSpriteNum;
        shotgunBarrelData.y = shotGunSpriteNum;
        sniperBarrelData.y = sniperSpriteNum;
        rocketLauncherBarrelData.y = rocketLauncherSpriteNum;

        SMGBarrelData.z = SMGSpriteNum;
        pistolBarrelData.z = pistolSpriteNum;
        machineGunBarrelData.z = machineGunSpriteNum;
        shotgunBarrelData.z = shotGunSpriteNum;
        sniperBarrelData.z = sniperSpriteNum;
        rocketLauncherBarrelData.z = rocketLauncherSpriteNum;
    }

    //applys static data to the vector 3s that control the orginization of the diffrent barrel types cursors and priorities in cases of ties
    private void ApplyStaticGunData()
    {
        SetCorespoondingSpritesAndPriority
        (StaticGunData.instance.SMGSpritePriorityAndNumber,
        StaticGunData.instance.pistolSpritePriorityAndNumber,
        StaticGunData.instance.machineGunSpritePriorityAndNumber,
        StaticGunData.instance.shotgunSpritePriorityAndNumber,
        StaticGunData.instance.sniperSpritePriorityAndNumber,
        StaticGunData.instance.rocketLauncherSpritePriorityAndNumber);
    }
    //FUNCTIONS FOR GUN HEAD SPIN <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

    private void CaculateSpinSpeedPercent()
    {
        int barrelAmount = 0;
        int length = StaticGunData.instance.workshopGunHeads[currentHeadNumber].headSlots.Length;

        for (int i = 0; i < length; i++)
        {
            if (StaticGunData.instance.workshopGunHeads[currentHeadNumber].headSlots[i].slotType != BarrelType.Empty)
            {
                barrelAmount++;
            }
        }
        //current / (max - 1)
        spinSpeedPercent = Mathf.Clamp((barrelAmount / (float)length) - (1 / (float)length), 0, 1);
    }

    //spins the gun barrels depending on quantity of barrels in relation to the total
    private void SpinBarrels()
    {
        if (spinSpeedPercent > 0)
        {
            spinningGunPiece.transform.Rotate(0, 0, (currentSpinSpeed * spinSpeedPercent) * Time.deltaTime);
        }
    }

    //startup coroutine for the barrel spin
    private IEnumerator SpinStartup()
    {
        float currentSpeed = currentSpinSpeed;
        for (float i = 0; i < spinSpeedStartupTime; i += Time.deltaTime)
        {
            currentSpinSpeed = Mathf.Lerp(currentSpeed, maxSpinSpeed, i / spinSpeedStartupTime);
            yield return null;
        }
        currentSpinSpeed = maxSpinSpeed;
    }
    //slowdown coroutine for the barrel spin
    private IEnumerator SpinSlowdown()
    {
        float currentSpeed = currentSpinSpeed;
        for (float i = 0; i < spinSpeedStartupTime; i += Time.deltaTime)
        {
            currentSpinSpeed = Mathf.Lerp(currentSpinSpeed, 0, i / spinSpeedStartupTime);
            yield return null;
        }
        currentSpinSpeed = 0;
    }

    //FUNCTIONS TRIGGERED BY BUTTONS <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

    //used by the button to set flag for firing
    public void startFiring()
    {
        firing = true;
        StartCoroutine(SpinStartup());
        cursorImage.color = cursorFiringColor;
    }
    public void StopFiring()
    {
        firing = false;
        StartCoroutine(SpinSlowdown());
        cursorImage.color = cursorInactiveColor;
    }

    //FUNCTIONS USED FOR SWAPING CURRENT GUNHEAD <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    //changes actuve gunhead, INITILIZED THROUGH THE GAME STATE MANAGER
    public void SwitchGunHeads(int headNumber)
    {
        headNumber = Mathf.Clamp(headNumber, 0, StaticGunData.instance.workshopGunHeads.Length - 1);

        for (int i = 0; i < StaticGunData.instance.gunSlots.Length; i++)
        {
            StaticGunData.instance.gunSlots[i].UpdateSlotBarrel(headNumber);
        }

        currentHeadNumber = headNumber;
        RefreshBarrelQuantitiesList();
    }

}