using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverdriveGauge : MonoBehaviour
{
    private Slider[] fillSliders;
    private Color fillColor;

    public static float drainRate = 1;
    public static int activeOverdriveTier = 0;
    public static int murderScoreMax = 1000;

    public static bool atTopTier;
    public static bool overdriveActive;

    public static float fillPercent;

    private static Coroutine overdrive;

    [SerializeField] private float overdriveBaseTime;

    [SerializeField] private OverdriveTierObject[] overdriveTiers;

    //Armour piercing variables
    [Header("ArmorPiercing Variables")]
    public static bool armorPierceActive;
    [SerializeField] private float armourPierceBulletScale;
    public static float pierceScale;

    //Incindiary Variables
    [Header("Incendary Variables")]
    public static bool incendiaryActive;
    [SerializeField] private int incendiaryDamage;
    public static int _incendiaryDamage;
    [SerializeField] private float burnDuration;
    public static float _burnDuration;
    [SerializeField] private float burnFrequency;
    public static float _burnFrequency;
    [SerializeField] private GameObject fireTrailPrefab;
    public static GameObject fireTrail;
    [SerializeField] private GameObject enemyFireEffect;
    public static GameObject _enemyFireEffect;

    //Explosion Variables
    [Header("Explosion Variables")]
    public static bool explosionActive;
    [SerializeField] private GameObject explosionPrefab;
    public static GameObject explosionObject;
    [SerializeField] private int explosionDamage;
    public static int _explosionDamage;
    [SerializeField] private int explosionRange;
    public static int _explosionRange;

    //UI
    [Header("UI")]
    [SerializeField] private Text multiplyerText;
    [SerializeField] private Image buttonImage;


    void Start()
    {
        InitilizeOverdriveGauge();

        armorPierceActive = false;
        pierceScale = armourPierceBulletScale;

        incendiaryActive = false;
        _incendiaryDamage = incendiaryDamage;
        _burnDuration = burnDuration;
        _burnFrequency = burnFrequency;
        fireTrail = fireTrailPrefab;
        _enemyFireEffect = enemyFireEffect;

        explosionActive = false;
        explosionObject = explosionPrefab;
        _explosionDamage = explosionDamage;
        _explosionRange = explosionRange;
    }


    private void Update()
    {
        if(!atTopTier)
        {
            for (int i = 0; i < fillSliders.Length; i++)
            {
                fillSliders[i].value = fillPercent;
            }
            if (fillPercent >= 1)
            {
                SetOverdriveTier(activeOverdriveTier + 1);
            }
        }

    }

    private void InitilizeOverdriveGauge()
    {
        List<Slider> sliders = new List<Slider>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).TryGetComponent(out Slider slider))
            {
                sliders.Add(slider);
            }
        }

        fillSliders = sliders.ToArray();

        for (int i = 0; i < fillSliders.Length; i++)
        {
            fillSliders[i].value = 0;
        }


        SetOverdriveTier(3);

    }

    public void SetOverdriveTier(int newTier)
    {
        activeOverdriveTier = Mathf.Clamp(newTier, 0, overdriveTiers.Length - 1);

        atTopTier = false;

        //runs if at max tier
        if(activeOverdriveTier >= overdriveTiers.Length - 1)
        {
            for (int i = 0; i < fillSliders.Length; i++)
            {
                //sets the slider to 1, gauge always looks full when at highest tier
                fillSliders[i].value = 1;
            }
            //resets the fillpercent to 0, stops tracking fillpercent for updating while at max tier
            fillPercent = 0;

            atTopTier = true;
        }


        //sets the fillcolor and appopiate images
        fillColor = overdriveTiers[activeOverdriveTier].barColor;
        for (int i = 0; i < fillSliders.Length; i++)
        {
            fillSliders[i].fillRect.gameObject.GetComponent<Image>().color = fillColor;
        }
        //sets the drain rate
        drainRate = overdriveTiers[activeOverdriveTier].gaugeDrainRate;

        //sets the max murderscore for the tier
        murderScoreMax = overdriveTiers[activeOverdriveTier].nextTierMurderScoreThreshold;

        //resets murderscore apon tier up
        PlayerResourcesManager.murderScore = 0;
        UpdateFillCalculations();
        UpdateGaugeUI();
    }

    public static void UpdateFillCalculations()
    {
        fillPercent = Mathf.Clamp01(PlayerResourcesManager.murderScore / murderScoreMax);
    }

    private void SetColors(Color newColor)
    {
        fillColor = newColor;

        for (int i = 0; i < fillSliders.Length; i++)
        {
            fillSliders[i].fillRect.gameObject.GetComponent<Image>().color = fillColor;
        }
    }

    public void ActivateOverdrive()
    {
        if(activeOverdriveTier > 0)
        {
            if (overdrive == null)
            {
                overdrive = StartCoroutine(Overdrive());
            }

        }
    }

    public IEnumerator Overdrive()
    {
        overdriveActive = true;

        OverdriveTierObject.OverdriveMode overdriveMode = overdriveTiers[activeOverdriveTier].mode;

        switch (overdriveMode)
        {
            case OverdriveTierObject.OverdriveMode.None:
                break;
            case OverdriveTierObject.OverdriveMode.ArmorPierce:
                ArmorPierceActive();
                break;
            case OverdriveTierObject.OverdriveMode.Incendiary:
                IncendiaryActive();
                break;
            case OverdriveTierObject.OverdriveMode.Explosive:
                ExplosionActive();
                break;
            case OverdriveTierObject.OverdriveMode.Flower:
                break;
            default:
                break;
        }

        for (float t = 0; t < overdriveBaseTime; t += Time.deltaTime)
        {
            for (int i = 0; i < fillSliders.Length; i++)
            {
                //sets the slider to 1, gauge always looks full when at highest tier
                fillSliders[i].value = 1 - (t / overdriveBaseTime);
                SetColors(Color.Lerp(overdriveTiers[activeOverdriveTier - 1].barColor, Color.white, Mathf.PingPong(t, 1)));
            }
            yield return null;
        }

        switch (overdriveMode)
        {
            case OverdriveTierObject.OverdriveMode.None:
                break;
            case OverdriveTierObject.OverdriveMode.ArmorPierce:
                ArmorPierceInactive();
                break;
            case OverdriveTierObject.OverdriveMode.Incendiary:
                IncendiaryInactive();
                break;
            case OverdriveTierObject.OverdriveMode.Explosive:
                ExplosionInactive();
                break;
            case OverdriveTierObject.OverdriveMode.Flower:
                break;
            default:
                break;
        }

        SetOverdriveTier(0);
        overdrive = null;
        overdriveActive = false;
    }

    //Overdrive effects Functions

    public static void ArmorPierceActive()
    {
        armorPierceActive = true;
    }
    public static void ArmorPierceInactive()
    {
        armorPierceActive = false;
    }

    public static void IncendiaryActive()
    {
        incendiaryActive = true;
    }
    public static void IncendiaryInactive()
    {
        incendiaryActive = false;
    }

    public static void ExplosionActive()
    {
        explosionActive = true;
    }
    public static void ExplosionInactive()
    {
        explosionActive = false;
    }

    public void UpdateGaugeUI()
    {
        if(buttonImage != null && multiplyerText != null)
        {
            if (activeOverdriveTier <= 0)
            {
                buttonImage.gameObject.GetComponent<Button>().interactable = false;
            }
            else
            {
                buttonImage.gameObject.GetComponent<Button>().interactable = true;
            }

            multiplyerText.text = (activeOverdriveTier.ToString() + "x");
            buttonImage.color = (overdriveTiers[activeOverdriveTier].barColor);
        }
    }


}
