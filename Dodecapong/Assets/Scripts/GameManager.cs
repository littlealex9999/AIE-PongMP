using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Map map;
    public Ball ball;

    public GameObject paddleObject;

    public int playerCount;
    List<Paddle> players = new List<Paddle>();

    [Range(0, 360)] public float mapRotationOffset = 0.0f;

    [ColorUsage(true, true)] public List<Color> playerEmissives = new List<Color>();

    public float playerDistance = 4.0f;
    [Min(0)] public int shieldHits = 1;

    void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);

        BuildGameBoard();
    }

    void Update()
    {
        
    }

    void BuildGameBoard()
    {
        map.angleOffset = mapRotationOffset;

        map.shieldLevels.Clear();

        for (int i = 0; i < playerCount; i++) {
            players.Add(Instantiate(paddleObject, map.transform).GetComponent<Paddle>());

            // the "starting position" is as follows, with 2 players as an example:
            // 360 / player count to get the base angle (360 / 2 = 180)
            // ... * i + 1 to get a multiple of the base angle based on the player (180 * (0 + 1) = 180)
            players[i].Initialise(i, playerDistance, 360.0f / playerCount * (i + 1) + mapRotationOffset - 360.0f / (playerCount * 2), 360.0f / playerCount);

            if (i < playerEmissives.Count) {
                players[i].GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", GetPlayerColor(i));
            }

            map.shieldLevels.Add(shieldHits);
        }

        // Set the colors of the linerenderer to match the player's colors
        LineRenderer mapRenderer = map.GetComponent<LineRenderer>();

        GradientColorKey[] colorKeys = new GradientColorKey[playerCount];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[playerCount];
        for (int i = 0; i < playerCount; i++) {
            Color targetColor = GetPlayerColor(i);
            float time = 1.0f / playerCount * (i + 1);
            colorKeys[i] = new GradientColorKey(targetColor, time);
            alphaKeys[i] = new GradientAlphaKey(targetColor.a, time);
        }

        Gradient gradient = new Gradient();
        gradient.mode = GradientMode.Fixed;
        gradient.SetKeys(colorKeys, alphaKeys);

        mapRenderer.colorGradient = gradient;

        map.CalculateCircle();
    }

    Color GetPlayerColor(int index)
    {
        if (index < playerEmissives.Count) return playerEmissives[index];
        else return playerEmissives[playerEmissives.Count - 1];
    }
}
