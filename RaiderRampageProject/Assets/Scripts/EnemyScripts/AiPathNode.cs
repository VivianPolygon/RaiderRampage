using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPathNode : MonoBehaviour
{
    [SerializeField]
    private Transform[] nextNodes;
    [SerializeField]
    private int[] nodeWeights;

    [SerializeField]
    private int defaultWeight;


    private int[] weightsStorage;

    private int weightTotal;
    private int generatedPathValue;
    private int weightsCheck;

    private void Awake()
    {
        InitilizeWeights();
        HideNodeModel();
    }
    
    private void InitilizeWeights()
    {
        weightTotal = 0;

        if (nodeWeights.Length != nextNodes.Length)
        {
            if (nodeWeights.Length < nextNodes.Length)
            {
                Debug.LogWarning("node: " + name + "has a less node weights than nodes, make sure they are equal. weights created for extra nodes, and defaulted to " + defaultWeight.ToString());
                weightsStorage = nodeWeights;
                nodeWeights = new int[nextNodes.Length];

                for (int i = 0; i < nodeWeights.Length; i++)
                {
                    if (i < weightsStorage.Length - 1)
                    {
                        nodeWeights[i] = weightsStorage[i];
                    }
                    else
                    {
                        nodeWeights[i] = defaultWeight;
                    }

                }

                weightsStorage = null;
            }
            else
            {
                Debug.LogWarning("there are extra unneded node weights, make sure there is as many weights as their is next nodes, fixed on runtime");
                weightsStorage = nodeWeights;
                nodeWeights = new int[nextNodes.Length];
                for (int i = 0; i < nextNodes.Length; i++)
                {
                    nodeWeights[i] = weightsStorage[i];
                }

                weightsStorage = null;
            }
        }

        SetWeights();
    }

    public void SetWeights()
    {
        for (int i = 0; i < nodeWeights.Length; i++)
        {
            if (nodeWeights[i] < 1)
            {

                nodeWeights[i] = defaultWeight;

            }

            weightTotal += nodeWeights[i];
        }
    }

    public Transform DecideNextNode()
    {
        weightsCheck = 0;
        generatedPathValue = Random.Range(0, weightTotal + 1);

        for (int i = 0; i < nextNodes.Length; i++)
        {
            weightsCheck += nodeWeights[i];

            if(weightsCheck >= generatedPathValue)
            {
                return nextNodes[i];
            }
        }
        Debug.LogWarning("Path could not be decided by the node properly, defaulted to first node in array");
        return nextNodes[0];
    }

    private void HideNodeModel()
    {
        if (TryGetComponent(out MeshRenderer meshRenderer))
        {
            meshRenderer.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        weightTotal = 0;

        for (int i = 0; i < nextNodes.Length; i++)
        {
            weightTotal += nodeWeights[i];
        }
        for (int i = 0; i < nextNodes.Length; i++)
        {
            Gizmos.color = Color.Lerp(Color.black, Color.green, nodeWeights[i] / (float)weightTotal);
            Gizmos.DrawLine(transform.position, nextNodes[i].position);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.Normalize(nextNodes[i].position - transform.position) * 5);
        }
    }
}
