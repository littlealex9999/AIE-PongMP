using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Variables")]
public class GameVariables : ScriptableObject
{
    // comments will be placed on the same line for things that may not be implemented

    public GameVariables() { }
    public GameVariables(GameVariables gv)
    {
        playerSpeed = gv.playerSpeed;
        playerSize = gv.playerSize;
        playerRotationalForce = gv.playerRotationalForce;
        playerNormalBending = gv.playerNormalBending;
        //playerBounceTowardsCenterBias = gv.playerBounceTowardsCenterBias;
        dashEnabled = gv.dashEnabled;
        dashDuration = gv.dashDuration;
        dashCooldown = gv.dashCooldown;
        hitEnabled = gv.hitEnabled;
        hitDuration = gv.hitDuration;
        hitCooldown = gv.hitCooldown;

        ballCount = gv.ballCount;
        ballSpeed = gv.ballSpeed;
        ballSpeedPerHit = gv.ballSpeedPerHit;
        ballSize = gv.ballSize;
        shieldBounceTowardsCenterBias = gv.shieldBounceTowardsCenterBias;

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
    public Vector3 playerSize = new Vector3(0.03f, 0.03f, 0.03f);
    public float playerRotationalForce = 0.5f;
    public float playerNormalBending = 2.0f;
    //public float playerBounceTowardsCenterBias;
    public bool dashEnabled = true;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1;
    public bool hitEnabled = true;
    public float hitDuration = 0.1f;
    public float hitCooldown = 1;
    public float hitStrength = 4;

    public void SetPlayerSpeed(float value) { playerSpeed = value; }
    public void SetPlayerSize(Vector3 value) { playerSize = value; }
    public void SetPlayerRotationalForce(float value) { playerRotationalForce = value; }
    public void SetPlayerNormalBending(float value) { playerNormalBending = value; }
    //public void SetPlayerBounceBias(float value) { playerBounceTowardsCenterBias = value; }
    public void SetPlayerDashEnabled(bool enabled) { dashEnabled = enabled; }
    public void SetPlayerDashDuration(float value) { dashDuration = value; }
    public void SetPlayerDashCooldown(float value) { dashCooldown = value; }
    public void SetPlayerHitEnabled(bool enabled) { hitEnabled = enabled; }
    public void SetPlayerHitDuration(float value) { hitDuration = value; }
    public void SetPlayerHitCooldown(float value) { hitCooldown = value; }
    public void SetPlayerHitStrength(float value) { hitStrength = value; }
    #endregion

    #region Ball
    [Header("Ball"), Min(1)] public int ballCount = 1;
    public float ballSpeed = 4.0f;
    public float ballSpeedDamp = 1.2f;
    public float ballSpeedPerHit = 0.0f;
    public float ballSize = 0.5f;
    [Range(0, 1)] public float shieldBounceTowardsCenterBias;

    public void SetBallCount(int value) { ballCount = value; }
    public void SetBallSpeed(float value) { ballSpeed = value; }
    public void SetBallSpeedDamp(float value) { ballSpeedDamp = value; }
    public void SetBallSpeedPerHit(float value) { ballSpeedPerHit = value; }
    public void SetBallSize(float value) { ballSize = value; }
    public void SetShieldBounceBias(float value) { shieldBounceTowardsCenterBias = value; }
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