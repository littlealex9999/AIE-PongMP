using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager instance;

    #region Game Objects
    [Header("Game Objects")]
    public Map map;
    public Ball ball;

    public GameObject pillarPrefab;
    public GameObject playerPrefab;
    List<GameObject> pillars = new List<GameObject>();

    public GameVariables defaultGameVariables;
    GameVariables gameVariables;

    public ArcTanShaderHelper arcTanShaderHelper;
    #endregion

    #region Map Settings
    [Header("Map Settings")]
    public AnimationCurve elimSpeedCurve;
    public float playerElimTime = 2.0f;

    [Range(0, 360)]
    public float mapRotationOffset = 0.0f;
    public float playerDistance = 4.0f;

    [ColorUsage(true, true), SerializeField]
    List<Color> playerEmissives = new List<Color>();
    #endregion

    #region Transformers
    [Header("Transformers")]
    public List<Transformer> transformers = new List<Transformer>();
    public float transformerSpawnRadius = 2.0f;
    public float transformerSpawnTime = 10.0f;
    float transformerSpawnTimer;
    public List<Transformer> activeTransformers = new List<Transformer>();
    #endregion

    #region UI
    [Header("UI")]
    public List<Image> playerImages;

    public GameObject shieldTextObj;
    public Transform shieldTextParent;
    public List<TextMeshProUGUI> shieldText = new List<TextMeshProUGUI>();


    public delegate void GameStateChange();
    public GameStateChange OnGameStateChange;

    public GameState gameState = GameState.MAINMENU;

    public enum GameState
    {
        MAINMENU,
        JOINMENU,
        SETTINGSMENU,
        GAMEPLAY,
        GAMEPAUSED,
        GAMEOVER,
    }
    #endregion

    #region Pause
    bool inGame;
    public bool holdGameplay { get { return smashingPillars; } }
    bool smashingPillars = false;
    #endregion

    #region Gameplay Settings
    float gameEndTimer;
    [HideInInspector] public List<Player> alivePlayers;
    [HideInInspector] public List<Player> players;

    [HideInInspector] public bool blackHoleActive = false;
    #endregion
    #endregion

    #region Functions
    #region Unity
    void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);

        if (defaultGameVariables) gameVariables = new GameVariables(defaultGameVariables);
        else gameVariables = new GameVariables();

        OnGameStateChange += OnGameStateChanged;

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
                    if (gameVariables.useTimer && gameEndTimer <= 0)
                    {
                        // UpdateGameState(GameState.GAMEOVER);
                    }

                    TransformerUpdate(Time.deltaTime);
                }
                break;
            default:
                break;
        }
    }
    #endregion

    #region HELPER
    public Color GetPlayerColor(int index)
    {
        if (index < playerEmissives.Count) return playerEmissives[index];
        else return playerEmissives[playerEmissives.Count - 1];
    }
    #endregion

    #region GAMESTATE
    public void UpdateGameState(GameState state)
    {
        if (state == GameState.GAMEPLAY && players.Count < 2) return;
        gameState = state;
        OnGameStateChange.Invoke();
    }

    private void OnGameStateChanged()
    {
        switch (gameState) {
            case GameState.MAINMENU:
                EventManager.instance?.mainMenuEvent?.Invoke();
                break;
            case GameState.JOINMENU:
                EventManager.instance?.joinMenuEvent?.Invoke();
                break;
            case GameState.SETTINGSMENU:
                EventManager.instance?.settingsMenuEvent?.Invoke();
                break;
            case GameState.GAMEPLAY:
                EventManager.instance?.gameplayEvent?.Invoke();
                if (!inGame)
                {
                    StartGame();
                }
                else
                {
                    UpdateShieldText();
                }
                break;
            case GameState.GAMEPAUSED:
                EventManager.instance?.gamePausedEvent?.Invoke();

                break;
            case GameState.GAMEOVER:
                EventManager.instance?.gameOverEvent?.Invoke();
                break;
            default:
                break;
        }
    }
    #endregion

    #region PLAYERS
    [ContextMenu("Create New Player")]
    public Player GetNewPlayer()
    {
        EventManager.instance.playerJoinEvent.Invoke();

        Player player = Instantiate(playerPrefab).GetComponent<Player>();
        player.color = GetPlayerColor(players.Count);
        players.Add(player);

        UpdatePlayerImages();
        return player;
    }

    public void RemovePlayer(Player playerToRemove)
    {
        if (playerToRemove == null) return;
        EventManager.instance.playerLeaveEvent.Invoke();

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
        StartCoroutine(EliminatePlayerRoutine(index));
    }

    public Color[] GenerateLivingColors()
    {
        Color[] colors = new Color[alivePlayers.Count];
        for (int i = 0; i < alivePlayers.Count; i++) {
            colors[i] = alivePlayers[i].color;
        }

        return colors;
    }
    #endregion

    #region GAMEPLAY MANAGEMENT
    void StartGame()
    {
        inGame = true;
        SetupPlayers();
        SetupBalls();
        SetupPillars();
        SetupMap();
        ball.ResetBall();
        UpdateShieldText();
    }

    void SetupPlayers()
    {
        foreach (Player p in players) alivePlayers.Add(p);

        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];

            player.gameObject.SetActive(true);

            player.shieldHealth = gameVariables.shieldLives;

            player.dashCooldown = gameVariables.dashCooldown;
            player.dashDuration = gameVariables.dashDuration;

            player.hitCooldown = gameVariables.hitCooldown;
            player.hitDuration = gameVariables.hitDuration;
            player.hitStrength = gameVariables.hitStrength;

            player.grabCooldown = gameVariables.grabCooldown;
            player.grabDuration = gameVariables.grabDuration;

            player.rotationalForce = gameVariables.playerRotationalForce;
            player.collider.normalBending = gameVariables.playerNormalBending;

            player.transform.localScale = gameVariables.playerSize;
            player.collider.scale = gameVariables.playerSize;
            player.collider.RecalculateNormals();

            player.CalculateLimits();
            player.SetPosition(player.playerSectionMiddle);
        }
    }

    void SetupBalls()
    {
        ball.dampStrength = gameVariables.ballSpeedDamp;
        ball.constantVel = gameVariables.ballSpeed;
        ball.transform.localScale = new Vector3(gameVariables.ballSize, gameVariables.ballSize, gameVariables.ballSize);
        ball.shieldBounceTowardsCenterBias = gameVariables.shieldBounceTowardsCenterBias;
        //ball.paddleBounceTowardsCenterBias = gameVariables.playerBounceTowardsCenterBias;
    }

    void SetupPillars()
    {
        for (int i = 0; i < players.Count; i++)
        {
            GameObject pillar = Instantiate(pillarPrefab, map.transform);
            pillar.transform.SetPositionAndRotation(
                map.GetTargetPointInCircle(360.0f / players.Count * i),
                Quaternion.Euler(0, 0, 360.0f / players.Count * i));
            pillars.Add(pillar);
        }
    }

    public void SetupMap()
    {
        map.GenerateMap();
        map.arcTangentShader.SetFloat("_Shrink", 0);
        arcTanShaderHelper.colors = new Color[alivePlayers.Count];
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            arcTanShaderHelper.colors[i] = alivePlayers[i].color;
        }
        arcTanShaderHelper.CreateTexture();
    }

    void BuildGameBoard()
    {
        ball.constantVel = gameVariables.ballSpeed;
        ball.transform.localScale = new Vector3(gameVariables.ballSize, gameVariables.ballSize, gameVariables.ballSize);
        ball.shieldBounceTowardsCenterBias = gameVariables.shieldBounceTowardsCenterBias;
        //ball.paddleBounceTowardsCenterBias = gameVariables.playerBounceTowardsCenterBias;

        gameEndTimer = gameVariables.timeInSeconds;

        UpdateShieldText();
    }
    #endregion

    #region GAMEPLAY
    void TransformerUpdate(float delta)
    {
        transformerSpawnTimer += delta;

        if (transformerSpawnTimer > transformerSpawnTime) {
            SpawnTransformer();
            transformerSpawnTimer = 0;
        }

        for (int i = 0; i < activeTransformers.Count; i++) {
            if (activeTransformers[i].limitedTime) {
                activeTransformers[i].duration -= delta;

                if (activeTransformers[i].duration <= 0) {
                    activeTransformers[i].EndModifier();
                }
            }
        }
    }

    [ContextMenu("Spawn Transformer")]
    public void SpawnTransformer()
    {
        Vector2 spawnPos = Random.insideUnitCircle;
        spawnPos *= Random.Range(0, transformerSpawnRadius);

        Instantiate(transformers[Random.Range(0, transformers.Count)], spawnPos, Quaternion.identity);
    }

    void UpdatePlayerImages()
    {
        foreach (Image image in playerImages) image.color = Color.white;
        for (int i = 0; i < players.Count; i++) {
            playerImages[i].color = GetPlayerColor(players[i].ID);
        }
    }

    private void UpdateShieldText()
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
        }
        else 
        {
            foreach (TextMeshProUGUI proUGUI in shieldText)
            {
                Destroy(proUGUI.gameObject);
            }
            shieldText.Clear();
            UpdateShieldText();
        }

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

            if (player.shieldHealth <= 0) EventManager.instance?.shieldBreakEvent?.Invoke();
            else EventManager.instance?.shieldHitEvent?.Invoke();
            UpdateShieldText();
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

        EventManager.instance?.playerEliminatedEvent?.Invoke();
        EventManager.instance?.towerMoveEvent?.Invoke();

        smashingPillars = true;

        index %= pillars.Count;
        arcTanShaderHelper.SetTargetPlayer(index);

        float pillarSmashTimer = 0.0f;

        float[] playerStartAngles = new float[alivePlayers.Count];
        float[] playerTargetAngles = new float[alivePlayers.Count];

        Vector3 elimPlayerStartScale = alivePlayers[index].transform.localScale;

        // calculate start and end angle for each player
        for (int i = 0; i < alivePlayers.Count; i++) {
            playerStartAngles[i] = Player.Angle(alivePlayers[i].transform.position);
            int targetPlayerIndex = i;
            if (i > index) --targetPlayerIndex;

            if (i == index) {
                // player being eliminated
                playerTargetAngles[i] = 360.0f / (alivePlayers.Count - 1) * targetPlayerIndex;
            } else {
                playerTargetAngles[i] = 180.0f / (alivePlayers.Count - 1) + 360.0f / (alivePlayers.Count - 1) * targetPlayerIndex;
            }
        }

        // move pillars over time & handle ArcTanShader shrinkage
        while (pillarSmashTimer < playerElimTime) {
            pillarSmashTimer += Time.deltaTime;
            float playerRemovalPercentage = elimSpeedCurve.Evaluate(pillarSmashTimer / playerElimTime);

            float pseudoPlayerCount = alivePlayers.Count - playerRemovalPercentage;
            for (int i = 0; i < alivePlayers.Count; i++) {
                float targetAngle = 360.0f / pseudoPlayerCount * i;
                if (i > index) {
                    int countAfter = alivePlayers.Count - i;
                    targetAngle = 360.0f / alivePlayers.Count * i - 360.0f / alivePlayers.Count / pseudoPlayerCount * playerRemovalPercentage * countAfter;
                }

                pillars[i].transform.position = map.GetTargetPointInCircle(targetAngle);
                pillars[i].transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            }

            arcTanShaderHelper.SetTargetPlayer(index);
            arcTanShaderHelper.SetShrink(playerRemovalPercentage);

            for (int i = 0; i < alivePlayers.Count; i++) {
                if (i == index) {
                    Vector3 targetScale = new Vector3(
                        alivePlayers[i].transform.localScale.x,
                        Mathf.Lerp(elimPlayerStartScale.y, 0, playerRemovalPercentage),
                        Mathf.Lerp(elimPlayerStartScale.z, 0, playerRemovalPercentage)
                        );
                    alivePlayers[i].transform.localScale = targetScale;
                }

                alivePlayers[i].SetPosition(Mathf.Lerp(playerStartAngles[i], playerTargetAngles[i], playerRemovalPercentage));
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(pillars[index]);
        pillars.RemoveAt(index);

        // ensure each pillar is exactly where it was calculated to belong
        for (int i = 0; i < pillars.Count; i++) {
            float targetAngle = 360.0f / (pillars.Count) * i;

            pillars[i].transform.position = map.GetTargetPointInCircle(targetAngle);
            pillars[i].transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }

        alivePlayers[index].gameObject.SetActive(false);
        alivePlayers.RemoveAt(index);
        for (int i = 0; i < alivePlayers.Count; i++) 
        {
            alivePlayers[i].CalculateLimits();
        }

        UpdateShieldText();
        
        arcTanShaderHelper.colors = GenerateLivingColors();
        arcTanShaderHelper.CreateTexture();
        arcTanShaderHelper.SetShrink(0.0f);

        ball.ResetBall();

        smashingPillars = false;
        yield break;
    }
    #endregion
    #endregion
}
