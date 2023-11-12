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
        TFBALLSIZE,
        TFBALLSPEED,
        TFBLACKHOLE,
        TFDASHCD,
        TFPLAYERSPEED,
        TFSHEILDHEALTH,
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
            case ToggleVariables.TFBALLSIZE:
                GameVariables.SetBallSizeTransformer(value);
                break;
            case ToggleVariables.TFBALLSPEED:
                GameVariables.SetBallSpeedTransformer(value);
                break;
            case ToggleVariables.TFBLACKHOLE:
                GameVariables.SetBlackHoleTransformer(value);
                break;
            case ToggleVariables.TFDASHCD:
                GameVariables.SetDashCooldownTransformer(value);
                break;
            case ToggleVariables.TFPLAYERSPEED:
                GameVariables.SetPlayerSpeedTransformer(value);
                break;
            case ToggleVariables.TFSHEILDHEALTH:
                GameVariables.SetShieldLivesTransformer(value);
                break;
        }
    }
}
