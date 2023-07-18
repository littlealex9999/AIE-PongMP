using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    List<AppHelper> runningApps = new List<AppHelper>();

    List<Image> visibleGames = new List<Image>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    private void Update()
    {
        for (int i = 0; i < runningApps.Count; ++i) {
            if (runningApps[i] != null && runningApps[i].hasExited) {

            }
        }

        if (Input.GetButtonDown("Select")) {
            Debug.Log(Application.persistentDataPath);
            // runningApps.Add(new AppHelper(Application.persistentDataPath));
        }
    }
}
