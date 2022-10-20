using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OverdriveTierScriptableObject", menuName = "OverdriveTier")]
public class OverdriveTierObject : ScriptableObject
{
    public OverdriveMode mode;

    public string overDriveButtonText;

    public int nextTierMurderScoreThreshold;

    public float gaugeDrainRate;

    public Color barColor;


    public enum OverdriveMode
    {
        None,
        ArmorPierce,
        Incendiary,
        Explosive,
        Flower
    }


}
