using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public int damage;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3) //LevelTerrain layer
        {
            Destroy(this.gameObject);
        }
    }
}
