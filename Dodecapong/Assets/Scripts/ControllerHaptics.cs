using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Controller Haptics")]
public class ControllerHaptics : ScriptableObject
{
    [Range(0f, 1f)] public float lowFrequencyIntensity;
    [Range(0f, 1f)] public float highFrequencyIntensity;
    public float duration = 0.5f;
}
