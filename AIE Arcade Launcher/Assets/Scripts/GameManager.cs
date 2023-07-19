using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    List<AppHelper> runningApps = new List<AppHelper>();

    public TextMeshProUGUI text;
    public int targetGame = 0;
    public int displayedGame = -1;
    public float switchCooldown = 0.1f;
    float switchTimer;
    public Color normalColor = Color.white;
    public Color runningColor = Color.green;

    public TMP_InputField gameExecutableInputField;
    public TMP_InputField gameTitleInputField;
    public TMP_InputField gameDescriptionInputField;

    DataManager dataManager;
    List<GameData> gameData = new List<GameData>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        dataManager = new DataManager(Application.persistentDataPath, Application.persistentDataPath + "/GameData");
        for (int i = 0; i < dataManager.gameTitles.Count; ++i) {
            gameData.Add(dataManager.ReadGameData(dataManager.gameTitles[i]));

            // There was no data at the location, so remove it from the list and continue
            if (gameData[i] == null) {
                gameData.RemoveAt(i);
                dataManager.gameTitles.RemoveAt(i);
                --i;
            }
        }

        if (gameData.Count == 0) {
            GameData data = new GameData(Application.persistentDataPath + "/" + "RunUntilInput.exe", "Test", "This is a test app");
            AddExistingGameData(data);
        }
    }

    private void OnApplicationQuit()
    {
        dataManager.WriteTitles(gameData);
    }

    private void Update()
    {
        for (int i = 0; i < runningApps.Count; ++i) {
            if (runningApps[i] != null) {
                if (runningApps[i].hasExited) {
                    runningApps.RemoveAt(i);
                    --i;

                    // in case the game being played was currently selected
                    UpdateSelectionText();
                }
            }
        }

        float verticalInput = Input.GetAxis("Vertical");

        switchTimer -= Time.deltaTime;
        if (switchTimer <= 0) {
            if (verticalInput > 0.5f) {
                ++targetGame;
                switchTimer = switchCooldown;
            } else if (verticalInput < -0.5f) {
                --targetGame;
                switchTimer = switchCooldown;
            }
        }

        if (targetGame != displayedGame) {
            if (targetGame < 0) targetGame = gameData.Count - 1;
            else if (targetGame >= gameData.Count) targetGame = 0;

            displayedGame = targetGame;
            UpdateSelectionText();
        }

        // start running a game
        if (Input.GetButtonDown("Select")) {
            AppHelper targetApp = GetAppIfRunning(gameData[displayedGame]);

            if (targetApp != null) {
                targetApp.KillApp();
            } else {
                runningApps.Add(new AppHelper(gameData[displayedGame]));
            }

            UpdateSelectionText();
        }
    }

    /// <summary>
    /// Creates new GameData based on input fields, then adds it to the list of games.
    /// </summary>
    public void CreateNewGameData()
    {
        GameData data = new GameData(gameExecutableInputField.text, gameTitleInputField.text, gameDescriptionInputField.text);
        AddExistingGameData(data);
    }

    /// <summary>
    /// Adds GameData to the list of games the launcher is tracking.
    /// </summary>
    /// <param name="data"></param>
    void AddExistingGameData(GameData data)
    {
        if (gameData.Contains(data)) return;
        for (int i = 0; i < gameData.Count; ++i) {
            if (gameData[i].gameTitle == data.gameTitle) {
#if UNITY_EDITOR
                Debug.LogWarning("There is already a game with the name: \"" + data.gameTitle + "\"");
#endif

                return;
            }
        }

        gameData.Add(data);
        dataManager.WriteGameData(data);
    }

    void UpdateSelectionText()
    {
        text.text = gameData[displayedGame].gameTitle;

        if (CheckAppRunning(gameData[displayedGame])) {
            text.color = runningColor;
        } else {
            text.color = normalColor;
        }
    }

    bool CheckAppRunning(GameData data)
    {
        for (int i = 0; i < runningApps.Count; ++i) {
            if (runningApps[i].getData == data) return true;
        }

        return false;
    }

    AppHelper GetAppIfRunning(GameData data)
    {
        for (int i = 0; i < runningApps.Count; ++i) {
            if (runningApps[i].getData == data) return runningApps[i];
        }

        return null;
    }
}
