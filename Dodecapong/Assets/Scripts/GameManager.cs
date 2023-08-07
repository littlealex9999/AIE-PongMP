using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Map map;
    public Ball ball;

    public GameObject paddleObject;

    public GameVariables defaultGameVariables;
    GameVariables gameVariables;

    public int playerCount;

    [Range(0, 360)] public float mapRotationOffset = 0.0f;

    [ColorUsage(true, true), SerializeField] List<Color> playerEmissives = new List<Color>();

    public float playerDistance = 4.0f;

    void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);

        if (defaultGameVariables) gameVariables = new GameVariables(defaultGameVariables);
        else gameVariables = new GameVariables();

        Initialise();
    }

    void Initialise()
    {
        ball.constantVel = gameVariables.ballSpeed;
        ball.transform.localScale = new Vector3(gameVariables.ballSize, gameVariables.ballSize, gameVariables.ballSize);

        BuildGameBoard();
    }

    void BuildGameBoard()
    {
        map.shieldLevels.Clear();

        for (int i = 0; i < playerCount; i++) {
            map.shieldLevels.Add(gameVariables.shieldLives);
            map.players.Add(Instantiate(paddleObject, map.transform).GetComponent<Paddle>());

            // changing the order of player size variables for easier readability in the inspector.
            // x correlates to what the camera sees as horizontal length, while y correlates to vertical width
            // though the actual scaling is different
            map.players[i].transform.localScale = new Vector3(gameVariables.playerSize.y, gameVariables.playerSize.x, gameVariables.playerSize.y);

            // the "starting position" is as follows, with 2 players as an example:
            // 360 / player count to get the base angle (360 / 2 = 180)
            // ... * i + 1 to get a multiple of the base angle based on the player (180 * (0 + 1) = 180)
            // ... + mapRotationOffset to ensure the paddles spawn relative to the way the map is rotated (+ 0 in example, so ignored)
            // 360 / (playerCount * 2) to get the offset of the middle of each player area (360 / (2 * 2) = 90)
            // (player position - segment offset) to get the correct position to place the player (180 - 90 = 90)
            map.players[i].Initialise(i, playerDistance, 360.0f / playerCount * (i + 1) + mapRotationOffset - 360.0f / (playerCount * 2), 180.0f / playerCount);
            map.players[i].GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", GetPlayerColor(i));
            map.players[i].name = "Player " + i;
        }

        map.GenerateMap();
    }

    public Color GetPlayerColor(int index)
    {
        if (index < playerEmissives.Count) return playerEmissives[index];
        else return playerEmissives[playerEmissives.Count - 1];
    }
}
