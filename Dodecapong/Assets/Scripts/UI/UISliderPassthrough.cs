using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISliderPassthrough : MonoBehaviour
{
    public float[] values;
    public UnityEvent<float> UnityEvent;

    public void ApplyValue(int index)
    {
        UnityEvent.Invoke(values[index - 1]);
    }

    public void ApplyValue(float index)
    {
        UnityEvent.Invoke(values[(int)index - 1]);
    }
}