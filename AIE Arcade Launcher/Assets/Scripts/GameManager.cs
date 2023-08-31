using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    List<AppHelper> runningApps = new List<AppHelper>();

    public TextMeshProUGUI text;
    public GamesList gamesList;
    public List<GameObject> gamesListingObjects = new List<GameObject>();

    public int targetGame = 0;
    public int displayedGame = -1;
    public float switchCooldown = 0.1f;
    bool moving;

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

        if (gameData.Count > 0) {
            PickDisplayGame();
            UpdateAllSelectionText();
        }
    }

    private void OnApplicationQuit()
    {
        dataManager.WriteTitles(gameData);
    }

    private void Update()
    {
        if (gameData.Count <= 0) {
            UpdateAllSelectionTextCustom("Please Upload A Game");
        } else {
            GamesFoundUpdate();
        }
    }

    void GamesFoundUpdate()
    {
        for (int i = 0; i < runningApps.Count; ++i) {
            if (runningApps[i] != null) {
                if (runningApps[i].hasExited) {
                    runningApps.RemoveAt(i);
                    --i;

                    // in case the game being played was currently selected
                    UpdateAllSelectionText();
                }
            }
        }

        float verticalInput = Input.GetAxis("Vertical");

        if (verticalInput > 0.5f) {
            StartCoroutine(MoveBanners(1, switchCooldown));
        } else if (verticalInput < -0.5f) {
            StartCoroutine(MoveBanners(-1, switchCooldown));
        }

        // start running a game
        if (Input.GetButtonDown("Select")) {
            AppHelper targetApp = GetAppIfRunning(gameData[displayedGame]);

            if (targetApp != null) {
                targetApp.KillApp();
            } else {
                runningApps.Add(new AppHelper(gameData[displayedGame]));
            }

            UpdateAllSelectionText();
        }
    }

    /// <summary>
    /// Opens a choose file dialog and outputs the selected file path to the given input field
    /// </summary>
    /// <param name="inputField"></param>
    public void SelectFileDialog(TMP_InputField inputField)
    {
        string[] outs = FileManager.FileDialog();
        if (outs.Length > 0) inputField.text = outs[0];
    }

    /// <summary>
    /// Creates new GameData based on input fields, then adds it to the list of games.
    /// </summary>
    public void CreateNewGameData()
    {
        GameData data = new GameData(gameExecutableInputField.text, gameTitleInputField.text, gameDescriptionInputField.text);
        AddExistingGameData(data, true);
    }

    /// <summary>
    /// Adds GameData to the list of games the launcher is tracking.
    /// </summary>
    /// <param name="data"></param>
    void AddExistingGameData(GameData data, bool updateText = false)
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
        if (updateText) UpdateAllSelectionText();
    }

    void PickDisplayGame()
    {
        if (targetGame < 0) targetGame = gameData.Count - 1;
        else if (targetGame >= gameData.Count) targetGame = 0;
        displayedGame = targetGame;
    }

    void UpdateSelectionText(int index)
    {
        TextMeshProUGUI targetText = gamesList.GetListObject(index).GetComponentInChildren<TextMeshProUGUI>();
        targetText.text = SafeGetGameData(displayedGame + index).gameTitle;
        if (CheckAppRunning(SafeGetGameData(displayedGame + index))) {
            targetText.color = runningColor;
        } else {
            targetText.color = normalColor;
        }
    }

    void UpdateAllSelectionText()
    {
        UpdateSelectionText(0);

        for (int i = 1; i < gamesList.layers + 1; i++) {
            UpdateSelectionText(i);
            UpdateSelectionText(-i);
        }
    }

    void UpdateAllSelectionTextCustom(string txt)
    {
        for (int i = -gamesList.layers + 1; i < gamesList.layers; i++) {
            gamesList.GetListObject(i).GetComponentInChildren<TextMeshProUGUI>().text = txt;
        } 
    }

    GameData SafeGetGameData(int index)
    {
        if (index >= gameData.Count) {
            index %= gameData.Count;
        } else if (index < 0) {
            index = (gameData.Count - index * -1 % gameData.Count) % gameData.Count;
        }

        return gameData[index];
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

    IEnumerator MoveBanners(int direction, float duration)
    {
        if (direction == 0 || moving) yield break;
        if (direction < 0) direction = -1;
        else direction = 1;

        moving = true;

        Vector3[] startingPositions = new Vector3[gamesList.banners.Count];
        for (int i = 0; i < startingPositions.Length; i++) {
            startingPositions[i] = gamesList.banners[i].transform.position;
        }

        int[] accessors = new int[startingPositions.Length - 2];
        for (int i = 0; i < accessors.Length; i++) {
            if (direction > 0) {
                if (i % 2 == 0) {
                    accessors[i] = i + 2;
                } else {
                    accessors[i] = i - 2;
                    if (accessors[i] < 0) {
                        accessors[i] = 0;
                    }
                }
            } else {
                if (i % 2 == 1) {
                    accessors[i] = i + 2;
                } else {
                    accessors[i] = i - 2;
                    if (accessors[i] < 0) {
                        accessors[i] = 1;
                    }
                }
            }
        }

        float elapsedTime = 0.0f;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float completionPercent = elapsedTime / duration;

            for (int i = 0; i < accessors.Length; i++) {
                gamesList.banners[i].transform.position = Vector3.Lerp(startingPositions[i], startingPositions[accessors[i]], completionPercent);
            }

            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < startingPositions.Length; i++) {
            gamesList.banners[i].transform.position = startingPositions[i];
        }

        moving = false;
        targetGame += direction;

        PickDisplayGame();
        UpdateAllSelectionText();

        yield break;
    }
}
