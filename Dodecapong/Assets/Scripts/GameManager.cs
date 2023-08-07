using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    public static GameManager instance;

    public Map map;
    public Ball ball;

    public GameObject paddleObject;

    [Range(0, 360)] public float mapRotationOffset = 0.0f;

    [ColorUsage(true, true), SerializeField] List<Color> playerEmissives = new List<Color>();

    public float playerDistance = 4.0f;
    [Min(0)] public int shieldHits = 1;

    public List<Image> playerImages;

    public GameObject shieldTextObj;
    public Transform shieldTextParent;
    public List<TextMeshProUGUI> shieldText = new List<TextMeshProUGUI>();
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
            case GameState.GAMEOVER:

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
        foreach (Player player in alivePlayers)
        {
            Paddle paddle;
            if (player.paddle == null) player.paddle = paddle = Instantiate(paddleObject, map.transform).GetComponent<Paddle>();
            else paddle = player.paddle;

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
        GeneratePaddles();
        map.GenerateMap();
        UpdateShields();
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
