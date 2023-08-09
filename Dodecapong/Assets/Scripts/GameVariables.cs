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

        obstacleFrequency = gv.obstacleFrequency;
        blackHoleEnabled = gv.blackHoleEnabled;

        powerupFrequency = gv.powerupFrequency;
        speedPowerupEnabled = gv.speedPowerupEnabled;
        sizePowerupEnabled = gv.sizePowerupEnabled;

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
    [Header("Player")] public float playerSpeed = 90.0f;
    public Vector2 playerSize = new Vector2(2.0f, 0.5f);
    public float playerStickiness = 0; // note: may not be implemented
    public bool dashEnabled = true;

    public void SetPlayerSpeed(float value) { playerSpeed = value; }
    public void SetPlayerSize(float value) { playerSize.x = value; }
    public void SetPlayerStickiness(float value) { playerStickiness = value; }
    public void SetPlayerDashEnabled(bool enabled) { dashEnabled = enabled; }
    #endregion

    #region Ball
    [Header("Ball"), Min(1)] public int ballCount = 1;
    public float ballSpeed = 4.0f;
    public float ballSpeedPerHit = 0.0f;
    public float ballSize = 0.5f;

    public void SetBallCount(int value) { ballCount = value; }
    public void SetBallSpeed(float value) { ballSpeed = value; }
    public void SetBallSpeedPerHit(float value) { ballSpeedPerHit = value; }
    public void SetBallSize(float value) { ballSize = value; }
    #endregion

    #region Goal & Shield
    [Header("Goal & Shield"), Min(0)] public int shieldLives = 1;

    public void SetShieldLives(int value) { shieldLives = value; }
    #endregion

    #region Field
    [Header("Field"), Range(0, 1)] public float obstacleFrequency;
    public bool blackHoleEnabled;

    [Space(), Range(0, 1)] public float powerupFrequency;
    public bool speedPowerupEnabled;
    public bool sizePowerupEnabled;

    public void SetObstacleFrequency(float value) { obstacleFrequency = value; }
    public void SetBlackHoleEnabled(bool enabled) { blackHoleEnabled = enabled; }

    public void SetPowerupFrequency(float value) { powerupFrequency = value; }
    public void SetSpeedPowerupEnabled(bool enabled) { speedPowerupEnabled = enabled; }
    public void SetSizePowerupEnabled(bool enabled) { sizePowerupEnabled = enabled; }
    #endregion

    #region Victory
    [Header("Victory")] public WinType winType = WinType.ELIMINATION;
    public bool useTimer = true;
    public float timeInSeconds = 20.0f;

    public void SetWinType(WinType type) { winType = type; }
    public void SetTimerEnabled(bool enabled) { useTimer = enabled; }
    public void SetTimerSeconds(float value) { timeInSeconds = value; }
    #endregion
}