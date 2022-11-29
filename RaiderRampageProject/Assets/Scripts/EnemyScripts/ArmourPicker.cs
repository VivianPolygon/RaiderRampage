using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//from the emptys that hold pants, chect armor, helmets and faces. randomly picke one and destroys the others
//controled on each enemy prefab by deleting one that shouldnn't be in the pool
public class ArmourPicker : MonoBehaviour
{
    [SerializeField] private GameObject torsos;
    [SerializeField] private GameObject pants;
    [SerializeField] private GameObject helmets;
    [SerializeField] private GameObject faces;

    private GameObject[] _armorSelectionPool;
    private int _selectionNum;

    //used for skin tone adjustments


    void Start()
    {
        if(torsos == null || pants == null || helmets == null || faces == null)
        {
            Debug.LogWarning("Armorpicker.cs on an enemy had a null refrence for one of the emptys that hod the possibiliies, please double check the prefabs. picker script deleted on runtime, object name: " + name);
            Destroy(this);
            return;
        }

        SelectArmourPiece(torsos);
        SelectArmourPiece(pants);
        SelectArmourPiece(helmets);
        SelectArmourPiece(faces);
    }

    //takes one of the emptys that holds the possible objects and selects one of their children to remain, deletes the others
    private void SelectArmourPiece(GameObject armorPoolObject)
    {
        //creates the array
        _armorSelectionPool = new GameObject[armorPoolObject.transform.childCount];

        for (int i = 0; i < armorPoolObject.transform.childCount; i++)
        {
            _armorSelectionPool[i] = armorPoolObject.transform.GetChild(i).gameObject;
        }

        //selects the object
        _selectionNum = Random.Range(0, _armorSelectionPool.Length);

        //deletes every object besides the selected object
        for (int i = 0; i < _armorSelectionPool.Length; i++)
        {
            if (i == _selectionNum)
            {

            }
            else
            {
                Destroy(_armorSelectionPool[i]);
            }
        }
    }
}
