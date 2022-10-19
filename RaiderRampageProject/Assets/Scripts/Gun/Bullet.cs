using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void Start()
    {
        damage = PlayerResourcesManager.instance.MultiplyDamage(damage);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3) //LevelTerrain layer
        {
            Destroy(this.gameObject);
        }
    }
}
