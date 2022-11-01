using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    //needs to control isInjured
    //needs to control if isAttacking
    //needs to generate value between 0-3 to control what attack
    //needs to control if dead

    public Animator enemyAnim;

    [SerializeField] private float attackGenerateRate = 1;


    private void Start()
    {
        if(enemyAnim == null)
        {
            Debug.LogWarning("enemy Anim located on an instance of EnemyAnimatorController.cs on: " + name + "is null, Gameobject deleted on runtime to prevent errors");
            Destroy(this.gameObject);
        }
    }

    public void SetInjured(bool state)
    {
        enemyAnim.SetBool("isInjured", state);
    }

    public void SetAttacking(bool state)
    {
        enemyAnim.SetBool("isAttacking", state);
        StartCoroutine(SetAttackNumber());
    }

    public void SetDeath(bool state)
    {
        enemyAnim.SetBool("isDead", state);
    }

    public void SetDucking(bool state)
    {
        enemyAnim.SetBool("isDucking", state);
    }


    private IEnumerator SetAttackNumber()
    {
        for (; ; )
        {
            for (float i = 0; i < attackGenerateRate; i += Time.deltaTime)
            {


                yield return null;
            }

            enemyAnim.SetInteger("AttackNum", Random.Range(0, 4));

            yield return null;
        }

    }
}
