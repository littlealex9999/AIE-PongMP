using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Variables")]
public class GameVariables : ScriptableObject
{
    // comments will be placed on the same line for things that may not be implemented

    public GameVariables() { }
    //public GameVariables(GameVariables gv)
    //{
    //    Copy(gv);
    //}

    public void Copy(GameVariables gv)
    {
        playerSpeed = gv.playerSpeed;
        playerSizes = gv.playerSizes;
        playerRotationalForce = gv.playerRotationalForce;
        playerNormalBending = gv.playerNormalBending;
        dashEnabled = gv.dashEnabled;
        dashDistance = gv.dashDistance;
        dashDuration = gv.dashDuration;
        dashCooldown = gv.dashCooldown;
        hitEnabled = gv.hitEnabled;
        hitDuration = gv.hitDuration;
        hitStrength = gv.hitStrength;
        hitCooldown = gv.hitCooldown;
        grabEnabled = gv.grabEnabled;
        grabDuration = gv.grabDuration;
        grabCooldown = gv.grabCooldown;

        ballCount = gv.ballCount;
        ballSpeed = gv.ballSpeed;
        ballSpeedPerHit = gv.ballSpeedPerHit;
        ballSize = gv.ballSize;
        ballSpeedDamp = gv.ballSpeedDamp;
        shieldBounceTowardsCenterBias = gv.shieldBounceTowardsCenterBias;

        shieldLives = gv.shieldLives;
        enableHitstun = gv.enableHitstun;

        transformerFrequency = gv.transformerFrequency;
        transformerPower = gv.transformerPower;
        enabledTransformers = gv.enabledTransformers;

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
    [Tooltip("player size based on player count Element 0 = 2 players Element 1 = 3 players ect.")] 
    public List<Vector3> playerSizes = new List<Vector3>() {
        new Vector3(0.04f, 0.04f, 0.04f),
        new Vector3(0.04f, 0.04f, 0.04f),
        new Vector3(0.03f, 0.03f, 0.03f),
        new Vector3(0.03f, 0.03f, 0.03f),
        new Vector3(0.03f, 0.03f, 0.03f),
        new Vector3(0.02f, 0.02f, 0.02f),
        new Vector3(0.02f, 0.02f, 0.02f),
    };
    public float playerRotationalForce = 0.5f;
    public float playerNormalBending = 2.0f;

    [Space()]
    public bool dashEnabled = true;
    public float dashDistance = 0.5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1;

    [Space()]
    public bool hitEnabled = true;
    public float hitDuration = 0.1f;
    public float hitCooldown = 1;
    public float hitStrength = 4;
    
    [Space()]
    public bool grabEnabled = true;
    public float grabDuration = 1;
    public float grabCooldown = 1;

    public static void SetPlayerSpeed(float value) { GameManager.instance.selectedGameVariables.playerSpeed = value; }

    public static void SetPlayerRotationalForce(float value) { GameManager.instance.selectedGameVariables.playerRotationalForce = value; }
    public static void SetPlayerNormalBending(float value) { GameManager.instance.selectedGameVariables.playerNormalBending = value; }
    public static void SetPlayerDashEnabled(bool enabled) { GameManager.instance.selectedGameVariables.dashEnabled = enabled; }
    public static void SetPlayerDashDistance(float value) { GameManager.instance.selectedGameVariables.dashDistance = value; }
    public static void SetPlayerDashDuration(float value) { GameManager.instance.selectedGameVariables.dashDuration = value; }
    public static void SetPlayerDashCooldown(float value) { GameManager.instance.selectedGameVariables.dashCooldown = value; }
    public static void SetPlayerHitEnabled(bool enabled) { GameManager.instance.selectedGameVariables.hitEnabled = enabled; }
    public static void SetPlayerHitDuration(float value) { GameManager.instance.selectedGameVariables.hitDuration = value; }
    public static void SetPlayerHitCooldown(float value) { GameManager.instance.selectedGameVariables.hitCooldown = value; }
    public static void SetPlayerHitStrength(float value) { GameManager.instance.selectedGameVariables.hitStrength = value; }
    public static void SetPlayerGrabEnabled(bool enabled) { GameManager.instance.selectedGameVariables.hitEnabled = enabled; }
    public static void SetPlayerGrabDuration(float value) { GameManager.instance.selectedGameVariables.hitDuration = value; }
    public static void SetPlayerGrabCooldown(float value) { GameManager.instance.selectedGameVariables.hitCooldown = value; }
    #endregion

    #region Ball
    [Header("Ball"), Min(1)] public int ballCount = 1;
    public float ballSpeed = 4.0f;
    public float ballSpeedDamp = 1.2f;
    public float ballSpeedPerHit = 0.0f;
    public float ballSize = 0.5f;
    [Range(0, 1)] public float shieldBounceTowardsCenterBias;

    public static void SetBallCount(int value) { GameManager.instance.selectedGameVariables.ballCount = value; }
    public static void SetBallCount(float value) { GameManager.instance.selectedGameVariables.ballCount = (int)value; }
    public static void SetBallSpeed(float value) { GameManager.instance.selectedGameVariables.ballSpeed = value; }
    public static void SetBallSpeedDamp(float value) { GameManager.instance.selectedGameVariables.ballSpeedDamp = value; }
    public static void SetBallSpeedPerHit(float value) { GameManager.instance.selectedGameVariables.ballSpeedPerHit = value; }
    public static void SetBallSize(float value) { GameManager.instance.selectedGameVariables.ballSize = value; }
    public static void SetShieldBounceBias(float value) { GameManager.instance.selectedGameVariables.shieldBounceTowardsCenterBias = value; }
    #endregion

    #region Goal & Shield
    [Header("Goal & Shield"), Min(0)] public int shieldLives = 1;
    public bool enableHitstun = true;

    public static void SetShieldLives(int value) { GameManager.instance.selectedGameVariables.shieldLives = value; }
    public static void SetShieldLives(float value) { GameManager.instance.selectedGameVariables.shieldLives = (int)value; }
    public static void SetHitstunEnabled(bool enabled) { GameManager.instance.selectedGameVariables.enableHitstun = enabled; }
    #endregion

    #region Field
    [Range(0, 1)] public float transformerFrequency;
    public float transformerPower = 1.0f;
    public Transformer.TransformerTypes enabledTransformers;

    public static void SetTransformerFrequency(float value) { GameManager.instance.selectedGameVariables.transformerFrequency = value; }
    public static void SetTransformerPower(float value) { GameManager.instance.selectedGameVariables.transformerPower = value; }
    public void SetBits(Transformer.TransformerTypes mask, bool add) { enabledTransformers = add ? enabledTransformers | mask : enabledTransformers & ~mask; }

    public static void SetBallSpeedTransformer(bool enabled) { GameManager.instance.selectedGameVariables.SetBits(Transformer.TransformerTypes.BALLSPEED, enabled); }
    public static void SetBallSizeTransformer(bool enabled) { GameManager.instance.selectedGameVariables.SetBits(Transformer.TransformerTypes.BALLSIZE, enabled); }
    public static void SetPlayerSpeedTransformer(bool enabled) { GameManager.instance.selectedGameVariables.SetBits(Transformer.TransformerTypes.PLAYERSPEED, enabled); }
    public static void SetPlayerSizeTransformer(bool enabled) { GameManager.instance.selectedGameVariables.SetBits(Transformer.TransformerTypes.PLAYERSIZE, enabled); }
    public static void SetDashCooldownTransformer(bool enabled) { GameManager.instance.selectedGameVariables.SetBits(Transformer.TransformerTypes.DASHCOOLDOWN, enabled); }
    public static void SetShieldLivesTransformer(bool enabled) { GameManager.instance.selectedGameVariables.SetBits(Transformer.TransformerTypes.SHIELDHEALTH, enabled); }
    public static void SetBallCountTransformer(bool enabled) { GameManager.instance.selectedGameVariables.SetBits(Transformer.TransformerTypes.BALLCOUNT, enabled); }
    public static void SetBlackHoleTransformer(bool enabled) { GameManager.instance.selectedGameVariables.SetBits(Transformer.TransformerTypes.BLACKHOLE, enabled); }
    #endregion

    #region Victory
    [Header("Victory")] public WinType winType = WinType.ELIMINATION;
    public bool useTimer = true;
    public float timeInSeconds = 20.0f;

    public static void SetWinType(WinType type) { GameManager.instance.selectedGameVariables.winType = type; }
    public static void SetTimerEnabled(bool enabled) { GameManager.instance.selectedGameVariables.useTimer = enabled; }
    public static void SetTimerSeconds(float value) { GameManager.instance.selectedGameVariables.timeInSeconds = value; }
    #endregion

    #region Extra
    public static void SetHapticsEnabled(bool enabled) { GameManager.instance.enableHaptics = enabled; }
    public static void SetScreenShakeEnabled(bool enabled) { GameManager.instance.enableScreenShake = enabled; }
    #endregion
}