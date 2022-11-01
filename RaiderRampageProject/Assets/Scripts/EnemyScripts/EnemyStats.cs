using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField]
    private int health = 20;
    [SerializeField]
    private int armour = 0;


    [Header("Damage Indicater Material, Temporary")]
    [SerializeField]
    private Material normalMat;
    [SerializeField]
    private Material damageMat;
    [SerializeField]
    private SkinnedMeshRenderer materialRenderer;

    [Header("Animation Related")]
    [SerializeField] private float injuredThreshold = 0.4f; // 0-1;


    //used for burn status
    private GameObject burnEffect;
    private Coroutine burn;
    private WaitForSeconds burnFrequency;

    //used to check if injured
    private int maxhealth;

    [Header("Ragdoll Model Spawned on Death")]
    [SerializeField] private GameObject ragdollModel;

    private void Start()
    {
        if (materialRenderer != null)
        {
            materialRenderer.material = normalMat;
        }
        else
        {
            Debug.LogWarning("Enenmy: " + name + "Does not have a mesh renderer attatched as refrence in its enemyStats script, make sure this is assigned via the inspector, enemy destroyed");
            Destroy(this.gameObject);
        }

        maxhealth = health;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Bullet bullet))
        {

            TakeDamage(bullet.damage);

            Destroy(bullet.gameObject);
        }
    }


    public void TakeDamage(int damageAmount)
    {
        if(OverdriveGauge.armorPierceActive)
        {
            health -= damageAmount;
        }
        else
        {
            health -= Mathf.Clamp((damageAmount - armour), 1, damageAmount);
        }

        if(OverdriveGauge.incendiaryActive)
        {
            InflictBurn(OverdriveGauge._burnDuration, OverdriveGauge._incendiaryDamage, OverdriveGauge._burnFrequency, OverdriveGauge._enemyFireEffect);
        }

        CheckDeathOrDamage();
    }

    private void CheckDeathOrDamage()
    {
        if (health <= 0)
        {
            GetComponent<EnemyAnimatorController>().SetDeath(true);

            GameObject ragdoll = Instantiate(ragdollModel, transform.GetChild(0).position, transform.rotation);
            ragdoll.GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().velocity, ForceMode.Impulse);
            Destroy(ragdoll, 5);
            Destroy(this.gameObject);


        }
        else
        {
            StartCoroutine(IndicateDamage());

            if (injuredThreshold >=  (float)health / maxhealth)
            {
                GetComponent<EnemyAnimatorController>().SetInjured(true);
            }
        }
    }

    private IEnumerator IndicateDamage()
    {
        for (int i = 0; i < 20; i++)
        {
            if(i % 2 == 0)
            {
                materialRenderer.material = damageMat;
            }
            else
            {
                materialRenderer.material = normalMat;
            }
            yield return new WaitForEndOfFrame();
        }
        materialRenderer.material = normalMat;
    }

    private void InflictBurn(float duration, int burndamage, float frequency, GameObject burnEffectPrefab)
    {
        if(burn != null)
        {
            StopCoroutine(burn);

            if(burnEffect != null)
            {
                Destroy(burnEffect);
            }
        }

        burn = StartCoroutine(BurnDamage(duration, burndamage, frequency, burnEffectPrefab));
    }

    private IEnumerator BurnDamage(float duration, int burnDamage, float frequency, GameObject burnEffectPrefab)
    {
        burnEffect = Instantiate(burnEffectPrefab, transform.position, transform.rotation);
        burnEffect.transform.parent = transform;

        burnFrequency = new WaitForSeconds(frequency);

        int hitsTaken = 1;

        print("Enemy Burned");

        for (float i = 0; i < duration; i += Time.deltaTime)
        {

            hitsTaken++;

            health -= burnDamage;

            CheckDeathOrDamage();

            yield return burnFrequency;
        }

        if(burnEffect != null)
        {
            Destroy(burnEffect);
        }
    }


    private void OnDestroy()
    {
        //adds the murder score from enemies death
        if(!OverdriveGauge.atTopTier)
        {
            PlayerResourcesManager.murderScore += maxhealth;
        }

        UIData.instance.SetWaveBar(WaveTracker.instance.waveSpawner.UpdateWaveProgress(-1));
    }
}
