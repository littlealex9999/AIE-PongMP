using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Image = UnityEngine.UI.Image;
using Color = UnityEngine.Color;
using System.Drawing;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager instance;
    #region Game Objects
    [Header("Game Objects")]

    public PPController postEffectsController;
    public Ball ballPrefab;
    [HideInInspector] public List<Ball> balls = new List<Ball>();

    public GameObject pillarPrefab;
    public GameObject playerPrefab;
    [HideInInspector] public  List<GameObject> pillars = new List<GameObject>();

    public GameObject healthDotPrefab;

    public List<GameVariables> gameVariables;
    public GameVariables selectedGameVariables;

    public ArcTanShaderHelper arcTanShaderHelper;

    [HideInInspector] public List<ControllerInputHandler> controllers;
    #endregion

    #region Map Settings
    [Header("Map Settings")]
    public float mapRadius = 4.5f;
    public AnimationCurve elimSpeedCurve;
    public float playerElimTime = 2.0f;
    public float playerElimBallSpinSpeed = 2.0f;
    [Range(0, 1)]
    public float healthBlipSpread = 0.1f;
    public float healthBlipSquishTime = 0.5f;
    public float healthBlipDistance = 4.6f;
    public Vector3 healthBlipOffset;

    [Range(0, 360)]
    public float mapRotationOffset = 0.0f;
    public float playerDistance = 4.0f;
    public float playerBlinkOnHitDuration;

    [ColorUsage(true, true), SerializeField]
    List<Color> playerEmissives = new List<Color>();
    [SerializeField] List<ParticleSystem.MinMaxGradient> particleColors;

    [SerializeField] List<Texture> playerShapeTextures = new();
    [SerializeField] List<Sprite> playerShapeGlowSprites = new();
    [SerializeField] List<Sprite> playerShapeLineSprites = new();
    #endregion

    #region Transformers
    [Header("Transformers")]
    public List<Transformer> transformers = new List<Transformer>();
    List<Transformer> allowedTransformers = new List<Transformer>();
    public float transformerSpawnRadius = 2.0f;
    float transformerSpawnTimer;
    [HideInInspector] public List<Transformer> spawnedTransformers = new List<Transformer>();
    [HideInInspector] public List<Transformer> activeTransformers = new List<Transformer>();
    #endregion

    #region UI
    [Header("UI")]
    public List<Image> controllerImages;
    public List<Image> halfControllerImages;

    public List<Image> fullPlayerShapeGlow;
    public List<Image> fullPlayerShapeLine;

    public List<Image> halfPlayerShapeGlow;
    public List<Image> halfPlayerShapeLine;
    public Color imageDefaultColor;

    // The expected structure is that the image will have a sibling image relevant to it
    // so it should be treated as if it always has a parent
    [Space]
    public List<Image> endGamePlayerImages = new List<Image>();
    public List<Image> endGameShapeGlowImages = new();
    public List<Image> endGameShapeLineImages = new();
    public List<int> endGamePlayerEnableThresholds = new List<int>();
    public Transform endGameAlternatePosition;
    Vector3 endGameRestorePosition;

    [Space]
    public List<Image> playtimeControlsUI;
    public float playtimeControlsUIDisappearTimer;
    float playtimeControlsUItimer;
    public AnimationCurve playtimeControlsUICurve;

    [Space]
    public GameObject loadingScreen;
    public float loadingScreenDuration = 5.0f;

    public delegate void GameStateChange();
    public GameStateChange OnGameStateChange;

    [HideInInspector] public GameState gameState = GameState.MAINMENU;

    public enum GameState
    {
        MAINMENU,
        JOINMENU,
        PRESETSELECT,
        EDITPRESET,
        GAMEPLAY,
        GAMEPAUSED,
        GAMEOVER,
        CREDITS,
    }
    #endregion

    #region Pause
    bool inGame;
    public bool holdGameplay { get { return smashingPillars || countdownTimer > 0; } }
    [HideInInspector] public bool smashingPillars = false;

    float countdownTime = 3.0f;
    [HideInInspector] public float countdownTimer = 0.0f;
    #endregion

    #region Gameplay Settings
    float gameEndTimer;
    bool loading;
    [HideInInspector] public List<Player> players;
    [HideInInspector] public List<Player> alivePlayers;
    [HideInInspector] public List<Player> elimPlayers;
    [HideInInspector] public Dictionary<int, bool> takenColors;

    [HideInInspector] public BlackHole blackHole;
    #endregion

    #region Haptics
    [Header("Haptics")]
    public ControllerHaptics playerJoinHaptics;
    public ControllerHaptics shieldTouchHaptics;
    public ControllerHaptics paddleBounceHaptics;
    public ControllerHaptics playerElimHaptics;
    #endregion

    #region Extra Settings
    public bool enableHaptics = true;
    public bool enableScreenShake = true;
    public bool skipControlScreen;
    public Animator inGameUI;
    #endregion
    #endregion

    #region Functions
    #region Unity
    void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);

        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
        selectedGameVariables = ScriptableObject.CreateInstance<GameVariables>();

#if !UNITY_EDITOR
        UnityEngine.Cursor.visible = false;
#endif

        OnGameStateChange += OnGameStateChanged;

        SetupGameEndData();
        UpdateGameState(GameState.MAINMENU);
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.GAMEPLAY:
                if (loading) break;
                if (!holdGameplay)
                {
                    gameEndTimer -= Time.deltaTime;
                    if (selectedGameVariables.useTimer && gameEndTimer <= 0) 
                    {
                        // UpdateGameState(GameState.GAMEOVER);
                    }

                    TransformerUpdate(Time.deltaTime);
                    PlaytimeUIUpdate(Time.deltaTime);
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
    public int GetUnusedColorIndex()
    {
        for (int i = 0; i < playerEmissives.Count; i++) {
            bool passed = true;

            for (int j = 0; j < players.Count; j++) {
                if (playerEmissives[i] == players[j].color) {
                    passed = false;
                    break;
                }
            }

            if (passed) {
                return i;
            }
        }

#if UNITY_EDITOR
        Debug.LogError("Failed to find an unused color");
#endif

        return -1;
    }

    public ParticleSystem.MinMaxGradient GetPlayerParticleColor(int index)
    {
        if (index < particleColors.Count) return particleColors[index];
        else return particleColors[particleColors.Count - 1];
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
    #endregion

    #region GAMESTATE
    public void UpdateGameState(GameState state)
    {
        if (gameState == GameState.JOINMENU && state == GameState.PRESETSELECT && players.Count < 2) return;
        gameState = state;
        OnGameStateChange.Invoke();
    }

    private void OnGameStateChanged()
    {
        switch (gameState) {
            case GameState.MAINMENU:
                postEffectsController.DisableBloom();
                EventManager.instance?.menuEvent.Invoke();
                break;
            case GameState.JOINMENU:
                postEffectsController.DisableBloom();
                EventManager.instance?.menuEvent.Invoke();
                break;
            case GameState.PRESETSELECT:
                postEffectsController.DisableBloom();
                EventManager.instance?.menuEvent.Invoke();
                break;
            case GameState.EDITPRESET:
                postEffectsController.DisableBloom();
                EventManager.instance?.menuEvent.Invoke();
                break;
            case GameState.GAMEPLAY:
                StartCoroutine(DoLoadScreen());
                break;
            case GameState.GAMEPAUSED:
                EventManager.instance?.menuEvent.Invoke();

                break;
            case GameState.GAMEOVER:
                postEffectsController.DisableBloom();
                if (inGame) {
                    // EndGame calls a gamestate change to GAMEOVER so we must ensure it doesn't infinitely repeat and return
                    EndGame();
                    return;
                }

                EventManager.instance?.menuEvent?.Invoke();

                for (int i = 0; i < elimPlayers.Count && i < endGamePlayerImages.Count; i++)
                {
                    Player player = elimPlayers[elimPlayers.Count - (i + 1)];
                    endGamePlayerImages[i].color = player.color;
                    endGameShapeGlowImages[i].sprite = playerShapeGlowSprites[player.ID];
                    endGameShapeLineImages[i].sprite = playerShapeLineSprites[player.ID];
                    endGameShapeGlowImages[i].color = player.color;
                    endGameShapeLineImages[i].color = Color.white;

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

 
    IEnumerator DoLoadScreen()
    {
        float timer = loadingScreenDuration;

        loading = true;

        while (timer > 0) 
        {
            timer -= Time.deltaTime;

            if (skipControlScreen) break;

            yield return new WaitForEndOfFrame();
        }

        skipControlScreen = false;
        loading = false;
        inGameUI.SetTrigger("EndControlScreen");

        postEffectsController.EnableBloom();
        EventManager.instance?.gameplayEvent?.Invoke();

        if (!inGame) {
            StartGame();
        } else {
            ResetShieldDisplay();
        }

        yield break;
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

        player.colorIndex = GetUnusedColorIndex();
        player.SetupPlayer(playerEmissives[player.colorIndex], particleColors[player.colorIndex]);

        players.Add(player);

        MenuManager.instance.CheckPlayerCount();

        return player;
    }

    public void RemovePlayer(Player playerToRemove)
    {
        if (playerToRemove == null) return;
        EventManager.instance.playerLeaveEvent.Invoke();

        players.Remove(playerToRemove);
        Destroy(playerToRemove);

        MenuManager.instance.CheckPlayerCount();

        UpdatePlayerImages();
    }

    public void EliminatePlayer(Player player)
    {
        for (int i = 0; i < player.healthBlips.Count; i++)
        {
            Destroy(player.healthBlips[i]);
        }

        player.healthBlips.Clear();
        player.dead = true;
        int index = alivePlayers.IndexOf(player);
        foreach (Player p in players)
        {
            p.grabParticles.gameObject.SetActive(false);
        }
        StartCoroutine(EliminatePlayerRoutine(index));
    }


    public void PlayChromaticAberration()
    {
        StartCoroutine(postEffectsController.chromaticAberrationIntensity.CR_Play());
    }
    public void BlinkPlayerSegment(int aliveID)
    {
        StartCoroutine(CR_BlinkPlayerSegment(aliveID));
    }
    private IEnumerator CR_BlinkPlayerSegment(int aliveID)
    {
        Color[] colors = new Color[alivePlayers.Count];
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            if (i == aliveID) colors[i] = Color.white;
            else colors[i] = alivePlayers[i].color;

        }

        arcTanShaderHelper.colors = colors;
        arcTanShaderHelper.CreateTexture();
        arcTanShaderHelper.SetShrink(0.0f);

        yield return new WaitForSeconds(playerBlinkOnHitDuration);

        arcTanShaderHelper.colors = GenerateLivingColors();
        arcTanShaderHelper.CreateTexture();
        arcTanShaderHelper.SetShrink(0.0f);

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
        ResetShieldDisplay();
        SetupTransformers();
    }

    void EndGame()
    {
        inGame = false;
        for (int i = 0; i < players.Count; i++) {
            players[i].gameObject.SetActive(false);
            for (int j = 0; j < players[i].healthBlips.Count; j++) {
                // health blips should be managed as part of the shield reset, but they managed to persist through that
                Destroy(players[i].healthBlips[j]);
            }
            players[i].healthBlips.Clear();
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
            player.ResetStartValues();

            player.moveSpeed = selectedGameVariables.playerSpeed;

            player.rotationalForce = selectedGameVariables.playerRotationalForce;
            player.collider.normalBending = selectedGameVariables.playerNormalBending;

            player.dashDistance = selectedGameVariables.dashDistance;
            player.dashDuration = selectedGameVariables.dashDuration;
            player.dashCooldown = selectedGameVariables.dashCooldown;
            player.dashEnabled = selectedGameVariables.dashEnabled;

            player.hitCooldown = selectedGameVariables.hitCooldown;
            player.hitDuration = selectedGameVariables.hitDuration;
            player.hitStrength = selectedGameVariables.hitStrength;

            player.grabCooldown = selectedGameVariables.grabCooldown;
            player.grabDuration = selectedGameVariables.grabDuration;

            player.shieldHealth = selectedGameVariables.shieldLives;

            player.meshRenderer.material = new(player.meshRenderer.material)
            {
                mainTexture = playerShapeTextures[i]
            };
            player.meshRenderer.material.SetColor("_EmissiveColor", player.color);

            player.dead = false;

            alivePlayers.Add(player);
        }
        UpdateAlivePlayers();
    }

    void UpdateAlivePlayers()
    {
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            Player player = alivePlayers[i];

            player.shieldHealth = selectedGameVariables.shieldLives;

            if (alivePlayers.Count > 1) {
                // area limits are calculated with a resize
                player.Resize(selectedGameVariables.playerSizes[alivePlayers.Count - 2]);
            } else {
                player.CalculateLimits();
            }

            player.SetPosition(player.playerSectionMiddle);
        }

        EventManager.instance.UpdateMusicPitch();
    }

    void SetupBalls()
    {
        EventManager.instance.ballCountdownEvent.Invoke();

        for (int i = 0; i < selectedGameVariables.ballCount; i++) {
            Ball b = Instantiate(ballPrefab);

            b.transform.position = Vector2.zero;
            b.constantSpd = selectedGameVariables.ballSpeed;
            b.transform.localScale = new Vector3(selectedGameVariables.ballSize, selectedGameVariables.ballSize, selectedGameVariables.ballSize);
            b.collider.radius = selectedGameVariables.ballSize / 2;
            b.dampStrength = selectedGameVariables.ballSpeedDamp;
            b.shieldBounceTowardsCenterBias = selectedGameVariables.shieldBounceTowardsCenterBias;
            balls.Add(b);
        }
    }

    void ResetBalls()
    {
        if (alivePlayers.Count > 1) EventManager.instance.ballCountdownEvent.Invoke();
        countdownTimer = countdownTime;

        for (int i = balls.Count - 1; i > selectedGameVariables.ballCount; i--) {
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
                GetTargetPointInCircle(360.0f / players.Count * i) * mapRadius,
                Quaternion.Euler(0, 0, 360.0f / players.Count * i));
        }
    }

    public Vector3 GetTargetPointInCircle(float angle)
    {
        return Vector3.zero + Quaternion.Euler(0, 0, angle) * Vector3.up;
    }

    public void SetupMap()
    {
        arcTanShaderHelper.SetShrink(0);
        arcTanShaderHelper.colors = new Color[alivePlayers.Count];
        for (int i = 0; i < alivePlayers.Count; i++) {
            arcTanShaderHelper.colors[i] = alivePlayers[i].color;
        }
        arcTanShaderHelper.CreateTexture();

        playtimeControlsUItimer = 0;
        for (int i = 0; i < playtimeControlsUI.Count; i++) {
            playtimeControlsUI[i].color = new Color(playtimeControlsUI[i].color.r, playtimeControlsUI[i].color.g, playtimeControlsUI[i].color.b, 1);
        }
    }

    void SetupTransformers()
    {
        allowedTransformers.Clear();

        for (int i = 0; i < transformers.Count; i++) {
            if ((transformers[i].GetTransformerType() & selectedGameVariables.enabledTransformers) != 0) {
                allowedTransformers.Add(transformers[i]);
            }
        }

        CleanTransformers();
    }

    public void CleanTransformers(bool clearBlackHole = true)
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

        if (clearBlackHole && blackHole) {
            Destroy(blackHole.gameObject);
            blackHole = null;
        }
    }
    #endregion

    #region GAMEPLAY
    void TransformerUpdate(float delta)
    {
        transformerSpawnTimer += delta;

        if (transformerSpawnTimer > selectedGameVariables.transformerSpawnTime) {
            if (Random.Range(0, 1) < selectedGameVariables.transformerFrequency) {
                SpawnTransformer();
            }
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

        if (float.IsNaN(ret.x) || float.IsNaN(ret.y)) {
            return new Vector2(0, 0);
        }

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

    void PlaytimeUIUpdate(float delta)
    {
        if (playtimeControlsUItimer < playtimeControlsUIDisappearTimer) {
            playtimeControlsUItimer += delta;
            float disappearVal = playtimeControlsUICurve.Evaluate(playtimeControlsUItimer / playtimeControlsUIDisappearTimer);
            for (int i = 0; i < playtimeControlsUI.Count; i++) {
                playtimeControlsUI[i].color = new Color(playtimeControlsUI[i].color.r, playtimeControlsUI[i].color.g, playtimeControlsUI[i].color.b, disappearVal);
            }
        }
    }

    public void UpdatePlayerImages()
    {
        for (int i = 0; i < controllerImages.Count; i++)
        {
            controllerImages[i].color = imageDefaultColor;
            controllerImages[i].gameObject.SetActive(true);
            fullPlayerShapeGlow[i].sprite = null;
            fullPlayerShapeLine[i].sprite = null;
            fullPlayerShapeGlow[i].color = Color.clear;
            fullPlayerShapeLine[i].color = Color.clear;
        }

        for (int i = 0; i < halfControllerImages.Count; i++)
        {
            halfControllerImages[i].color = imageDefaultColor;
            halfControllerImages[i].gameObject.SetActive(false);
            halfPlayerShapeGlow[i].sprite = null;
            halfPlayerShapeLine[i].sprite = null;
            halfPlayerShapeGlow[i].color = Color.clear;
            halfPlayerShapeLine[i].color = Color.clear;
        }

        for (int controllerImageIndex = 0; controllerImageIndex < controllers.Count; controllerImageIndex++)
        {
            ControllerInputHandler controller = controllers[controllerImageIndex];
            int playerAIndex = controllerImageIndex * 2;
            int playerBIndex = playerAIndex + 1;
            if (controller.splitControls)
            {
                controllerImages[controllerImageIndex].gameObject.SetActive(false);
                halfControllerImages[playerAIndex].gameObject.SetActive(true);
                halfControllerImages[playerBIndex].gameObject.SetActive(true);

                halfPlayerShapeGlow[playerAIndex].sprite = playerShapeGlowSprites[controller.playerA.ID];
                halfPlayerShapeLine[playerAIndex].sprite = playerShapeLineSprites[controller.playerA.ID];

                halfPlayerShapeGlow[playerBIndex].sprite = playerShapeGlowSprites[controller.playerB.ID];
                halfPlayerShapeLine[playerBIndex].sprite = playerShapeLineSprites[controller.playerB.ID];

                halfControllerImages[playerAIndex].color = controller.leftBumperDown ? Color.white : controller.playerA.color;
                halfPlayerShapeGlow[playerAIndex].color = controller.leftBumperDown ? Color.white : controller.playerA.color;

                halfControllerImages[playerBIndex].color = controller.rightBumperDown ? Color.white : controller.playerB.color;
                halfPlayerShapeGlow[playerBIndex].color = controller.rightBumperDown ?  Color.white : controller.playerB.color;

                halfPlayerShapeLine[playerAIndex].color = Color.white;
                halfPlayerShapeLine[playerBIndex].color = Color.white;
            }
            else
            {
                fullPlayerShapeGlow[controllerImageIndex].sprite = playerShapeGlowSprites[controller.playerA.ID];
                fullPlayerShapeLine[controllerImageIndex].sprite = playerShapeLineSprites[controller.playerA.ID];

                controllerImages[controllerImageIndex].color = (controller.rightBumperDown || controller.leftBumperDown) ? Color.white : controller.playerA.color;
                fullPlayerShapeGlow[controllerImageIndex].color = (controller.rightBumperDown || controller.leftBumperDown) ? Color.white : controller.playerA.color;

                fullPlayerShapeLine[controllerImageIndex].color = Color.white;
            }
        }
    }

    public void BlinkController(ControllerInputHandler controller)
    {
        int controllerID = controllers.IndexOf(controller);
        int playerAIndex = controllerID * 2;
        int playerBIndex = playerAIndex + 1;
        if (controller.splitControls)
        {
            halfControllerImages[playerAIndex].color = Color.white;
            halfControllerImages[playerBIndex].color = Color.white;

            halfPlayerShapeGlow[playerAIndex].color = Color.white;
            halfPlayerShapeLine[playerAIndex].color = Color.white;

            halfPlayerShapeGlow[playerBIndex].color = Color.white;
            halfPlayerShapeLine[playerBIndex].color = Color.white;
        }
        else
        {
            controllerImages[controllerID].color = Color.white;

            fullPlayerShapeGlow[controllerID].color = Color.white;
            fullPlayerShapeLine[controllerID].color = Color.white;
        }
    }

    private void ResetShieldDisplay()
    {
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            float angleChange = alivePlayers[i].playerAngleDeviance * healthBlipSpread / (alivePlayers[i].shieldHealth + 1) * 2;
            float angle = alivePlayers[i].playerSectionMiddle - alivePlayers[i].playerAngleDeviance * healthBlipSpread + angleChange;

            for (int j = 0; j < alivePlayers[i].shieldHealth; j++)
            {
                if (alivePlayers[i].healthBlips.Count <= j) {
                    alivePlayers[i].healthBlips.Add(Instantiate(healthDotPrefab));
                    alivePlayers[i].healthBlips[alivePlayers[i].healthBlips.Count - 1].name = "Player " + i + " Blip " + j;
                }
                alivePlayers[i].healthBlips[j].transform.position = GetTargetPointInCircle(angle) * healthBlipDistance + healthBlipOffset;
                angle += angleChange;
            }

            for (int j = alivePlayers[i].healthBlips.Count - 1; j >= alivePlayers[i].shieldHealth; j--)
            {
                Destroy(alivePlayers[i].healthBlips[j]);
                alivePlayers[i].healthBlips.RemoveAt(j);
            }
        }
    }

    IEnumerator SquishHealthBlips(Player player)
    {
        float timer = 0.0f;
        int targetDestroyBlip = (int)(player.healthBlips.Count / 2.0f - 1);

        if (player.healthBlips.Count <= 1) {
            ResetShieldDisplay();
            yield break;
        }

        while (timer < healthBlipSquishTime) {
            timer += Time.deltaTime;
            float removalPercentage = timer / healthBlipSquishTime;

            float angleChange = player.playerAngleDeviance * healthBlipSpread / (player.healthBlips.Count - removalPercentage + 1) * 2;
            float angle = player.playerSectionMiddle - player.playerAngleDeviance * healthBlipSpread + angleChange;
            for (int i = 0; i < player.healthBlips.Count; i++) {
                player.healthBlips[i].transform.position = GetTargetPointInCircle(angle) * healthBlipDistance + healthBlipOffset;
                angle += angleChange;

                if (i == targetDestroyBlip) {
                    angleChange *= -1;
                    angle = player.playerSectionMiddle + player.playerAngleDeviance * healthBlipSpread + angleChange;
                }
            }


            yield return new WaitForEndOfFrame();
        }

        ResetShieldDisplay();

        yield return null;
    }

    /// <summary>
    /// Manages shield health and player elimination. Returns true if a hit causes a player to die.
    /// </summary>
    /// <param name="alivePlayerID"></param>
    /// <returns></returns>
    public bool OnShieldHit(int alivePlayerID)
    {
        if (alivePlayerID >= alivePlayers.Count) return false;

        Player player = alivePlayers[alivePlayerID];
        if (player.shieldHealth <= 1)
        {
            ResetShieldDisplay();
            EliminatePlayer(player);
            return true;
        } else 
        {
            player.shieldHealth--;
            if (player.shieldHealth <= 0) EventManager.instance?.shieldBreakEvent?.Invoke();
            else EventManager.instance?.shieldHitEvent?.Invoke();
            if (!player.isAI) player.controllerHandler.SetHaptics(shieldTouchHaptics);
            StartCoroutine(SquishHealthBlips(player));
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

        EventManager.instance?.playerEliminatedEvent?.Invoke();
        EventManager.instance?.towerMoveEvent?.Invoke();

        index %= pillars.Count;
        arcTanShaderHelper.SetTargetPlayer(index);

        float pillarSmashTimer = 0.0f;

        float[] playerStartAngles = new float[alivePlayers.Count];
        float[] playerTargetAngles = new float[alivePlayers.Count];

        float[] ballsStartAngles = new float[balls.Count];
        float[] ballsStartDistances = new float[balls.Count];

        int[] healthBlipsToAdd = new int[alivePlayers.Count];
        int[] healthBlipsAdded = new int[alivePlayers.Count];

        Vector3 elimPlayerStartScale = alivePlayers[index].transform.localScale;

        // calculate start and end angle for each player
        for (int i = 0; i < alivePlayers.Count; i++) {
            playerStartAngles[i] = Player.Angle(alivePlayers[i].transform.position);
            int targetPlayerIndex = i;
            if (i > index) --targetPlayerIndex;

            if (i == index) {
                // player being eliminated
                playerTargetAngles[i] = 360.0f / (alivePlayers.Count - 1) * targetPlayerIndex;

                // ensure elim player has exactly 1 health blip
                for (int j = 0; j < alivePlayers[i].healthBlips.Count; j++) {
                    Destroy(alivePlayers[i].healthBlips[j]);
                }
                alivePlayers[i].healthBlips.Clear();

                alivePlayers[i].healthBlips.Add(Instantiate(healthDotPrefab));

                healthBlipsToAdd[i] = 0;
            } else {
                playerTargetAngles[i] = 180.0f / (alivePlayers.Count - 1) + 360.0f / (alivePlayers.Count - 1) * targetPlayerIndex;
                healthBlipsToAdd[i] = selectedGameVariables.shieldLives - alivePlayers[i].healthBlips.Count;
            }
        }

        for (int i = 0; i < balls.Count; i++) {
            ballsStartAngles[i] = Player.Angle(balls[i].transform.position);
            ballsStartDistances[i] = balls[i].transform.position.magnitude;
            balls[i].collider.enabled = false;
        }

        // set haptics
        playerElimHaptics.duration = playerElimTime;
        if (!alivePlayers[index].isAI) alivePlayers[index].controllerHandler.SetHaptics(playerElimHaptics);

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

                pillars[i].transform.position = GetTargetPointInCircle(targetAngle) * mapRadius;
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
                
                // health blips
                float deviance = 180.0f / pseudoPlayerCount;
                float healthBlipInterval = 1.0f / (healthBlipsToAdd[i] + 1);
                float healthBlipIntervalPremultiplied = healthBlipInterval * (healthBlipsAdded[i] + 1);

                float targetMidsection;
                if (i > index) {
                    deviance *= -1;
                    targetMidsection = 360.0f - 360.0f / pseudoPlayerCount * (alivePlayers.Count - 1 - i) + deviance;
                } else if (i == index) {
                    deviance = 0;
                    targetMidsection = 360.0f / pseudoPlayerCount * i + 360.0f / pseudoPlayerCount * ((1 - playerRemovalPercentage) / 2);
                } else {
                    targetMidsection = 360.0f / pseudoPlayerCount * i + deviance;
                }
                float angleChange = deviance * healthBlipSpread / (alivePlayers[i].healthBlips.Count - healthBlipsAdded[i] + Mathf.Lerp(0, healthBlipsToAdd[i], playerRemovalPercentage) + 1) * 2;
                float healthAngle = targetMidsection - deviance * healthBlipSpread + angleChange;
                for (int j = 0; j < alivePlayers[i].healthBlips.Count; j++) {
                    alivePlayers[i].healthBlips[j].transform.position = GetTargetPointInCircle(healthAngle) * healthBlipDistance + healthBlipOffset;

                    if (healthBlipsAdded[i] > 0 && j == alivePlayers[i].healthBlips.Count - 2) {
                        healthAngle += Mathf.Lerp(0, angleChange, playerRemovalPercentage % healthBlipInterval / healthBlipInterval);
                    } else {
                        healthAngle += angleChange;
                    }
                }

                if (healthBlipsToAdd[i] > 0 && healthBlipsAdded[i] < healthBlipsToAdd[i]) {
                    if (healthBlipIntervalPremultiplied <= playerRemovalPercentage) {
                        alivePlayers[i].healthBlips.Add(Instantiate(healthDotPrefab, alivePlayers[i].healthBlips[alivePlayers[i].healthBlips.Count - 1].transform.position, Quaternion.identity));
                        healthBlipsAdded[i]++;
                    }
                }
            }

            for (int i = 0; i < balls.Count; i++) {
                Vector3 position = GetTargetPointInCircle(ballsStartAngles[i] + playerElimBallSpinSpeed * Mathf.Lerp(0, pillarSmashTimer, playerRemovalPercentage)).normalized;
                position *= Mathf.Lerp(ballsStartDistances[i], 0.0f, playerRemovalPercentage);
                balls[i].transform.position = position;
                balls[i].transform.rotation = Quaternion.Euler(0, 0, Player.Angle(balls[i].transform.position) + 90);
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(pillars[index]);
        pillars.RemoveAt(index);

        // ensure each pillar is exactly where it was calculated to belong
        for (int i = 0; i < pillars.Count; i++) {
            float targetAngle = 360.0f / (pillars.Count) * i;

            pillars[i].transform.position = GetTargetPointInCircle(targetAngle) * mapRadius;
            pillars[i].transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }

        for (int i = 0; i < players.Count; i++) {
            for (int j = 0; j < players[i].healthBlips.Count; j++) {
                // health blips are immortal somehow. this forcibly destroys them all and replaces them
                Destroy(players[i].healthBlips[j]);
            }

            players[i].healthBlips.Clear();
        }

        elimPlayers.Add(alivePlayers[index]);
        alivePlayers[index].gameObject.SetActive(false);
        alivePlayers.RemoveAt(index);

        UpdateAlivePlayers();

        ResetShieldDisplay();

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
