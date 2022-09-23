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

    [Header("Damage Indicater Material, Temporary")]
    [SerializeField]
    private Material damageMat;
    private Material normalMat;

    private void Start()
    {
        normalMat = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Bullet bullet))
        {
            health -= Mathf.Clamp((bullet.damage - armour), 1, bullet.damage);
            TakeDamage();

            Destroy(bullet.gameObject);
        }
    }

    private void TakeDamage()
    {
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            StartCoroutine(IndicateDamage());
        }
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
}