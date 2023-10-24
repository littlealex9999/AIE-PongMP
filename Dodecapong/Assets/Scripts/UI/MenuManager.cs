using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Xml.Serialization;
using UnityEngine.InputSystem;

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
    public PlayerInputManager playerInputManager;

    [Header("Screens")]
    public GameObject mainMenu;
    public GameObject startScreen;
    public GameObject presetSelectScreen;
    public GameObject editPresetScreen;
    public GameObject gameScreen;
    public GameObject pauseScreen;
    public GameObject endScreen;

    public GameObject[] presetSubScreens;
    int presetCurrentActive = 0;

    public GameObject[] editPresetSubScreens;
    int editPresetCurrentActive = 0;

    [Header("Default Buttons")]
    public GameObject mainMenuDefault;
    public GameObject startDefault;
    public GameObject presetSelectDefault;
    public GameObject editPresetDefault;
    public GameObject pauseDefault;
    public GameObject endDefault;

    [Space]
    public List<MenuTextPair> menuTextPairs;

    // having each one be given its own variable and looked after seems to be the best way to set all values 
    [Header("Settings UI")]
    // ball
    public UISliderPassthrough ballSpeedSlider;
    public UISliderPassthrough ballSizeSlider;
    public UISliderPassthrough ballCountSlider;
    public Toggle increasingSpeedToggle;
    public Toggle increasingSizeToggle;
    [Space] // player
    public UISliderPassthrough playerSpeedSlider;
    public UISliderPassthrough playerSizeSlider;
    public Toggle playerDashToggle;
    [Space] // score
    public UISliderPassthrough timerSlider;
    public UISliderPassthrough shieldHealthSlider;
    [Space] // morph
    public UISliderPassthrough transformerFrequencySlider;
    public UISliderPassthrough transformerPowerSlider;
    public Toggle transformerBallSizeToggle;
    public Toggle transformerBallSpeedToggle;
    public Toggle transformerBlackHoleToggle;
    public Toggle transformerDashCooldownToggle;
    public Toggle transformerPlayerSpeedToggle;
    public Toggle transformerShieldHealthToggle;
    public bool settingUIVars { get; private set; } = true;

    public List<Selectable> applyButtonUps;
    public List<Selectable> applyButtonDowns;
    public Button applyButton;

    public GameObject startGameButton;
    public GameObject notEnoughPlayersButton;
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
        editPresetScreen.SetActive(false);
        presetSelectScreen.SetActive(false);
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
            case GameManager.GameState.PRESETSELECT:
                PresetSelectScreen();
                break;
            case GameManager.GameState.EDITPRESET:
                EditPresetScreen();
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
    void PresetSelectScreenCycle(int index)
    {
        if (index >= presetSubScreens.Length) index = 0;
        if (index < 0) index = presetSubScreens.Length - 1;

        foreach (GameObject screen in presetSubScreens) screen.SetActive(false);
        presetSubScreens[index].SetActive(true);
        presetCurrentActive = index;
        eventSystem.SetSelectedGameObject(presetSelectDefault);
    }

    void EditPresetSelectScreenCycle(int index)
    {
        if (index >= editPresetSubScreens.Length) index = 0;
        if (index < 0) index = editPresetSubScreens.Length - 1;

        foreach (GameObject screen in editPresetSubScreens) screen.SetActive(false);

        Navigation navigation = new()
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown = applyButtonDowns[index].GetComponent<Selectable>(),
            selectOnUp = applyButtonUps[index].GetComponent<Selectable>()
        };
        applyButton.navigation = navigation;

        editPresetSubScreens[index].SetActive(true);
        editPresetCurrentActive = index;
        eventSystem.SetSelectedGameObject(applyButton.gameObject);
    }

    public void PageRight()
    { 
        if (GameManager.instance.gameState == GameManager.GameState.PRESETSELECT)
        {
            PresetSelectScreenCycle(presetCurrentActive + 1);
            eventSystem.SetSelectedGameObject(presetSelectDefault);
            //GameManager.instance.selectedGameVariables = new(GameManager.instance.gameVariables[presetCurrentActive]);
            GameManager.instance.selectedGameVariables.Copy(GameManager.instance.gameVariables[presetCurrentActive]);
        }
        else if (GameManager.instance.gameState == GameManager.GameState.EDITPRESET)
        {
            EditPresetSelectScreenCycle(editPresetCurrentActive + 1);
            eventSystem.SetSelectedGameObject(editPresetDefault);
        }
    }

    public void PageLeft()
    {
        if (GameManager.instance.gameState == GameManager.GameState.PRESETSELECT)
        {
            PresetSelectScreenCycle(presetCurrentActive - 1);
            eventSystem.SetSelectedGameObject(presetSelectDefault);
            //GameManager.instance.selectedGameVariables = new(GameManager.instance.gameVariables[presetCurrentActive]);
            GameManager.instance.selectedGameVariables.Copy(GameManager.instance.gameVariables[presetCurrentActive]);
        }
        else if (GameManager.instance.gameState == GameManager.GameState.EDITPRESET)
        {
            EditPresetSelectScreenCycle(editPresetCurrentActive - 1);
            eventSystem.SetSelectedGameObject(editPresetDefault);
        }
    }

    public void CheckPlayerCount()
    {
        if (GameManager.instance.players.Count >= 2)
        {
            startGameButton.SetActive(true);
            notEnoughPlayersButton.SetActive(false);
            eventSystem.SetSelectedGameObject(startGameButton);
        }
        else
        {
            startGameButton.SetActive(false);
            notEnoughPlayersButton.SetActive(true);
            eventSystem.SetSelectedGameObject(notEnoughPlayersButton);
        }
    }

    public void StartScreen()
    {
        DisableAll();
        startScreen.SetActive(true);
        playerInputManager.EnableJoining();
        eventSystem.SetSelectedGameObject(startDefault);
    }

    public void PresetSelectScreen()
    {
        //GameManager.instance.selectedGameVariables = new(GameManager.instance.gameVariables[presetCurrentActive]);
        GameManager.instance.selectedGameVariables.Copy(GameManager.instance.gameVariables[presetCurrentActive]);
        DisableAll();
        presetSelectScreen.SetActive(true);
        PresetSelectScreenCycle(presetCurrentActive);
        eventSystem.SetSelectedGameObject(presetSelectDefault);
    }

    public void EditPresetScreen()
    {
        if (GameManager.instance.selectedGameVariables == null)
        {
            //GameManager.instance.selectedGameVariables = new(GameManager.instance.gameVariables[presetCurrentActive]);
            GameManager.instance.selectedGameVariables.Copy(GameManager.instance.gameVariables[presetCurrentActive]);
        }
        SettingsScreenUpdateVariableUI();

        editPresetCurrentActive = 0;
        DisableAll();
        editPresetScreen.SetActive(true);
        EditPresetSelectScreenCycle(editPresetCurrentActive);
        eventSystem.SetSelectedGameObject(editPresetDefault);
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
        playerInputManager.DisableJoining();
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

    public void Apply()
    {
        eventSystem.SetSelectedGameObject(editPresetDefault);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Application Quit");
#endif

        Application.Quit();
    }

    void SettingsScreenUpdateVariableUI()
    {
        settingUIVars = true;

        ballSpeedSlider.SetSliderToApproximate(GameManager.instance.selectedGameVariables.ballSpeed);
        ballSizeSlider.SetSliderToApproximate(GameManager.instance.selectedGameVariables.ballSize);
        ballCountSlider.SetSliderToApproximate(GameManager.instance.selectedGameVariables.ballCount);
        //increasingSpeedToggle.isOn = GameManager.instance.
        //increasingSizeToggle.isOn = GameManager.instance

        playerSpeedSlider.SetSliderToApproximate(GameManager.instance.selectedGameVariables.playerSpeed);
        //playerSizeSlider.SetSliderToApproximate(GameManager.instance.currentGameVariables.playerSizes);
        playerDashToggle.isOn = GameManager.instance.selectedGameVariables.dashEnabled;

        timerSlider.SetSliderToApproximate(GameManager.instance.selectedGameVariables.timeInSeconds);
        shieldHealthSlider.SetSliderToApproximate(GameManager.instance.selectedGameVariables.shieldLives);

        transformerFrequencySlider.SetSliderToApproximate(GameManager.instance.selectedGameVariables.transformerFrequency);
        transformerPowerSlider.SetSliderToApproximate(GameManager.instance.selectedGameVariables.transformerPower);
        transformerBallSizeToggle.isOn = (int)(GameManager.instance.selectedGameVariables.enabledTransformers & Transformer.TransformerTypes.BALLSIZE) > 0;
        transformerBallSpeedToggle.isOn = (int)(GameManager.instance.selectedGameVariables.enabledTransformers & Transformer.TransformerTypes.BALLSPEED) > 0;
        transformerBlackHoleToggle.isOn = (int)(GameManager.instance.selectedGameVariables.enabledTransformers & Transformer.TransformerTypes.BLACKHOLE) > 0;
        transformerDashCooldownToggle.isOn = (int)(GameManager.instance.selectedGameVariables.enabledTransformers & Transformer.TransformerTypes.DASHCOOLDOWN) > 0;
        transformerPlayerSpeedToggle.isOn = (int)(GameManager.instance.selectedGameVariables.enabledTransformers & Transformer.TransformerTypes.PLAYERSPEED) > 0;
        transformerShieldHealthToggle.isOn = (int)(GameManager.instance.selectedGameVariables.enabledTransformers & Transformer.TransformerTypes.SHIELDHEALTH) > 0;

        settingUIVars = false;
    }
}
