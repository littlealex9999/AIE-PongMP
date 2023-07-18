using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    List<AppHelper> runningApps = new List<AppHelper>();

    List<GameObject> visibleGames = new List<GameObject>();

    public TextMeshProUGUI text;
    public Color normalColor = Color.white;
    public Color runningColor = Color.green;

    readonly string gameLocationsFilePath = Application.persistentDataPath + "/gamePaths";
    List<string> gameLocations;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        gameLocations = FileManager.ReadStringArray(gameLocationsFilePath);
        if (gameLocations == null) gameLocations = new List<string>();
    }

    private void OnApplicationQuit()
    {
        if (gameLocations != null) FileManager.WriteStringArray(gameLocationsFilePath, gameLocations);
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
                runningApps.Add(new AppHelper(Application.persistentDataPath + "/" + "RunUntilInput.exe"));
                text.color = runningColor;
            }
        }
    }
}
