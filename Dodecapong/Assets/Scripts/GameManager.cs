using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
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
        SCOREBOARD,
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

    public static GameManager instance;

    public Map map;
    public Ball ball;

    public GameObject paddleObject;

    [Range(0, 360)] public float mapRotationOffset = 0.0f;

    [ColorUsage(true, true), SerializeField] List<Color> playerEmissives = new List<Color>();

    public float playerDistance = 4.0f;
    [Min(0)] public int shieldHits = 1;

    public List<Image> playerImages;

    void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);

        if (gameStateChanged == null) gameStateChanged = new UnityEvent();

        gameStateChanged.AddListener(OnGameStateChanged);

        UpdateGameState(GameState.MAINMENU);
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

                GeneratePaddles();
                BuildGameBoard();
                break;
            case GameState.GAMEPAUSED:

                break;
            case GameState.SCOREBOARD:

                break;
        }
    }

    public Player AddPlayer()
    {
        Player player = new Player();
        player.id = players.Count;
        player.shieldHealth = shieldHits;
        player.color = playerEmissives[players.Count];
        players.Add(player);
        alivePlayers.Add(player);
        UpdatePlayerImages();
        return player;
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
        foreach (Player player in players)
        {
            Paddle paddle = Instantiate(paddleObject, map.transform).GetComponent<Paddle>();
            player.paddle = paddle;

            // the "starting position" is as follows, with 2 players as an example:
            // 360 / player count to get the base angle (360 / 2 = 180)
            // ... * i + 1 to get a multiple of the base angle based on the player (180 * (0 + 1) = 180)
            // ... + mapRotationOffset to ensure the paddles spawn relative to the way the map is rotated (+ 0 in example, so ignored)
            // 360 / (playerCount * 2) to get the offset of the middle of each player area (360 / (2 * 2) = 90)
            // (player position - segment offset) to get the correct position to place the player (180 - 90 = 90)
            paddle.Initialise(player.id, playerDistance, 360.0f / alivePlayerCount * (player.id + 1) + mapRotationOffset - 360.0f / (alivePlayerCount * 2), 180.0f / alivePlayerCount);
            paddle.SetColor(player.color);
            paddle.name = "Player " + player.id;
        }
    }
       

    void BuildGameBoard()
    {
        map.GenerateMap();
    }

    public Color GetPlayerColor(int index)
    {
        if (index < playerEmissives.Count) return playerEmissives[index];
        else return playerEmissives[playerEmissives.Count - 1];
    }

    // returns true if a hit causes a player to die.
    public bool OnSheildHit(int alivePlayerID)
    {
        Player player = players[alivePlayerID];
        if (player.shieldHealth <= 0)
        {
            alivePlayers.Add(player);
            players.Remove(player);
            map.GenerateMap();
            return true;
        }
        else
        {
            player.shieldHealth--;
            return false;
        }
    }
}
