using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Variables", menuName = "")]
public class GameVariables : ScriptableObject
{
    // comments will be placed on the same line for things that may not be implemented

    public enum WinType
    {
        ELIMINATION = default,
        SCORE,
    }

    #region Player
    [Header("Player")] public float playerSpeed;
    public Vector2 playerSize;
    public float playerStickiness; // note: may not be implemented
    public bool dashEnabled;
    #endregion

    #region Ball
    [Header("Ball"), Min(1)] public int ballCount;
    public float ballSpeed;
    public float ballSize;
    #endregion

    #region Goal & Shield
    [Header("Goal & Shield"), Min(0)] public int shieldLives;
    #endregion

    #region Field
    [Header("Field")] public bool obstacleSpawn;
    public float powerupFrequency;
    #endregion

    #region Victory
    [Header("Victory")] public WinType winType;
    public bool useTimer;
    public float timeInSeconds;
    #endregion
}