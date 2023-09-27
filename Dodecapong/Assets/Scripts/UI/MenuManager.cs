using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Events;

[Serializable]
public class MenuTextPair
{
    public Slider sliderWidget;
    public TMP_Text labelWidget;
    public List<string> customValues = new List<string>();

    public void UpdateText()
    {
        if (sliderWidget != null && labelWidget != null)
        {
            if (customValues == null || customValues.Count == 0)
            {
                labelWidget.text = sliderWidget.value.ToString();

            }
            else
            {
                labelWidget.text = customValues[(int)sliderWidget.value];
            }

        }
    }
}

public class MenuManager : MonoBehaviour
{
    #region Instance setup
    public static MenuManager instance;

    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);
    }
    #endregion

    public EventSystem eventSystem;

    [Header("Screens")]
    public GameObject mainMenu;
    public GameObject startScreen;
    public GameObject settingsScreen;
    public GameObject gameScreen;
    public GameObject pauseScreen;
    public GameObject endScreen;

    [Header("Settings Screens")]
    public GameObject[] settingsSubScreens;
    int settingsCurrentActive = 0;

    [Header("Default Buttons")]
    public GameObject mainMenuDefault;
    public GameObject startDefault;
    public GameObject settingsDefault;
    public GameObject pauseDefault;
    public GameObject endDefault;

    [Space]
    public List<MenuTextPair> menuTextPairs;

    // having each one be given its own variable and looked after seems to be the best way to set all values 
    [Header("Settings UI")]
    public UISliderPassthrough ballSpeedSlider;
    public UISliderPassthrough ballSizeSlider;
    public UISliderPassthrough ballCountSlider;
    public Toggle increasingSpeedToggle;
    public Toggle increasingSizeToggle;
    [Space]
    public UISliderPassthrough playerSpeedSlider;
    public UISliderPassthrough playerSizeSlider;
    public Toggle playerDashToggle;
    [Space]
    public UISliderPassthrough timerSlider;
    public UISliderPassthrough shieldHealthSlider;
    [Space]
    public UISliderPassthrough transformerFrequencySlider;
    public UISliderPassthrough transformerPowerSlider;
    public Toggle transformerBallSizeToggle;
    public Toggle transformerBallSpeedToggle;
    public Toggle transformerBlackHoleToggle;
    public Toggle transformerDashCooldownToggle;
    public Toggle transformerPlayerSpeedToggle;
    public Toggle transformerShieldHealthToggle;
    public bool settingUIVars { get; private set; } = true;

    // Start is called before the first frame update
    void Start()
    {
        DisableAll();
        GameManager.instance.OnGameStateChange += OnGameStateChanged;
        UpdateState(GameManager.GameState.MAINMENU);
    }

    public void Update()
    {
        foreach(MenuTextPair thisPair in menuTextPairs)
        {
            thisPair.UpdateText();
        }
    }

    public void DisableAll()
    {
        settingsScreen.SetActive(false);
        startScreen.SetActive(false);
        mainMenu.SetActive(false);
        gameScreen.SetActive(false);
        pauseScreen.SetActive(false);
        endScreen.SetActive(false);
    }

    public void UpdateState(StateToChangeTo stateToChangeTo)
    {
        UpdateState(stateToChangeTo.state);
    }

    public void UpdateState(GameManager.GameState state)
    {
        GameManager.instance.UpdateGameState(state);
    }

    void OnGameStateChanged()
    {
        switch (GameManager.instance.gameState)
        {
            case GameManager.GameState.MAINMENU:
                MainMenu();
                break;
            case GameManager.GameState.JOINMENU:
                StartScreen();
                break;
            case GameManager.GameState.SETTINGSMENU:
                SettingsScreen();
                break;
            case GameManager.GameState.GAMEPLAY:
                GameScreen();
                break;
            case GameManager.GameState.GAMEPAUSED:
                PauseMenu();
                break;
            case GameManager.GameState.GAMEOVER:
                EndGame();
                break;
        }
    }

    /// <summary>
    /// Changes the subscreen on the settings menu. Only takes effect when the settings screen is active.
    /// </summary>
    /// <param name="index"></param>
    void SettingsScreenCycle(int index)
    {
        if (index >= settingsSubScreens.Length) index = 0;
        if (index < 0) index = settingsSubScreens.Length - 1;

        settingsSubScreens[settingsCurrentActive].SetActive(false);
        settingsSubScreens[index].SetActive(true);
        settingsCurrentActive = index;
        eventSystem.SetSelectedGameObject(settingsDefault);
    }

    public void SettingsScreenPageRight() => SettingsScreenCycle(settingsCurrentActive + 1);
    public void SettingsScreenPageLeft() => SettingsScreenCycle(settingsCurrentActive - 1);

    void SettingsScreenResetVariableUI()
    {
        settingUIVars = true;

        ballSpeedSlider.SetSliderToApproximate(GameManager.instance.gameVariables.ballSpeed);
        ballSizeSlider.SetSliderToApproximate(GameManager.instance.gameVariables.ballSize);
        ballCountSlider.SetSliderToApproximate(GameManager.instance.gameVariables.ballCount);
        //increasingSpeedToggle.isOn = GameManager.instance.
        //increasingSizeToggle.isOn = GameManager.instance

        playerSpeedSlider.SetSliderToApproximate(GameManager.instance.gameVariables.playerSpeed);
        //playerSizeSlider.SetSliderToApproximate(GameManager.instance.gameVariables.playerSizes);
        playerDashToggle.isOn = GameManager.instance.gameVariables.dashEnabled;

        timerSlider.SetSliderToApproximate(GameManager.instance.gameVariables.timeInSeconds);
        shieldHealthSlider.SetSliderToApproximate(GameManager.instance.gameVariables.shieldLives);

        transformerFrequencySlider.SetSliderToApproximate(GameManager.instance.gameVariables.transformerFrequency);
        transformerPowerSlider.SetSliderToApproximate(GameManager.instance.gameVariables.transformerPower);
        transformerBallSizeToggle.isOn = (int)(GameManager.instance.gameVariables.enabledTransformers & Transformer.TransformerTypes.BALLSIZE) > 0;
        transformerBallSpeedToggle.isOn = (int)(GameManager.instance.gameVariables.enabledTransformers & Transformer.TransformerTypes.BALLSPEED) > 0;
        transformerBlackHoleToggle.isOn = (int)(GameManager.instance.gameVariables.enabledTransformers & Transformer.TransformerTypes.BLACKHOLE) > 0;
        transformerDashCooldownToggle.isOn = (int)(GameManager.instance.gameVariables.enabledTransformers & Transformer.TransformerTypes.DASHCOOLDOWN) > 0;
        transformerPlayerSpeedToggle.isOn = (int)(GameManager.instance.gameVariables.enabledTransformers & Transformer.TransformerTypes.PLAYERSPEED) > 0;
        transformerShieldHealthToggle.isOn = (int)(GameManager.instance.gameVariables.enabledTransformers & Transformer.TransformerTypes.SHIELDHEALTH) > 0;

        settingUIVars = false;
    }

    public void StartScreen()
    {
        DisableAll();
        startScreen.SetActive(true);
        eventSystem.SetSelectedGameObject(startDefault);
    }

    public void SettingsScreen()
    {
        SettingsScreenResetVariableUI();

        DisableAll();
        settingsScreen.SetActive(true);
        SettingsScreenCycle(0);
        eventSystem.SetSelectedGameObject(settingsDefault);
    }

    public void GameScreen()
    {
        DisableAll();
        gameScreen.SetActive(true);
    }

    public void EndGame()
    {
        DisableAll();
        endScreen.SetActive(true);
        eventSystem.SetSelectedGameObject(endDefault);
    }

    public void MainMenu()
    {
        DisableAll();
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(mainMenuDefault);
    }

    public void PauseMenu()
    {
        DisableAll();
        pauseScreen.SetActive(true);
        eventSystem.SetSelectedGameObject(pauseDefault);
        //time scale 0 or whatever 
    }

    public void SelectButton()
    {
        EventManager.instance?.hoverUIEvent?.Invoke();
    }
    public void SubmitButton()
    {
        EventManager.instance?.selectUIEvent?.Invoke();
    }
}
