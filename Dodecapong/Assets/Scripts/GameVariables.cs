using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Variables", menuName = "")]
public class GameVariables : ScriptableObject
{
    // comments will be placed on the same line for things that may not be implemented

    public GameVariables() { }
    public GameVariables(GameVariables gv)
    {
        playerSpeed = gv.playerSpeed;
        playerSize = gv.playerSize;
        playerStickiness = gv.playerStickiness;
        dashEnabled = gv.dashEnabled;

        ballCount = gv.ballCount;
        ballSpeed = gv.ballSpeed;
        ballSpeedPerHit = gv.ballSpeedPerHit;
        ballSize = gv.ballSize;

        shieldLives = gv.shieldLives;

        obstacleSpawn = gv.obstacleSpawn;
        powerupFrequency = gv.powerupFrequency;

        winType = gv.winType;
        useTimer = gv.useTimer;
        timeInSeconds = gv.timeInSeconds;
    }

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

    public void SetPlayerSpeed(float value) { playerSpeed = value; }
    public void SetPlayerSize(float value) { playerSize.x = value; }
    public void SetPlayerStickiness(float value) { playerStickiness = value; }
    public void SetPlayerDashEnabled(bool enabled) { dashEnabled = enabled; }
    #endregion

    #region Ball
    [Header("Ball"), Min(1)] public int ballCount = 1;
    public float ballSpeed;
    public float ballSpeedPerHit;
    public float ballSize;

    public void SetBallCount(int value) { ballCount = value; }
    public void SetBallSpeed(float value) { ballSpeed = value; }
    public void SetBallSpeedPerHit(float value) { ballSpeedPerHit = value; }
    public void SetBallSize(float value) { ballSize = value; }
    #endregion

    #region Goal & Shield
    [Header("Goal & Shield"), Min(0)] public int shieldLives;

    public void SetShieldLives(int value) { shieldLives = value; }
    #endregion

    #region Field
    [Header("Field")] public bool obstacleSpawn;
    public float powerupFrequency;

    public void SetObstaclesEnabled(bool enabled) { obstacleSpawn = enabled; }
    public void SetPowerupFrequency(float value) { powerupFrequency = value; }
    #endregion

    #region Victory
    [Header("Victory")] public WinType winType;
    public bool useTimer;
    public float timeInSeconds;

    public void SetWinType(WinType type) { winType = type; }
    public void SetTimerEnabled(bool enabled) { useTimer = enabled; }
    public void SetTimerSeconds(float value) { timeInSeconds = value; }
    #endregion
}