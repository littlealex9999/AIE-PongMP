using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISliderPassthrough : MonoBehaviour
{
    Slider slider;
    public SliderVariables variable;
    public float[] values;

    public enum SliderVariables
    {
        BALLSPEED,
        BALLSIZE,
        BALLCOUNT,
        PLAYERSPEED,
        TIMER,
        SHIELDHEALTH,
        TFFREQUENCY,
        TFPOWER,
    }

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
            slider.value = i + 1;
            break;
        }
    }

    public void ApplyValue(int index)
    {
        if (MenuManager.instance.settingUIVars) return;

        switch (variable)
        {
            case SliderVariables.BALLSPEED:
                GameVariables.SetBallSpeed(values[index - 1]);
                break;
            case SliderVariables.BALLSIZE:
                GameVariables.SetBallSize(values[index - 1]);
                break;
            case SliderVariables.BALLCOUNT:
                GameVariables.SetBallCount(values[index - 1]);
                break;
            case SliderVariables.PLAYERSPEED:
                GameVariables.SetPlayerSpeed(values[index - 1]);
                break;
            case SliderVariables.TIMER:
                GameVariables.SetTimerSeconds(values[index - 1]);
                break;
            case SliderVariables.SHIELDHEALTH:
                GameVariables.SetShieldLives(values[index - 1]);
                break;
            case SliderVariables.TFFREQUENCY:
                GameVariables.SetTransformerFrequency(values[index - 1]);
                break;
            case SliderVariables.TFPOWER:
                GameVariables.SetTransformerPower(values[index - 1]);
                break;
            
        }
    }

    public void ApplyValue(float index)
    {
        ApplyValue((int)index);
    }
}