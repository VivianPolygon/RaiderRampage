using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIMovement : MonoBehaviour
{
    private NavMeshAgent navmeshAgent;
    [SerializeField]
    private Transform startingDestination;

    [HideInInspector]
    public Transform lastNode;
    [HideInInspector]
    public Transform nextNode;

    private Transform spawnNode;

    [Header("Used to fix enemies getting stuck, in seconds")]
    [SerializeField]
    private int velocityResetTimeThreshold = 10;
    private int moveToLastNodeTimeThreshold = 25;


    [Header("Does this enemy take cover?")]
    public bool takesCover = true;

    //used for unstucking
    [HideInInspector]
    public float timeSenseNewNode;
    private Coroutine trackTime;

    //used for taking cover
    private Coroutine takeCover;
    private bool inCover;


    // Start is called before the first frame update
    void Awake()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
        inCover = false;
    }


    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.TryGetComponent(out AiPathNode node))
        {
            if(spawnNode == null)
            {
                spawnNode = node.transform;
            }

            if(nextNode != null)
            {
                lastNode = nextNode;
            }

            nextNode = node.DecideNextNode();

            if(lastNode == null)
            {
                lastNode = spawnNode;
            }

            switch (node.mode)
            {
                case AiPathNode.NodeMode.NextNode:
                    navmeshAgent.destination = nextNode.position;
                    break;
                case AiPathNode.NodeMode.TakeCover:
                    //checks and starts the taking cover coroutine
                    if(!inCover && takesCover)
                    {
                        if (takeCover != null)
                        {
                            takeCover = null;
                        }
                        takeCover = StartCoroutine(TakeCover(node));
                    }
                    //if the enemy dosent take cover, simply moves them on to the next node
                    else if(!takesCover)
                    {
                        navmeshAgent.destination = nextNode.position;
                    }
                    break;
                default:
                    break;
            }

            SetTimer();
        }


        //Temporary, for testing purposes, destroys enemy when they contact somthing tagged ScrapBin <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        if(other.gameObject.tag == "ScrapBin")
        {
            Destroy(this.gameObject);
        }
    }

    private void SetTimer()
    {
        if(trackTime != null)
        {
            StopCoroutine(trackTime);
        }

        timeSenseNewNode = 0;
        trackTime = StartCoroutine(TrackTime());
    }

    private IEnumerator TrackTime()
    {
        int velocityResetCount = 1;
        bool nodeReset = false;

        while (true)
        {
            timeSenseNewNode += Time.deltaTime;

            //resets velocity every 10 seconds sense node started
            if(timeSenseNewNode > velocityResetTimeThreshold * velocityResetCount)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;

                velocityResetCount++;
            }
            if(timeSenseNewNode > moveToLastNodeTimeThreshold && !nodeReset)
            {
                navmeshAgent.destination = lastNode.position;
                nodeReset = true;
            }


            yield return null;
        }
    }

    private IEnumerator TakeCover(AiPathNode node)
    {
        float speed = navmeshAgent.speed;
        inCover = true;

        transform.localScale = transform.localScale - (Vector3.up);
        navmeshAgent.speed = 0;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        for (float i = 0; i < Random.Range(node.minCoverTime, node.maxCoverTime); i += Time.deltaTime)
        {

            timeSenseNewNode = 0;
            yield return null;
        }

        navmeshAgent.speed = speed;

        transform.localScale = transform.localScale + (Vector3.up);

        navmeshAgent.destination = nextNode.position;

        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        //prevents cover being immediatly reentered
        for (float i = 0; i < 2; i += Time.deltaTime)
        {

            yield return null;
        }

        inCover = false;
    }
}
