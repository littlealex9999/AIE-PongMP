using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Map map;
    Ball ball;

    public GameObject paddleObject;

    public int playerCount;
    List<Paddle> players = new List<Paddle>();

    [ColorUsage(true, true)] public List<Color> playerEmissives = new List<Color>();

    public float playerDistance = 4.0f;

    void Awake()
    {
        BuildGameBoard();
    }

    void Update()
    {
        
    }

    void BuildGameBoard()
    {
        for (int i = 0; i < playerCount; i++) {
            players.Add(Instantiate(paddleObject, map.transform).GetComponent<Paddle>());
            players[i].Initialise(i, playerDistance, 360 / playerCount * (i + 1) - 90, 180f);

            if (i < playerEmissives.Count) {
                players[i].GetComponent<MeshRenderer>().material.SetColor("emissiveMap", playerEmissives[i]);
            }
        }
    }
}
