using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager instance;
    #region Game Objects
    [Header("Game Objects")]
 
    public Ball ballPrefab;
    [HideInInspector] public List<Ball> balls = new List<Ball>();

    public GameObject pillarPrefab;
    public GameObject playerPrefab;
    List<GameObject> pillars = new List<GameObject>();

    public GameVariables defaultGameVariables;
    [HideInInspector] public GameVariables gameVariables;

    public ArcTanShaderHelper arcTanShaderHelper;

    [HideInInspector] public List<ControllerInputHandler> controllers;
    #endregion

    #region Map Settings
    [Header("Map Settings")]
    public float mapRadius = 4.5f;
    public AnimationCurve elimSpeedCurve;
    public float playerElimTime = 2.0f;
    public float playerElimBallSpinSpeed = 2.0f;

    [Range(0, 360)]
    public float mapRotationOffset = 0.0f;
    public float playerDistance = 4.0f;

    [ColorUsage(true, true), SerializeField]
    List<Color> playerEmissives = new List<Color>();
    #endregion

    #region Transformers
    [Header("Transformers")]
    public List<Transformer> transformers = new List<Transformer>();
    List<Transformer> allowedTransformers = new List<Transformer>();
    public float transformerSpawnRadius = 2.0f;
    public float transformerSpawnTime = 10.0f;
    float transformerSpawnTimer;
    [HideInInspector] public List<Transformer> spawnedTransformers = new List<Transformer>();
    [HideInInspector] public List<Transformer> activeTransformers = new List<Transformer>();
    #endregion

    #region UI
    [Header("UI")]
    public List<Image> controllerImages;
    public List<Image> halfControllerImages;
    public Color imageDefaultColor;

    public GameObject shieldTextObj;
    public Transform shieldTextParent;
    public List<TextMeshProUGUI> shieldText = new List<TextMeshProUGUI>();

    // The expected structure is that the image will have a sibling image relevant to it
    // so it should be treated as if it always has a parent
    public List<Image> endGamePlayerImages = new List<Image>();
    public List<int> endGamePlayerEnableThresholds = new List<int>();
    public Transform endGameAlternatePosition;
    Vector3 endGameRestorePosition;

    public delegate void GameStateChange();
    public GameStateChange OnGameStateChange;

    [HideInInspector] public GameState gameState = GameState.MAINMENU;

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
    public bool holdGameplay { get { return smashingPillars || countdownTimer > 0; } }
    [HideInInspector] public bool smashingPillars = false;

    float countdownTime = 3.0f;
    float countdownTimer = 0.0f;
    #endregion

    #region Gameplay Settings
    float gameEndTimer;
    [HideInInspector] public List<Player> players;
    [HideInInspector] public List<Player> alivePlayers;
    [HideInInspector] public List<Player> elimPlayers;

    [HideInInspector] public BlackHole blackHole;
    #endregion
    #endregion

    #region Functions
    #region Unity
    void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);

        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;

        if (defaultGameVariables) gameVariables = new GameVariables(defaultGameVariables);
        else gameVariables = new GameVariables();

        OnGameStateChange += OnGameStateChanged;

        SetupGameEndData();
        UpdateGameState(GameState.MAINMENU);
    }

    void Update()
    {
        switch (gameState) {
            case GameState.GAMEPLAY:
                if (!holdGameplay) {
                    gameEndTimer -= Time.deltaTime;
                    if (gameVariables.useTimer && gameEndTimer <= 0) {
                        // UpdateGameState(GameState.GAMEOVER);
                    }

                    TransformerUpdate(Time.deltaTime);
                } else if (countdownTimer > 0) {
                    countdownTimer -= Time.deltaTime;
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

    public Vector2 GetCircleIntersection(Vector2 startPos, Vector2 direction, float radius)
    {
        Vector2 p1 = startPos;
        Vector2 p2 = startPos + direction * mapRadius * 2;

        Vector2 dist = new Vector2(p2.x - p1.x, p2.y - p1.y);
        float a = dist.x * dist.x + dist.y * dist.y;
        float b = 2 * (dist.x * p1.x + dist.y * p1.y);
        float c = p1.x * p1.x + p1.y * p1.y;
        c -= radius * radius;
        float bb4ac = b * b - 4 * a * c;

        float mu1 = (-b + Mathf.Sqrt(bb4ac)) / (2 * a);

        return new Vector2(p1.x + mu1 * (p2.x - p1.x), p1.y + mu1 * (p2.y - p1.y));
    }

    public Vector2[] GetCircleIntersectionDouble(Vector2 startPos, Vector2 direction, float radius)
    {
        Vector2 p1 = startPos;
        Vector2 p2 = startPos + direction * mapRadius * 2;

        Vector2 dist = new Vector2(p2.x - p1.x, p2.y - p1.y);
        float a = dist.x * dist.x + dist.y * dist.y;
        float b = 2 * (dist.x * p1.x + dist.y * p1.y);
        float c = p1.x * p1.x + p1.y * p1.y;
        c -= radius * radius;
        float bb4ac = b * b - 4 * a * c;

        float mu1 = (-b + Mathf.Sqrt(bb4ac)) / (2 * a);
        float mu2 = (-b - Mathf.Sqrt(bb4ac)) / (2 * a);

        return new Vector2[2] {
            new Vector2(p1.x + mu1 * (p2.x - p1.x), p1.y + mu1 * (p2.y - p1.y)),
            new Vector2(p1.x + mu2 * (p2.x - p1.x), p1.y + mu2 * (p2.y - p1.y)),
        };
    }

    public static void QuitGame()
    {
        Application.Quit();
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
                EventManager.instance?.menuEvent.Invoke();
                break;
            case GameState.JOINMENU:
                EventManager.instance?.menuEvent.Invoke();
                break;
            case GameState.SETTINGSMENU:
                EventManager.instance?.menuEvent.Invoke();
                break;
            case GameState.GAMEPLAY:
                EventManager.instance?.gameplayEvent?.Invoke();

                if (!inGame) {
                    StartGame();
                } else {
                    UpdateShieldText();
                }
                break;
            case GameState.GAMEPAUSED:
                EventManager.instance?.menuEvent.Invoke();

                break;
            case GameState.GAMEOVER:
                if (inGame) {
                    // EndGame calls a gamestate change to GAMEOVER so we must ensure it doesn't infinitely repeat and return
                    EndGame();
                    return;
                }

                EventManager.instance?.menuEvent?.Invoke();

                for (int i = 0; i < elimPlayers.Count && i < endGamePlayerImages.Count; i++) {
                    endGamePlayerImages[i].color = elimPlayers[elimPlayers.Count - (i + 1)].color;
                }

                if (elimPlayers.Count == 2) {
                    endGamePlayerImages[1].transform.parent.position = endGameAlternatePosition.position;
                } else {
                    endGamePlayerImages[1].transform.parent.localPosition = endGameRestorePosition;
                }

                int lastAccess = 0;
                for (int i = 0; i < endGamePlayerEnableThresholds.Count; i++) {
                    if (endGamePlayerEnableThresholds[i] > elimPlayers.Count) {
                        for (int j = lastAccess + 1; j < endGamePlayerImages.Count; j++) {
                            endGamePlayerImages[j].transform.parent.gameObject.SetActive(false);
                        }
                        break;
                    }

                    for (int j = lastAccess; j < endGamePlayerImages.Count; j++) {
                        if (j >= endGamePlayerEnableThresholds[i]) break;
                        lastAccess = j;
                        endGamePlayerImages[j].transform.parent.gameObject.SetActive(true);
                    }
                }
                break;
            default:
                break;
        }
    }

    void SetupGameEndData()
    {
        if (endGamePlayerImages.Count > 1) {
            endGameRestorePosition = endGamePlayerImages[1].transform.parent.localPosition;
        }
    }
    #endregion

    #region PLAYERS

    [ContextMenu("Create New Player")]
    public Player GetNewPlayer()
    {
        if (controllers.Count > 4 || players.Count >= playerEmissives.Count) return null;
        EventManager.instance.playerJoinEvent.Invoke();

        Player player = Instantiate(playerPrefab).GetComponent<Player>();
        player.gameObject.SetActive(false);

        //for (int i = 0; i < playerEmissives.Count; i++)
        //{
        //    if (i >= players.Count)
        //    {
        //        player.ID = i;
        //        break;
        //    }

        //    if (players[i].ID == i) continue;

        //    player.ID = i;
        //    break;
        //}
        //player.color = GetPlayerColor(player.ID);

        players.Add(player);

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
        //if (alivePlayers.Count <= 2) {
        //    EndGame();
        //    return;
        //}
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
        ResetBalls();
        UpdateShieldText();
        SetupTransformers();
    }

    void EndGame()
    {
        inGame = false;
        for (int i = 0; i < players.Count; i++) {
            players[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < balls.Count; i++) {
            Destroy(balls[i].gameObject);
        }
        balls.Clear();

        for (int i = 0; i < spawnedTransformers.Count; i++) {
            if (spawnedTransformers[i] != null) Destroy(spawnedTransformers[i].gameObject);
        }
        spawnedTransformers.Clear();
        activeTransformers.Clear();

        if (blackHole) {
            Destroy(blackHole.gameObject);
            blackHole = null;
        }


        for (int i = 0; i < alivePlayers.Count; i++) {
            elimPlayers.Add(alivePlayers[i]);
        }
        UpdateGameState(GameState.GAMEOVER);
    }

    void SetupPlayers()
    {
        elimPlayers.Clear();
        alivePlayers.Clear();

        for (int i = 0; i < players.Count; i++) {
            Player player = players[i];

            player.gameObject.SetActive(true);

            player.moveSpeed = gameVariables.playerSpeed;

            player.rotationalForce = gameVariables.playerRotationalForce;
            player.collider.normalBending = gameVariables.playerNormalBending;

            player.dashCooldown = gameVariables.dashCooldown;
            player.dashDuration = gameVariables.dashDuration;

            player.hitCooldown = gameVariables.hitCooldown;
            player.hitDuration = gameVariables.hitDuration;
            player.hitStrength = gameVariables.hitStrength;

            player.grabCooldown = gameVariables.grabCooldown;
            player.grabDuration = gameVariables.grabDuration;

            player.shieldHealth = gameVariables.shieldLives;

            alivePlayers.Add(player);
        }
        UpdateAlivePlayers();
    }

    void UpdateAlivePlayers()
    {
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            Player player = alivePlayers[i];

            player.shieldHealth = gameVariables.shieldLives;

            Vector3 playerSize = gameVariables.playerSizes[alivePlayers.Count - 2];
            player.transform.localScale = playerSize;
            player.collider.scale = new Vector2(playerSize.y, playerSize.x);

            player.collider.RecalculateScale();

            player.CalculateLimits();

            player.SetPosition(player.playerSectionMiddle);
        }
    }

    void SetupBalls()
    {
        EventManager.instance.ballCountdownEvent.Invoke();

        for (int i = 0; i < gameVariables.ballCount; i++) {
            Ball b = Instantiate(ballPrefab);

            b.transform.position = Vector2.zero;
            b.constantSpd = gameVariables.ballSpeed;
            b.transform.localScale = new Vector3(gameVariables.ballSize, gameVariables.ballSize, gameVariables.ballSize);
            b.collider.radius = gameVariables.ballSize / 2;
            b.dampStrength = gameVariables.ballSpeedDamp;
            b.shieldBounceTowardsCenterBias = gameVariables.shieldBounceTowardsCenterBias;
            balls.Add(b);
        }
    }

    void ResetBalls()
    {
        if (alivePlayers.Count > 1) EventManager.instance.ballCountdownEvent.Invoke();
        countdownTimer = countdownTime;

        for (int i = balls.Count - 1; i > gameVariables.ballCount; i--) {
            Destroy(balls[i]);
            balls.RemoveAt(i);
        }

        int player = Random.Range(0, alivePlayers.Count);
        Vector2 dir = (alivePlayers[player].transform.position - Vector3.zero).normalized;
        for (int i = 0; i < balls.Count; i++) {
            balls[i].gameObject.SetActive(true);
            balls[i].collider.enabled = true;
            balls[i].collider.immovable = true;
            balls[i].collider.velocity = Quaternion.Euler(0, 0, 360.0f / balls.Count * i) * dir * balls[i].constantSpd;
            balls[i].transform.position = Vector2.zero;
        }
    }

    void SetupPillars()
    {
        while (pillars.Count != players.Count) {
            if (pillars.Count < players.Count) {
                pillars.Add(Instantiate(pillarPrefab));
            } else {
                Destroy(pillars[pillars.Count - 1]);
                pillars.RemoveAt(pillars.Count - 1);
            }
        }

        for (int i = 0; i < players.Count; i++) {
            pillars[i].transform.SetPositionAndRotation(
                GetTargetPointInCircle(360.0f / players.Count * i),
                Quaternion.Euler(0, 0, 360.0f / players.Count * i));
        }
    }

    public Vector3 GetTargetPointInCircle(float angle)
    { 
       return Vector3.zero + Quaternion.Euler(0, 0, angle) * Vector3.up * mapRadius;
    }

    public void SetupMap()
    {
        arcTanShaderHelper.SetShrink(0);
        arcTanShaderHelper.colors = new Color[alivePlayers.Count];
        for (int i = 0; i < alivePlayers.Count; i++) {
            arcTanShaderHelper.colors[i] = alivePlayers[i].color;
        }
        arcTanShaderHelper.CreateTexture();
    }

    void SetupTransformers()
    {
        for (int i = 0; i < transformers.Count; i++) {
            if ((transformers[i].GetTransformerType() & gameVariables.enabledTransformers) != 0) {
                allowedTransformers.Add(transformers[i]);
            }
        }

        CleanTransformers();
    }

    void CleanTransformers()
    {
        for (int i = 0; i < activeTransformers.Count; i++) {
            if (activeTransformers[i] != null) {
                activeTransformers[i].EndModifier();
            }
        }

        for (int i = 0; i < spawnedTransformers.Count; i++) {
            if (spawnedTransformers[i] != null) {
                Destroy(spawnedTransformers[i].gameObject);
            }
        }

        spawnedTransformers.Clear();
        activeTransformers.Clear();

        if (blackHole) {
            Destroy(blackHole.gameObject);
            blackHole = null;
        }
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

        for (int i = 0; i < spawnedTransformers.Count; i++) {
            spawnedTransformers[i].despawnTime -= delta;
            if (spawnedTransformers[i].despawnTime <= 0.0f) {
                Destroy(spawnedTransformers[i].gameObject);
                spawnedTransformers.RemoveAt(i);
                --i;
            }
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

    public Vector2 GetTransformerSpawnPoint()
    {
        Vector2 ret = Vector2.zero;

        for (int i = 0; i < balls.Count; i++) {
            Vector2 clampedPos = Vector2.ClampMagnitude(-balls[i].transform.position, transformerSpawnRadius);
            Vector2 velocityPerp = new Vector2(balls[i].collider.velocity.y, -balls[i].collider.velocity.x);
            Vector2[] intersections = GetCircleIntersectionDouble(clampedPos, velocityPerp, transformerSpawnRadius);
            if ((intersections[0] - (Vector2)balls[i].transform.position).sqrMagnitude > (intersections[1] - (Vector2)balls[i].transform.position).sqrMagnitude) {
                ret += intersections[0];
            } else {
                ret += intersections[1];
            }
        }

        ret /= balls.Count;

        return ret;
    }

    [ContextMenu("Spawn Transformer")]
    public void SpawnTransformer()
    {
        Transformer[] passedTransformers = new Transformer[allowedTransformers.Count];
        int hits = 0;

        for (int i = 0; i < allowedTransformers.Count; i++) {
            bool allowed = true;
            if (allowedTransformers[i] is BlackHoleSpawn) {
                if (blackHole) {
                    allowed = false;
                }

                for (int j = 0; j < spawnedTransformers.Count; j++) {
                    if (spawnedTransformers[j] is BlackHoleSpawn) {
                        allowed = false;
                        break;
                    }
                }
            }
            
            for (int j = 0; j < spawnedTransformers.Count; j++)
            {
                if (allowedTransformers[i].GetTransformerType() == spawnedTransformers[j].GetTransformerType())
                {
                    allowed = false;
                    break;
                }
            }

            for (int j = 0; j < activeTransformers.Count; j++)
            {
                if (allowedTransformers[i].GetTransformerType() == activeTransformers[j].GetTransformerType())
                {
                    allowed = false;
                    break;
                }
            }



            if (allowed) {
                ++hits;
                for (int j = 0; j < passedTransformers.Length; j++) {
                    if (passedTransformers[j] == null) {
                        passedTransformers[j] = allowedTransformers[i];
                        break;
                    }
                }
            }
        }

        if (hits > 0) {
            spawnedTransformers.Add(Instantiate(passedTransformers[Random.Range(0, hits)], GetTransformerSpawnPoint(), Quaternion.identity));
        }
    }

    public void UpdatePlayerImages()
    {
        for (int i = 0; i < controllerImages.Count; i++)
        {
            controllerImages[i].color = imageDefaultColor;
            controllerImages[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < halfControllerImages.Count; i++)
        {
            halfControllerImages[i].color = imageDefaultColor;
            halfControllerImages[i].gameObject.SetActive(false);
        }

        for (int controllerImageIndex = 0; controllerImageIndex < controllers.Count; controllerImageIndex++)
        {
            ControllerInputHandler controller = controllers[controllerImageIndex];
            if (controller.splitControls)
            {
                int playerAIndex = controllerImageIndex * 2;
                int playerBIndex = playerAIndex + 1;

                controllerImages[controllerImageIndex].gameObject.SetActive(false);
                halfControllerImages[playerAIndex].gameObject.SetActive(true);
                halfControllerImages[playerBIndex].gameObject.SetActive(true);
                halfControllerImages[playerAIndex].color = controller.playerA.color;
                halfControllerImages[playerBIndex].color = controller.playerB.color;
            }
            else
            {
                controllerImages[controllerImageIndex].color = controller.playerA.color;
            }
        }


      
    }

    private void UpdateShieldText()
    {
        if (shieldText.Count < alivePlayers.Count) {
            Vector3 nextPos = Vector3.zero;

            for (int i = 0; i < alivePlayers.Count; i++) {
                TextMeshProUGUI proUGUI;
                nextPos.y = i * -50;
                Instantiate(shieldTextObj, nextPos, Quaternion.identity).TryGetComponent(out proUGUI);
                if (proUGUI != null) {
                    proUGUI.transform.SetParent(shieldTextParent, false);
                    shieldText.Add(proUGUI);
                }
            }
        }

        for (int i = 0; i < shieldText.Count; i++) {
            if (i >= alivePlayers.Count) {
                shieldText[i].gameObject.SetActive(false);
            } else {
                shieldText[i].gameObject.SetActive(true);
                shieldText[i].text = i.ToString() + ": " + alivePlayers[i].shieldHealth.ToString();
            }
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
        if (player.shieldHealth <= 0) {
            EliminatePlayer(player);
            return true;
        } else {
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

        float[] ballsStartAngles = new float[balls.Count];
        float[] ballsStartDistances = new float[balls.Count];

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

        for (int i = 0; i < balls.Count; i++) {
            ballsStartAngles[i] = Player.Angle(balls[i].transform.position);
            ballsStartDistances[i] = balls[i].transform.position.magnitude;
            balls[i].collider.enabled = false;
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

                pillars[i].transform.position = GetTargetPointInCircle(targetAngle);
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

            for (int i = 0; i < balls.Count; i++) {
                Vector3 position = GetTargetPointInCircle(ballsStartAngles[i] + playerElimBallSpinSpeed * Mathf.Lerp(0, pillarSmashTimer, playerRemovalPercentage)).normalized;
                position *= Mathf.Lerp(ballsStartDistances[i], 0.0f, playerRemovalPercentage);
                balls[i].transform.position = position;
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(pillars[index]);
        pillars.RemoveAt(index);

        // ensure each pillar is exactly where it was calculated to belong
        for (int i = 0; i < pillars.Count; i++) {
            float targetAngle = 360.0f / (pillars.Count) * i;

            pillars[i].transform.position = GetTargetPointInCircle(targetAngle);
            pillars[i].transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }

        elimPlayers.Add(alivePlayers[index]);
        alivePlayers[index].gameObject.SetActive(false);
        alivePlayers.RemoveAt(index);

        UpdateAlivePlayers();

        UpdateShieldText();

        arcTanShaderHelper.colors = GenerateLivingColors();
        arcTanShaderHelper.CreateTexture();
        arcTanShaderHelper.SetShrink(0.0f);

        CleanTransformers();
        ResetBalls();

        smashingPillars = false;

        if (alivePlayers.Count <= 1) {
            EndGame();
        }
        yield break;
    }
    #endregion
    #endregion
}
