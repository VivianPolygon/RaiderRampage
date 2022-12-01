using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//from the emptys that hold pants, chect armor, helmets and faces. randomly picke one and destroys the others
//controled on each enemy prefab by deleting one that shouldnn't be in the pool
public class ArmourPicker : MonoBehaviour
{
    [SerializeField] protected GameObject torsos;
    [SerializeField] protected GameObject pants;
    [SerializeField] protected GameObject helmets;
    [SerializeField] protected GameObject faces;

    private GameObject[] _armorSelectionPool;
    private int _selectionNum;

    //used for transfering over to ragdoll
    [HideInInspector] public int generatedTorsoValue;
    [HideInInspector] public int generatedPantsValue;
    [HideInInspector] public int generatedHelmetsValue;
    [HideInInspector] public int generatedFaceValue;

    //used for skin tone adjustments


    void Start()
    {
        if(torsos == null || pants == null || helmets == null || faces == null)
        {
            Debug.LogWarning("Armorpicker.cs on an enemy had a null refrence for one of the emptys that hod the possibiliies, please double check the prefabs. picker script deleted on runtime, object name: " + name);
            Destroy(this);
            return;
        }

        //grabs the values in the array and sets the selected item as the equipment
        generatedTorsoValue = SelectArmourPiece(torsos);
        generatedPantsValue = SelectArmourPiece(pants);
        generatedHelmetsValue = SelectArmourPiece(helmets);
        generatedFaceValue = SelectArmourPiece(faces);

    }

    //takes one of the emptys that holds the possible objects and selects one of their children to remain, deletes the others
    private int SelectArmourPiece(GameObject armorPoolObject)
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

        return _selectionNum;
    }
}
