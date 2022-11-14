using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignRandomOrient : MonoBehaviour
{
    void Start()
    {
        if(Random.Range(0, 2) == 0)
        {
            transform.Rotate(0, 180, 0);
        }
    }

}
