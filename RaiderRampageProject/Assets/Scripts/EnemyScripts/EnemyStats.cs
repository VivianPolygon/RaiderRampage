using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField]
    private int health = 20;
    [SerializeField]
    private int armour = 0;
    [SerializeField]
    private int damage = 5;

    [Header("Damage Indicater Material, Temporary")]
    [SerializeField]
    private Material damageMat;
    private Material normalMat;


    //used for burn status
    private GameObject burnEffect;
    private Coroutine burn;
    private WaitForSeconds burnFrequency;

    private void Start()
    {
        normalMat = GetComponent<MeshRenderer>().material;
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

        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            StartCoroutine(IndicateDamage());
        }
    }

    public void DamageBarricade()
    {
        Barricade.instance.BarricadeTakeDamage(damage);
    }

    private IEnumerator IndicateDamage()
    {
        for (int i = 0; i < 20; i++)
        {
            if(i % 2 == 0)
            {
                gameObject.GetComponent<MeshRenderer>().material = damageMat;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material = normalMat;
            }
            yield return new WaitForEndOfFrame();
        }
        gameObject.GetComponent<MeshRenderer>().material = normalMat;
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

            if (health <= 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                StartCoroutine(IndicateDamage());
            }

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
            PlayerResourcesManager.murderScore += damage;
        }

        UIData.instance.SetWaveBar(WaveTracker.instance.waveSpawner.UpdateWaveProgress(-1));
    }
}
