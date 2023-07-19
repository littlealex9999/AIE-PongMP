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
                    text.color = normalColor;
                }
            }
        }

        // start running a game
        if (Input.GetButtonDown("Select")) {
            if (runningApps.Count > 0) {
                runningApps[0].KillApp();
            } else {
                runningApps.Add(new AppHelper(gameData[0].exePath));
                text.color = runningColor;
            }
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
}
