using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class Barricade : MonoBehaviour
{
    public static Barricade instance;

    public int barricadeMaxHealth = 100;
    [HideInInspector]
    public int barricadeCurrentHealth;

    [SerializeField]
    private Mesh[] barricadeDamageModels;
    [SerializeField]
    private Material[] barricadeDamageMaterials;

    private MeshFilter mesh;
    private MeshRenderer meshRender;

    private int modelSwapIncrement;

    public int repairCost;
    public int repairAmount;

    private static event Action onDamageBarricade;

    public static void DamageBarricade() { onDamageBarricade?.Invoke(); }

    private void OnEnable()
    {
        onDamageBarricade += BarricadeTakeDamage;
    }

    private void OnDisable()
    {
        onDamageBarricade -= BarricadeTakeDamage;
    }


    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("two or more barricades found in Scene, make sure there is only one");
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        barricadeCurrentHealth = barricadeMaxHealth;
        if(TryGetComponent(out MeshFilter meshFilter))
        {
            mesh = meshFilter;
        }
        else
        {
            Debug.LogWarning("barricade does not have a mesh filter component, make sure it has one, barricade deleted on runtime. Object name: " + gameObject.name);
            Destroy(this.gameObject);
        }

        if (TryGetComponent(out MeshRenderer meshRend))
        {
            meshRender = meshRend;
        }
        else
        {
            Debug.LogWarning("barricade does not have a mesh Renderer component, make sure it has one, barricade deleted on runtime. Object name: " + gameObject.name);
            Destroy(this.gameObject);
        }

        modelSwapIncrement = barricadeMaxHealth / (barricadeDamageModels.Length - 1);

        barricadeCurrentHealth++;
        BarricadeTakeDamage();
    }

    public void BarricadeTakeDamage()
    {
        barricadeCurrentHealth = Mathf.Clamp(barricadeCurrentHealth - 1, 0, barricadeMaxHealth);

        //temporary reload to title menu if barricade is knocked down <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        if(barricadeCurrentHealth <= 0)
        {
            Debug.Log("TitleMenu Returned to Due to Barricade Being Broken");
            SceneManager.LoadScene(0);
        }

        int healthCalc = barricadeMaxHealth;
        for (int i = 0; i < barricadeDamageModels.Length; i++)
        {
            if (healthCalc >= barricadeCurrentHealth && mesh.mesh != barricadeDamageModels[i])
            {
                mesh.mesh = barricadeDamageModels[i];
                meshRender.material = barricadeDamageMaterials[i];
            }

            healthCalc -= modelSwapIncrement;

        }
    }

    //caculates how to charge for repairs that are smaller than the full increment
    public int CaculateRepairCost()
    {
        if(barricadeMaxHealth - barricadeCurrentHealth < repairAmount)
        {
            return Mathf.RoundToInt(((float)((float)(barricadeMaxHealth - barricadeCurrentHealth) / repairAmount)) * repairCost);
        }
        return repairCost;
    }

    public int CaculateRepairScrap()
    {
        if(barricadeMaxHealth - barricadeCurrentHealth < repairAmount)
        {
            return barricadeMaxHealth - barricadeCurrentHealth;
        }

        return repairAmount;
    }
    //temporary way to trigger animat8ons
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out EnemyAnimatorController anim))
        {
            anim.SetAttacking(true);

            other.GetComponent<NavMeshAgent>().destination = transform.position;
            other.GetComponent<NavMeshAgent>().speed = 0;

            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            other.GetComponent<Rigidbody>().Sleep();
        }
    }
}
