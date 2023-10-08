using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISliderPassthrough : MonoBehaviour
{
    public Slider slider;
    public float[] values;
    public UnityEvent<float> UnityEvent;

    private void Awake()
    {
        if (!slider) slider = GetComponent<Slider>();
    }

    public void SetSliderToApproximate(float val)
    {
        for (int i = 0; i < values.Length; i++) {
            if (i > 0) {
                if (values[i - 1] >= val) {
                    continue;
                }
            }

            if (i < values.Length - 1) {
                if (values[i + 1] <= val) {
                    continue;
                }
            }

            if (!slider) slider = GetComponent<Slider>();
            slider.value = i;
            break;
        }
    }

    public void ApplyValue(int index)
    {
        if (!MenuManager.instance.settingUIVars) {
            UnityEvent.Invoke(values[index - 1]);
        }
    }

    public void ApplyValue(float index)
    {
        ApplyValue((int)index);
    }
}