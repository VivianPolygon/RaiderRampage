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

    //slots parent
    public Transform slotParent;
    //slots
    public Transform[] BarrelSlots;
    //will check if slots are available (not implemented yet)
    public BarrelType[] slotFillType;

    //flag for if the gun is firing
    [HideInInspector]
    public bool firing;
    //flag for reloading, used to prevent shooting while reloading
    [HideInInspector]
    public bool reloading;


    //used by functions
    private GameObject newBarrel;

    //x value is the quantity of barrels, y value is the coresponding cursor for if that barrel is the majority, Z is the cursor priority
    private Vector3 pistolBarrelData;
    private Vector3 machineGunBarrelData;
    private Vector3 shotgunBarrelData;
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

    private void FixedUpdate()
    {
        SpinBarrels();
    }

    private void Start()
    {
        //sets the cursor sprite and gets the current slots at start

        UpdateSlotCount();
        ApplyStaticGunData();
        StopFiring();
    }


    //FUNCTIONS RELATING TO THE TRACKING OF SLOTS AND THEIR CONTENTS <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

    //updates the slotcount, make sure to only put slots in slotparent
    public void UpdateSlotCount()
    {
        BarrelSlots = new Transform[slotParent.childCount];
        slotFillType = new BarrelType[slotParent.childCount];

        for (int i = 0; i < BarrelSlots.Length; i++)
        {
            BarrelSlots[i] = slotParent.GetChild(i);
        }

        GetAllBarrelTypes();

    }

    public void GetSlotBarrelType(int slotNum)
    {
        if (BarrelSlots[slotNum].childCount > 0)
        {
            if (BarrelSlots[slotNum].GetChild(0).TryGetComponent(out GunBarrel gunbarrel))
            {
                slotFillType[slotNum] = gunbarrel.barrelType;
            }
            else
            {
                Debug.LogWarning("there is an object in a gunslot that is not a barrel or does not have the Gunbarrel script");
            }
        }
        else if (BarrelSlots[slotNum].childCount <= 0)
        {
            slotFillType[slotNum] = BarrelType.Empty;
        }
        RefreshBarrelQuantitiesList();
    }

    public void GetAllBarrelTypes()
    {
        for (int i = 0; i < BarrelSlots.Length; i++)
        {
            if (BarrelSlots[i].childCount > 0)
            {
                if (BarrelSlots[i].GetChild(0).TryGetComponent(out GunBarrel gunbarrel))
                {
                    slotFillType[i] = gunbarrel.barrelType;
                }
                else
                {
                    Debug.LogWarning("there is an object in a gunslot that is not a barrel or does not have the Gunbarrel script");
                }
            }
            else
            {
                slotFillType[i] = BarrelType.Empty;
            }
        }
        RefreshBarrelQuantitiesList();
    }

    public void CreateBarrelInFirstAvailableSlot(GameObject barrelPrefab)
    {
        for (int i = 0; i < slotFillType.Length; i++)
        {
            if(slotFillType[i] == BarrelType.Empty)
            {
                newBarrel = Instantiate(barrelPrefab, BarrelSlots[i].position, BarrelSlots[i].rotation);
                newBarrel.transform.parent = BarrelSlots[i];
                newBarrel.transform.localScale = barrelPrefab.transform.localScale;

                GetSlotBarrelType(i);
                return;
            }
        }
    }

    public void DestroyBarrelInSlot(int slotnum)
    {
        if(BarrelSlots[slotnum].childCount > 0)
        {
            foreach (Transform child in BarrelSlots[slotnum])
            {
                Destroy(child.gameObject);
            }
            slotFillType[slotnum] = BarrelType.Empty;
            RefreshBarrelQuantitiesList();
        }
    }

    public void DestroyAllBarrels()
    {
        for (int i = 0; i < BarrelSlots.Length; i++)
        {
            {
                foreach (Transform child in BarrelSlots[i])
                {
                    Destroy(child.gameObject);
                }
                slotFillType[i] = BarrelType.Empty;
                RefreshBarrelQuantitiesList();
            }
        }
    }

    //FUNCTIONS RELATING TO THE CURSOR DISPLAY SYSTEM <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

    //refreshes the quantity of barrel List (used when a barrel is added to or removed from the gun)
    private void RefreshBarrelQuantitiesList()
    {
        barrelQuantitiesOrdered.Clear();

        pistolBarrelData.x = 0;
        machineGunBarrelData.x = 0;
        shotgunBarrelData.x = 0;
        rocketLauncherBarrelData.x = 0;

        for (int i = 0; i < BarrelSlots.Length; i++)
        {
            switch (slotFillType[i])
            {
                case BarrelType.Pistol:
                    pistolBarrelData.x++;
                    break;
                case BarrelType.Shotgun:
                    shotgunBarrelData.x++;
                    break;
                case BarrelType.MachineGun:
                    machineGunBarrelData.x++;
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

        barrelQuantitiesOrdered.Add(pistolBarrelData);
        barrelQuantitiesOrdered.Add(machineGunBarrelData);
        barrelQuantitiesOrdered.Add(shotgunBarrelData);
        barrelQuantitiesOrdered.Add(rocketLauncherBarrelData);

        barrelQuantitiesOrdered.Sort(BarrelQuantitiesSortComparer);

        UpdateGunCursor();
        CaculateSpinSpeedPercent();
    }

    //comparer function that sorts by quantity and priority
    private int BarrelQuantitiesSortComparer(Vector3 a, Vector3 b)
    {
        if(a.x > b.x)
        {
            return -1;
        }
        else if(a.x < b.x)
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
        if(barrelQuantitiesOrdered[0].x == barrelQuantitiesOrdered[barrelQuantitiesOrdered.Count - 1].x)
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
    private void SetCorespoondingSprites(int pistolSpriteNum, int machineGunSpriteNum, int shotGunSpriteNum, int rocketLauncherSpriteNum)
    {
        pistolBarrelData.y = pistolSpriteNum;
        machineGunBarrelData.y = machineGunSpriteNum;
        shotgunBarrelData.y = shotGunSpriteNum;
        rocketLauncherBarrelData.y = rocketLauncherSpriteNum;
    }
    //sets priority for each guntype
    private void SetCorespoondingPriority(int pistolPriorityNum, int machineGunPriorityNum, int shotGunPriorityNum, int rocketLauncherPriorityNum)
    {
        pistolBarrelData.z = pistolPriorityNum;
        machineGunBarrelData.z = machineGunPriorityNum;
        shotgunBarrelData.z = shotGunPriorityNum;
        rocketLauncherBarrelData.z = rocketLauncherPriorityNum;
    }

    //applys static data to the vector 3s that control the orginization of the dffrent barrel types cursors and priorities in cases of ties
    private void ApplyStaticGunData()
    {
        SetCorespoondingSprites
        (StaticGunData.instance.pistolCursorSpriteNumber,
        StaticGunData.instance.machineGunCursorSpriteNumber,
        StaticGunData.instance.shotgunCursorSpriteNumber,
        StaticGunData.instance.rocketLauncherCursorSpriteNumber);

        SetCorespoondingPriority
        (StaticGunData.instance.pistolPriority,
        StaticGunData.instance.machineGunPriority,
        StaticGunData.instance.shotgunPriority,
        StaticGunData.instance.rockerLauncherPriority);
    }
    //FUNCTIONS FOR GUN HEAD SPIN <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

    private void CaculateSpinSpeedPercent()
    {
        int barrelAmount = 0;

        for (int i = 0; i < BarrelSlots.Length; i++)
        {
            if (slotFillType[i] != BarrelType.Empty)
            {
                barrelAmount++;
            }
        }
        //current / (max - 1)
        spinSpeedPercent = Mathf.Clamp((barrelAmount / (float)BarrelSlots.Length) - (1 / (float)BarrelSlots.Length), 0, 1);
    }

    private void SpinBarrels()
    {
        spinningGunPiece.transform.Rotate(0, 0, (currentSpinSpeed * spinSpeedPercent) * Time.deltaTime);
    }

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

}
