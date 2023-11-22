using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UITogglePassthrough : MonoBehaviour
{
    Toggle toggle;
    
    public ToggleVariables variable;

    public enum ToggleVariables
    {
        PLAYERDASH,
        TFBALLSIZEUP,
        TFBALLSIZEDOWN,
        TFBALLSPEED,
        TFBLACKHOLE,
    }

    private void Awake()
    {
        if (!toggle) toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(ApplyValue);
    }

    public void ApplyValue(bool value)
    {
        switch (variable)
        {
            case ToggleVariables.PLAYERDASH:
                GameVariables.SetPlayerDashEnabled(value);
                break;
            case ToggleVariables.TFBALLSIZEUP:
                GameVariables.SetBallSizeUpTransformer(value);
                break;
            case ToggleVariables.TFBALLSIZEDOWN:
                GameVariables.SetBallSizeDownTransformer(value);
                break;
            case ToggleVariables.TFBALLSPEED:
                GameVariables.SetBallSpeedTransformer(value);
                break;
            case ToggleVariables.TFBLACKHOLE:
                GameVariables.SetBlackHoleTransformer(value);
                break;
        }
    }
}
