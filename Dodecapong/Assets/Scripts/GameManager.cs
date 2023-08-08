using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GameManager;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MAINMENU,
        JOINMENU,
        SETTINGSMENU,
        GAMEPLAY,
        GAMEPAUSED,
        GAMEOVER,
    }
    public UnityEvent gameStateChanged;

    public GameState gameState { get; private set; }

    public void UpdateGameState(GameState state)
    {
        gameState = state;
        gameStateChanged.Invoke();
    }

    [Serializable]
    public class Player
    {
        public int id;
        public int shieldHealth;
        public Paddle paddle;
        public Color color;

        ~Player()
        {
            if (paddle) Destroy(paddle.gameObject);
        }
    }

    public List<Player> alivePlayers;
    public List<Player> players;
    public int alivePlayerCount { get { return alivePlayers.Count; } }

    List<GameObject> pillars = new List<GameObject>();

    public static GameManager instance;

    public Map map;
    public Ball ball;

    public GameObject paddleObject;
    public GameObject pillarObject;

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
        switch (gameState) {
            case GameState.GAMEPLAY:
                gameEndTimer -= Time.deltaTime;
                if (gameEndTimer <= 0) {
                    // end game
                }
                break;
            default:
                break;
        }
    }

    void OnGameStateChanged()
    {
        switch (gameState)
        {
            case GameState.MAINMENU:

                break;
            case GameState.JOINMENU:
                
                break;
            case GameState.SETTINGSMENU:

                break;
            case GameState.GAMEPLAY:
                ResetPlayers();

                GeneratePaddles();
                BuildGameBoard();
                break;
            case GameState.GAMEPAUSED:

                break;
            case GameState.GAMEOVER:

                break;
        }
    }

    public Player AddPlayer()
    {
        Player player = new Player();
        player.id = players.Count;
        player.shieldHealth = gameVariables.shieldLives;
        player.color = playerEmissives[players.Count];
        players.Add(player);
        alivePlayers.Add(player);
        UpdatePlayerImages();
        return player;
    }

    public void ResetPlayers()
    {
        alivePlayers.Clear();

        for (int i = 0; i < players.Count; i++) {
            players[i].shieldHealth = gameVariables.shieldLives;
            alivePlayers.Add(players[i]);
        }

        UpdatePlayerImages();
    }

    public void RemovePlayer(Player playerToRemove)
    {
        players.Remove(playerToRemove);
        alivePlayers.Remove(playerToRemove);
        UpdatePlayerImages();
    }

    void UpdatePlayerImages()
    {
        foreach (Image image in playerImages) image.color = Color.white;
        for (int i = 0; i < players.Count; i++)
        {
            playerImages[i].color = players[i].color;
        }
    }

    void GeneratePaddles()
    {
        foreach (Player player in alivePlayers)
        {
            Paddle paddle;
            if (player.paddle == null) player.paddle = paddle = Instantiate(paddleObject, map.transform).GetComponent<Paddle>();
            else paddle = player.paddle;

            // changing the order of player size variables for easier readability in the inspector.
            // x correlates to what the camera sees as horizontal length, while y correlates to vertical width
            // though the actual scaling is different
            paddle.transform.localScale = new Vector3(gameVariables.playerSize.y, gameVariables.playerSize.x, gameVariables.playerSize.y);

            // the "starting position" is as follows, with 2 players as an example:
            // 360 / player count to get the base angle (360 / 2 = 180)
            // ... * i + 1 to get a multiple of the base angle based on the player (180 * (0 + 1) = 180)
            // ... + mapRotationOffset to ensure the paddles spawn relative to the way the map is rotated (+ 0 in example, so ignored)
            // 360 / (playerCount * 2) to get the offset of the middle of each player area (360 / (2 * 2) = 90)
            // (player position - segment offset) to get the correct position to place the player (180 - 90 = 90)
            paddle.Initialise(player.id, playerDistance, 360.0f / alivePlayerCount * (player.id + 1) + mapRotationOffset - 360.0f / (alivePlayerCount * 2), 180.0f / alivePlayerCount);
            paddle.moveSpeed = gameVariables.playerSpeed;

            paddle.SetColor(player.color);
            paddle.name = "Player " + player.id;
        }
    }

    void BuildGameBoard()
    {
        ball.transform.position = map.transform.position;
        ball.constantVel = gameVariables.ballSpeed;
        ball.transform.localScale = new Vector3(gameVariables.ballSize, gameVariables.ballSize, gameVariables.ballSize);
        gameEndTimer = gameVariables.timeInSeconds;

        map.GenerateMap();
        UpdateShields();
    }

    private void UpdateShields()
    {
        if (shieldText.Count == 0)
        {
            for (int i = 0; i < alivePlayers.Count; i++)
            {
                TextMeshProUGUI proUGUI;
                Instantiate(shieldTextObj, shieldTextParent).TryGetComponent(out proUGUI);
                if (proUGUI != null)
                {
                    proUGUI.text = alivePlayers[i].shieldHealth.ToString();
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
        if (alivePlayerID >= alivePlayerCount) return false;

        Player player = alivePlayers[alivePlayerID];
        if (player.shieldHealth <= 0)
        {
            alivePlayers.Remove(player);
            if (alivePlayers.Count == 1)
            {
                UpdateGameState(GameState.GAMEOVER);
            }
            BuildGameBoard();
            return true;
        }
        else
        {
            player.shieldHealth--;
            UpdateShields();
            return false;
        }
    }
}
