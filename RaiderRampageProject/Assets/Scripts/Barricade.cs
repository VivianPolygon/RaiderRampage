using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour
{
    public static Barricade instance;

    private int barricadeMaxHealth = 100;
    [SerializeField]
    private int barricadeCurrentHealth;

    [SerializeField]
    private Mesh[] barricadeDamageModels;
    [SerializeField]
    private Material[] barricadeDamageMaterials;

    private MeshFilter mesh;
    private MeshRenderer meshRender;

    private int modelSwapIncrement;

    public int repairCost;
    public int repairAmount;

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

        BarricadeTakeDamage(0);
    }

    public void BarricadeTakeDamage(int damage)
    {
        barricadeCurrentHealth = Mathf.Clamp(barricadeCurrentHealth - damage, 0, barricadeMaxHealth);

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
        if(other.TryGetComponent(out Animator anim))
        {
            anim.SetBool("isAttacking", true);
        }
    }
}
