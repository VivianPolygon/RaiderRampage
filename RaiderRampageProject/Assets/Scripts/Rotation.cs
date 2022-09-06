using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//quick rotation script to simulate the head of the gun spinning
public class Rotation : MonoBehaviour
{

    [SerializeField]
    private Vector3 rotation;

    private void Update()
    {
        transform.Rotate(rotation * Time.deltaTime);
    }
}
