using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManagerInstance;

    List<GameObject> pillars = new List<GameObject>();

    public Map map;
    public Ball ball;

    public GameObject paddleObject;
    public GameObject pillarObject;

    public float pillarSmashTime = 2.0f;

    public GameVariables defaultGameVariables;
    GameVariables gameVariables;

    [Range(0, 360)] public float mapRotationOffset = 0.0f;

    [ColorUsage(true, true), SerializeField] List<Color> playerEmissives = new List<Color>();

    public float playerDistance = 4.0f;
    float gameEndTimer;

    public List<Image> playerImages;

    public GameObject shieldTextObj;
    public Transform shieldTextParent;
    public List<TextMeshProUGUI> shieldText = new List<TextMeshProUGUI>();

    bool inGame;
    public bool holdGameplay { get { return smashingPillars; } }
    bool smashingPillars = false;

    void Awake()
    {
        if (!gameManagerInstance) gameManagerInstance = this;
        else Destroy(this);

        if (defaultGameVariables) gameVariables = new GameVariables(defaultGameVariables);
        else gameVariables = new GameVariables();

        if (gameStateChanged == null) gameStateChanged = new UnityEvent();

        gameStateChanged.AddListener(OnGameStateChanged);

        UpdateGameState(GameState.MAINMENU);
    }

   void Update()
    {
        switch (gameState)
        {
            case GameState.GAMEPLAY:
                if (!holdGameplay)
                 {
                    gameEndTimer -= Time.deltaTime;
                    if (gameEndTimer <= 0)
                    {
                        // end game
                    }
                }
                break;
            default:
                break;
        }
    }

    //
    // GameState
    //
    public UnityEvent gameStateChanged;
    public GameState gameState { get; private set; }
    public enum GameState
    {
        MAINMENU,
        JOINMENU,
        SETTINGSMENU,
        GAMEPLAY,
        GAMEPAUSED,
        GAMEOVER,
    }
    public void UpdateGameState(GameState state)
    {
        gameState = state;
        gameStateChanged.Invoke();
    }

    void OnGameStateChanged()
    {
        switch (gameState) {
            case GameState.MAINMENU:

                break;
            case GameState.JOINMENU:

                break;
            case GameState.SETTINGSMENU:

                break;
            case GameState.GAMEPLAY:
                if (!inGame)
                {
                    StartGame(); 
                }
                else
                {
                    BuildGameBoard();
                }
                break;
            case GameState.GAMEPAUSED:

                break;
            case GameState.GAMEOVER:

                break;
            default:
                break;
        }
    }

    //
    // Players
    //
    public GameObject playerPrefab;

    public List<Player> alivePlayers;
    public List<Player> players;
    
    public Player GetNewPlayer()
    {
        GameObject playerObject = Instantiate(playerPrefab);
        playerObject.transform.parent = transform;
        Player player = playerObject.GetComponent<Player>();
        player.paddle.SetColor(GetPlayerColor(players.Count));
        players.Add(player);
        UpdatePlayerImages();
        return player;
    }

    public void ResetPlayers()
    {
        foreach (Player player in players)
        {
            player.shieldHealth = gameVariables.shieldLives;
        }
        alivePlayers = players;
        UpdatePlayerImages();
    }

    public void RemovePlayer(Player playerToRemove)
    {
        if (playerToRemove == null) return;
        players.Remove(playerToRemove);
        Destroy(playerToRemove);
        UpdatePlayerImages();
    }
    public void EliminatePlayer(Player player)
    {
        player.paddle.gameObject.SetActive(false);
        StartCoroutine(SmashPillars(alivePlayers.IndexOf(player)));
        alivePlayers.Remove(player);
        if (alivePlayers.Count == 1) UpdateGameState(GameState.GAMEOVER);
        BuildGameBoard();
    }

    //
    // Gameplay
    //

    void StartGame()
    {
        inGame = true;
        ResetPlayers();
        BuildGameBoard();
    }

    void UpdatePlayerImages()
    {
        foreach (Image image in playerImages) image.color = Color.white;
        for (int i = 0; i < players.Count; i++)
        {
            playerImages[i].color = GetPlayerColor(players[i].ID);
        }
    }


    void BuildGameBoard()
    {
        ball.transform.position = map.transform.position;
        ball.constantVel = gameVariables.ballSpeed;
        ball.transform.localScale = new Vector3(gameVariables.ballSize, gameVariables.ballSize, gameVariables.ballSize);
        gameEndTimer = gameVariables.timeInSeconds;
        map.GenerateMap();
        UpdatePaddles();
        UpdateShields();
    }

    void UpdatePaddles()
    {
        float segmentOffset = 180.0f / alivePlayers.Count;

        // pillars can now be accessed like alivePlayers
        while (alivePlayers.Count != pillars.Count) {
            if (pillars.Count > alivePlayers.Count) {
                Destroy(pillars[pillars.Count - 1]);
                pillars.RemoveAt(pillars.Count - 1);
            } else {
                pillars.Add(Instantiate(pillarObject, map.transform));
            }
        }

     

        for (int i = 0; i < alivePlayers.Count; i++)
        {
            alivePlayers[i].paddle.gameObject.SetActive(true);
            alivePlayers[i].paddle.CalculateLimits(i, alivePlayers.Count, mapRotationOffset);

            float playerMidPos = 360.0f / alivePlayers.Count * (i + 1) + mapRotationOffset - segmentOffset;    

            pillars[i].transform.position = map.GetTargetPointInCircle(playerMidPos - segmentOffset);
            pillars[i].transform.rotation = Quaternion.Euler(0, 0, playerMidPos - segmentOffset);
        }
    }

    void ResetGame()
    {
        alivePlayers.Clear();
        foreach (Player player in players)
        {
            player.paddle.gameObject.SetActive(false);
            alivePlayers.Add(player);
        }
        BuildGameBoard();
    }

    private void UpdateShields()
    {
        if (shieldText.Count == 0)
        {
            Vector3 nextPos = Vector3.zero;

            for (int i = 0; i < alivePlayers.Count; i++)
            {
                TextMeshProUGUI proUGUI;
                nextPos.y = i * -50;
                Instantiate(shieldTextObj, nextPos, Quaternion.identity).TryGetComponent(out proUGUI);
                if (proUGUI != null)
                {
                    proUGUI.transform.SetParent(shieldTextParent, false);
                    proUGUI.text = i.ToString() + ": " + alivePlayers[i].shieldHealth.ToString();
                    shieldText.Add(proUGUI);
                }
            }
        } else {
            foreach (TextMeshProUGUI proUGUI in shieldText) {
                Destroy(proUGUI.gameObject);
            }
            shieldText.Clear();
            UpdateShields();
        }

    }

    public Color GetPlayerColor(int index)
    {
        if (index < playerEmissives.Count) return playerEmissives[index];
        else return playerEmissives[playerEmissives.Count - 1];
    }

    // returns true if a hit causes a player to die.
    public bool OnSheildHit(int alivePlayerID)
    {
        if (alivePlayerID >= alivePlayers.Count) return false;

        Player player = alivePlayers[alivePlayerID];
        if (player.shieldHealth <= 0)
        {
            EliminatePlayer(player);
            return true;
        } else {
            player.shieldHealth--;
            UpdateShields();
            return false;
        }
    }

    /// <summary>
    /// Smashes the given pillar and the one after it together. Puts the game on hold by setting smashingPillars to true, returning to gameplay when it is done.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IEnumerator SmashPillars(int index)
    {
        smashingPillars = true;

        index %= pillars.Count;
        int nextIndex = (index + 1) % pillars.Count;

        float pillarSmashTimer = 0.0f;

        while (pillarSmashTimer < pillarSmashTime) {
            pillarSmashTimer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        smashingPillars = false;
        yield break;
    }
}
