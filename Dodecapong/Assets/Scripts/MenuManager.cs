using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using static GameManager;
using UnityEngine.EventSystems;

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
    public EventSystem eventSystem;

    [Header("Screens")]
    public GameObject mainMenu;
    public GameObject startScreen;
    public GameObject settingsScreen;
    public GameObject gameScreen;
    public GameObject pauseScreen;
    public GameObject endScreen;

    [Header("Settings Screens")]
    public GameObject fieldSettings;
    public GameObject playerSettings;
    public GameObject scoreSettings;
    public GameObject ballSettings;

    [Header("Default Buttons")]
    public GameObject mainMenuDefault;
    public GameObject startDefault;
    public GameObject settingsDefault;
    public GameObject pauseDefault;
    public GameObject endDefault;

    [Space]
    public List<MenuTextPair> menuTextPairs;

    // Start is called before the first frame update
    void Start()
    {
        DisableAll();
        MainMenu();
        instance.gameStateChanged.AddListener(OnGameStateChanged);
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

        ballSettings.SetActive(false);
        fieldSettings.SetActive(false);
        playerSettings.SetActive(false);
        scoreSettings.SetActive(false);
    }

    public void UpdateState(StateToChangeTo stateToChangeTo)
    {
        instance.UpdateGameState(stateToChangeTo.state);
    }

    void OnGameStateChanged()
    {
        switch (instance.gameState)
        {
            case GameState.MAINMENU:
                MainMenu();
                break;
            case GameState.JOINMENU:
                StartScreen();
                break;
            case GameState.SETTINGSMENU:
                SettingsScreen();
                break;
            case GameState.GAMEPLAY:
                GameScreen();
                break;
            case GameState.GAMEPAUSED:
                PauseMenu();
                break;
            case GameState.GAMEOVER:
                EndGame();
                break;
        }
    }

    // Buttons at top, enabling/disabling panels
    public void BallSettingsEnable()
    {
        DisableAll();
        settingsScreen.SetActive(true);
        ballSettings.SetActive(true);
    }

    public void FieldSettingsEnable()
    {
        DisableAll();
        settingsScreen.SetActive(true);
        fieldSettings.SetActive(true);
    }

    public void PlayerSettingsEnable()
    {
        DisableAll();
        settingsScreen.SetActive(true);
        playerSettings.SetActive(true);
    }

    public void ScoreSettingsEnable()
    {
        DisableAll();
        settingsScreen.SetActive(true);
        scoreSettings.SetActive(true);
    }

    // buttons ohohoho

    public void StartScreen()
    {
        DisableAll();
        startScreen.SetActive(true);
        eventSystem.SetSelectedGameObject(startDefault);
    }

    public void SettingsScreen()
    {
        DisableAll();
        settingsScreen.SetActive(true);
        ballSettings.SetActive(true);
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
}
