using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    List<GameObject> pillars = new List<GameObject>();

    public Map map;
    public Ball ball;
    public ArcTanShaderHelper arcTanShader;

    public GameObject pillarObject;

    public AnimationCurve pillarCurve;
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
        if (!instance) instance = this;
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
                EventManager.instance?.mainMenu?.Invoke();
                break;
            case GameState.JOINMENU:
                EventManager.instance?.joinMenu?.Invoke();
                break;
            case GameState.SETTINGSMENU:
                EventManager.instance?.settingsMenu?.Invoke();
                break;
            case GameState.GAMEPLAY:
                EventManager.instance?.gameplay?.Invoke();
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
                EventManager.instance?.gamePaused?.Invoke();

                break;
            case GameState.GAMEOVER:
                EventManager.instance?.gameOver?.Invoke();
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

    [ContextMenu("Create New Player")]
    public Player GetNewPlayer()
    {
        GameObject playerObject = Instantiate(playerPrefab);
        playerObject.transform.parent = transform;
        Player player = playerObject.GetComponent<Player>();
        player.color = GetPlayerColor(players.Count);
        players.Add(player);
        UpdatePlayerImages();
        return player;
    }

    public void ResetPlayers()
    {
        alivePlayers.Clear();

        foreach (Player player in players)
        {
            player.shieldHealth = gameVariables.shieldLives;
            alivePlayers.Add(player);
        }
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
        if (alivePlayers.Count <= 2)
        {
            inGame = false;
            UpdateGameState(GameState.GAMEOVER);
            return;
        }
        int index = alivePlayers.IndexOf(player);
        player.paddle.gameObject.SetActive(false);
        StartCoroutine(EliminatePlayerRoutine(index));

        BuildGameBoard();
    }

    //
    // Gameplay
    //

    void StartGame()
    {
        foreach (Player player in players)
        {
            player.dashCooldown = gameVariables.dashCooldown;
            player.dashDuration = gameVariables.dashDuration;
        }
        inGame = true;
        ResetPlayers();
        UpdatePaddles();
        map.SetupMap(alivePlayers);
        ball.ResetBall();
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
        ball.constantVel = gameVariables.ballSpeed;
        ball.transform.localScale = new Vector3(gameVariables.ballSize, gameVariables.ballSize, gameVariables.ballSize);
        ball.shieldBounceTowardsCenterBias = gameVariables.shieldBounceTowardsCenterBias;
        ball.paddleBounceTowardsCenterBias = gameVariables.playerBounceTowardsCenterBias;

        gameEndTimer = gameVariables.timeInSeconds;
        
        UpdateShields();
    }

    void UpdatePaddles()
    {
        float segmentOffset = 180.0f / alivePlayers.Count;

        // pillars can now be accessed like alivePlayers
        // ignore while holdGameplay because that likely means something special is happening like the pillar smash
        if (!holdGameplay) {
            while (alivePlayers.Count != pillars.Count) {
                if (pillars.Count > alivePlayers.Count) {
                    Destroy(pillars[pillars.Count - 1]);
                    pillars.RemoveAt(pillars.Count - 1);
                } else {
                    pillars.Add(Instantiate(pillarObject, map.transform));
                }
            }
        }

        for (int i = 0; i < alivePlayers.Count; i++)
        {
            alivePlayers[i].paddle.gameObject.SetActive(true);
            alivePlayers[i].paddle.CalculateLimits(i, alivePlayers.Count, mapRotationOffset);
            alivePlayers[i].paddle.SetPosition(alivePlayers[i].paddle.playerSectionMiddle);

            float playerMidPos = 360.0f / alivePlayers.Count * (i + 1) + mapRotationOffset - segmentOffset;    

            pillars[i].transform.position = map.GetTargetPointInCircle(360.0f / alivePlayers.Count * i);
            pillars[i].transform.rotation = Quaternion.Euler(0, 0, 360.0f / alivePlayers.Count * i);
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

    /// <summary>
    /// Manages shield health and player elimination. Returns true if a hit causes a player to die.
    /// </summary>
    /// <param name="alivePlayerID"></param>
    /// <returns></returns>
    public bool OnSheildHit(int alivePlayerID)
    {
        if (alivePlayerID >= alivePlayers.Count) return false;

        Player player = alivePlayers[alivePlayerID];
        if (player.shieldHealth <= 0)
        {
            EliminatePlayer(player);
            return true;
        } else
        {
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
    IEnumerator EliminatePlayerRoutine(int index)
    {
        if (smashingPillars) throw new Exception("Pillars are already being smashed");

        smashingPillars = true;

        index %= pillars.Count;
        arcTanShader.SetTargetPlayer(index);

        float pillarSmashTimer = 0.0f;

        float[] startAngles = new float[pillars.Count];
        float[] targetAngles = new float[pillars.Count];
        float[] playerStartAngles = new float[alivePlayers.Count - 1];
        float[] playerTargetAngles = new float[alivePlayers.Count - 1];

        // calculate start and end angle for each pillar
        for (int i = 0; i < pillars.Count; i++) {
            startAngles[i] = 360.0f / pillars.Count * i;
            targetAngles[i] = 360.0f / (pillars.Count - 1);

            if (i > index) {
                targetAngles[i] *= i - 1;
            } else {
                targetAngles[i] *= i;
            }
        }

        // calculate start and end angle for each player
        for (int i = 0; i < alivePlayers.Count - 1; i++) {
            int targetPlayer;
            if (i < index) targetPlayer = i;
            else targetPlayer = i + 1;

            playerStartAngles[i] = Paddle.Angle(alivePlayers[targetPlayer].paddle.transform.position);
            playerTargetAngles[i] = 180.0f / (alivePlayers.Count - 1) + 360.0f / (alivePlayers.Count - 1) * i;
        }

        // move pillars over time & handle ArcTanShader shrinkage
        while (pillarSmashTimer < pillarSmashTime) {
            pillarSmashTimer += Time.deltaTime;
            float playerRemovalPercentage = pillarCurve.Evaluate(pillarSmashTimer / pillarSmashTime);
            arcTanShader.SetShrink(playerRemovalPercentage);

            for (int i = 0; i < pillars.Count; i++) {
                float targetAngle = Mathf.Lerp(startAngles[i], targetAngles[i], playerRemovalPercentage);
                
                pillars[i].transform.position = map.GetTargetPointInCircle(targetAngle);
                pillars[i].transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            }

            for (int i = 0; i < alivePlayers.Count - 1; i++) {
                int targetPlayer;
                if (i < index) {
                    targetPlayer = i;
                } else if (i > index) {
                    targetPlayer = i + 1;
                } else {
                    // player being eliminated
                    // SQUISH PLAYER

                    continue;
                }

                alivePlayers[targetPlayer].paddle.SetPosition(Mathf.Lerp(playerStartAngles[i], playerTargetAngles[i], playerRemovalPercentage));
            }

            yield return new WaitForEndOfFrame();
        }

        // ensure each pillar is exactly where it was calculated to belong
        for (int i = 0; i < pillars.Count; i++) {
            pillars[i].transform.position = map.GetTargetPointInCircle(targetAngles[i]);
            pillars[i].transform.rotation = Quaternion.Euler(0, 0, targetAngles[i]);
        }

        alivePlayers[index].paddle.gameObject.SetActive(false);
        alivePlayers.RemoveAt(index);
        for (int i = 0; i < alivePlayers.Count; i++) {
            alivePlayers[i].paddle.CalculateLimits(i, alivePlayers.Count, mapRotationOffset);
        }

        Destroy(pillars[index]);
        pillars.RemoveAt(index);

        map.SetupMap(alivePlayers);
        ball.ResetBall();

        smashingPillars = false;
        yield break;
    }
}
