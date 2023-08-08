using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class GameManager : MonoBehaviour
{
  

   

    public static GameManager gameManagerInstance;

    public Map map;
    public Ball ball;

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
        if (!gameManagerInstance) gameManagerInstance = this;
        else Destroy(this);

        if (gameStateChanged == null) gameStateChanged = new UnityEvent();

        gameStateChanged.AddListener(OnGameStateChanged);

        UpdateGameState(GameState.MAINMENU);
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
        switch (gameState)
        {
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
        alivePlayers.Remove(player);
        if (alivePlayers.Count == 1) UpdateGameState(GameState.GAMEOVER);
        BuildGameBoard();
    }

    //
    // Gameplay
    //

    bool inGame;

    void StartGame()
    {
        inGame = true;
        foreach (Player player in players)
        {
            player.shieldHealth = shieldHits;
        }
        alivePlayers = players;
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
        map.GenerateMap();
        UpdatePaddles();
    }

    void UpdatePaddles()
    {
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            alivePlayers[i].paddle.gameObject.SetActive(true);
            alivePlayers[i].paddle.CalculateBounds(i, alivePlayers.Count, mapRotationOffset);
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
            EliminatePlayer(player);
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
