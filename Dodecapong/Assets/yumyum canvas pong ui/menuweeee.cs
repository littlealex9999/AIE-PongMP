using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using static GameManager;

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

public class menuweeee : MonoBehaviour
{
    public GameObject fieldSettings;
    public GameObject playerSettings;
    public GameObject scoreSettings;
    public GameObject ballSettings;


    public GameObject settingsScreen;
    public GameObject startScreen;
    public GameObject mainMenu;
    public GameObject gameScreen;
    public GameObject endScreen;
    public GameObject pauseMenu;

    public List<MenuTextPair> menuTextPairs;

    // Start is called before the first frame update
    void Awake()
    {
        DisableAll();
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
                PressStart();
                break;
            case GameState.SETTINGSMENU:
                CustomGame();
                break;
            case GameState.GAMEPLAY:
                PlayGame();
                break;
            case GameState.GAMEPAUSED:
                PauseMenu();
                break;
            case GameState.SCOREBOARD:
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

    public void PressStart()
    {
        DisableAll();
        startScreen.SetActive(true);
    }

    public void CustomGame()
    {
        DisableAll();
        settingsScreen.SetActive(true);
        ballSettings.SetActive(true);
    }

    public void PlayGame()
    {
        DisableAll();
        gameScreen.SetActive(true);
    }

    public void EndGame()
    {
        DisableAll();
        endScreen.SetActive(true);
    }

    public void MainMenu()
    {
        DisableAll();
        mainMenu.SetActive(true);
    }

    public void PauseMenu()
    {
        pauseMenu.SetActive(true);
        //time scale 0 or whatever 
    }
}
