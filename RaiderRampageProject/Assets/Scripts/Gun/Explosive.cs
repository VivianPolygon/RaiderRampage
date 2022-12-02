using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//runs on destroy, applys explosive damage and force to enemys within a range
//put this script on a bullet you want to explode, and it will explode apon being destroyed
public class Explosive : MonoBehaviour
{
    public int explosionDamage = 10;

    public int explosionForce = 5;
    public int explosionRange = 5;

    public GameObject explosionVisual;
    public float visualTime;

    private void Start()
    {
        if(PlayerResourcesManager.instance != null)
        {
            explosionDamage = PlayerResourcesManager.instance.MultiplyDamage(explosionDamage);
        }
    }

    private void OnDestroy()
    {
        if(gameObject.scene.isLoaded)
        {
            foreach (Collider collider in Physics.OverlapSphere(transform.position, explosionRange))
            {
                if (collider.TryGetComponent(out Rigidbody rigidBody))
                {
                    rigidBody.AddExplosionForce(explosionForce, transform.position, explosionDamage);
                }

                if (collider.TryGetComponent(out EnemyStats enemyStats))
                {
                    enemyStats.TakeDamage(explosionDamage, enemyStats.transform.position, true);
                }

            }

            GameObject visual = Instantiate(explosionVisual, transform.position, Quaternion.identity);
            Destroy(visual, visualTime);
        }

    }

}
