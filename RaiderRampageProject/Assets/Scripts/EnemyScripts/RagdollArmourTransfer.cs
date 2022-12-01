using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//from the emptys that hold pants, chect armor, helmets and faces. randomly picke one and destroys the others
//controled on each enemy prefab by deleting one that shouldnn't be in the pool
public class RagdollArmourTransfer : MonoBehaviour
{
    [SerializeField] private GameObject torsos;
    [SerializeField] private GameObject pants;
    [SerializeField] private GameObject helmets;
    [SerializeField] private GameObject faces;

    [SerializeField] private SkinnedMeshRenderer meshRender;

    [Header("defaulted too selections")]
    [SerializeField] private Color lightColor;
    [SerializeField] private Color darkColor;

    public void SetRagdollArmours(int torsoNum, int pantsNum, int helmetNum, int faceNum)
    {
        SetArmourPiece(torsos, torsoNum);
        SetArmourPiece(pants, pantsNum);
        SetArmourPiece(helmets, helmetNum);
        SetArmourPiece(faces, faceNum);
    }

    public void setRagdollSkintone(Color skinTone)
    {
        meshRender.material.color = skinTone;
    }

    private void SetArmourPiece(GameObject parent, int num)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if(i == num)
            {
                parent.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                parent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    //used to set the armour and skin tone if being spawned in victory screen
    public void RandomSelectArmourAndSkinTone()
    {
        SetArmourPiece(torsos, Random.Range(0, torsos.transform.childCount));
        SetArmourPiece(pants, Random.Range(0, pants.transform.childCount));
        SetArmourPiece(helmets, Random.Range(0, helmets.transform.childCount));
        SetArmourPiece(faces, Random.Range(0, faces.transform.childCount));

        meshRender.material.color = Color.Lerp(lightColor, darkColor, Random.Range(0f, 1f));
    }
}
