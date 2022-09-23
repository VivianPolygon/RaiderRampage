using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIMovement : MonoBehaviour
{
    private NavMeshAgent navmeshAgent;
    [SerializeField]
    private Transform startingDestination;

    // Start is called before the first frame update
    void Awake()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        MoveToObject();
    }

    private void MoveToObject()
    {
        navmeshAgent.destination = startingDestination.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out AiPathNode node))
        {
            navmeshAgent.destination = node.DecideNextNode().position;
        }
    }
}
