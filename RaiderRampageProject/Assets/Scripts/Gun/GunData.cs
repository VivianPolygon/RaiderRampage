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
    //cursor image
    public Image cursorImage;
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


    //x value is the quantity of barrels, y value is the spritesheet number for the cursorsprite
    private Vector2 SMGBarrelData;
    private Vector2 pistolBarrelData;
    private Vector2 machineGunBarrelData;
    private Vector2 shotgunBarrelData;
    private Vector2 sniperBarrelData;
    private Vector2 rocketLauncherBarrelData;


    //ordered list based on the x values of the above vector2s
    [SerializeField]
    private List<Vector2> barrelQuantitiesOrdered;

    [Header("Related to Gun Spin")]
    //object to spin
    public GameObject spinningGunPiece;
    //spin speed for the gun
    public float maxSpinSpeed;
    //used for spin speed caculations dependent on barrel ammount, also used for spread caculations in GunBarrel
    public float spinSpeedPercent;
    [SerializeField]
    private float spinSpeedStartupTime;

    private float currentSpinSpeed;

    [HideInInspector]
    public int currentHeadNumber;

    //total amount of gunheads, defaulted to 3 for now
    private int gunHeadQuantity;

    [Header("Gun Animator")]
    [SerializeField] private Animator gunAnim;
    public static Animator _gunAnim;

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
        gunHeadQuantity = 3;

        _gunAnim = gunAnim;
        if(_gunAnim == null)
        {
            Debug.LogWarning("The gun animator is null, double check that GunData located on: " + name + "has its field in the inspector for animator set to an animator");
        }
    }

    private void Update()
    {
        SpinBarrels();
    }

    private void Start()
    {
        ApplyStaticGunData();
        RefreshBarrelQuantitiesList();
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
    private int BarrelQuantitiesSortComparer(Vector2 a, Vector2 b)
    {
        if (a.x > b.x)
        {
            return -1;
        }
        else if (a.x < b.x)
        {
            return 1;
        }
        if (a.y > b.y)
        {
            return -1;
        }
        else if (a.y < b.y)
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
            cursorImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.neutralReticle);
        }
        //checks the top of the lists y component, which coresponds to a sprite on the cursorSprites array
        else
        {
            cursorImage.sprite = UIData.instance.SetSpriteFromLargeSheet((int)barrelQuantitiesOrdered[0].y);
        }
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

    //spins the gun barrels depending on quantity of barrels in relation to the total, spins faster depending on fire rate mult
    private void SpinBarrels()
    {
        if (spinSpeedPercent > 0)
        {
            spinningGunPiece.transform.Rotate(0, 0, ((currentSpinSpeed * spinSpeedPercent) * PlayerResourcesManager.fireSpeedMult) * Time.deltaTime);

            //updates the speed of the chain shader
            ChainMovement.UpdateChainSpeed(currentSpinSpeed * spinSpeedPercent);
            //updates the speed of the recoil Animation
            _gunAnim.SetFloat("RecoilSpeed", spinSpeedPercent);
        }
        else
        {
            ChainMovement.UpdateChainSpeed(0);
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
    public void StartFiring()
    {
        firing = true;
        StartCoroutine(SpinStartup());
        UIData.instance.fireButtonImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.activeFireButtonSpriteNumber);
        cursorImage.color = cursorFiringColor;

        //sets the recoil animation
        _gunAnim.SetBool("Firing", true);
    }
    public void StopFiring()
    {
        firing = false;
        StartCoroutine(SpinSlowdown());
        UIData.instance.fireButtonImage.sprite = UIData.instance.SetSpriteFromLargeSheet(UIData.instance.inactiveFireButtonSpriteNumber);
        cursorImage.color = cursorInactiveColor;

        //sets the recoil animation
        _gunAnim.SetBool("Firing", false);
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

    //used on arrow buttons to change gunhead
    public void ScrollGunHead(int direction)
    {
        direction = Mathf.Clamp(direction, -1, 1);
        if(currentHeadNumber + direction > gunHeadQuantity - 1)
        {
            SwitchGunHeads(0);
        }
        else if(currentHeadNumber + direction < 0)
        {
            SwitchGunHeads(gunHeadQuantity - 1);
        }
        else
        {
            SwitchGunHeads(Mathf.Clamp(currentHeadNumber + direction, 0, gunHeadQuantity - 1));
        }

        for (int i = 0; i < gunHeadQuantity; i++)
        {
            if(i == currentHeadNumber)
            {
                UIData.instance.gunheadActiveImages[i].color = UIData.instance.gunheadImageActiveColor;
            }
            else
            {
                UIData.instance.gunheadActiveImages[i].color = UIData.instance.gunheadImageInactiveColor;
            }
        }

    } 
}