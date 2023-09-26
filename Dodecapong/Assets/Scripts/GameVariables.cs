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
        playerSizes = gv.playerSizes;
        playerRotationalForce = gv.playerRotationalForce;
        playerNormalBending = gv.playerNormalBending;
        dashEnabled = gv.dashEnabled;
        dashDuration = gv.dashDuration;
        dashCooldown = gv.dashCooldown;
        hitEnabled = gv.hitEnabled;
        hitDuration = gv.hitDuration;
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

    public static void SetPlayerSpeed(float value) { GameManager.instance.gameVariables.playerSpeed = value; }
    public static void SetPlayersSize(List<Vector3> value) { GameManager.instance.gameVariables.playerSizes = value; }
    
    public static void SetPlayerRotationalForce(float value) { GameManager.instance.gameVariables.playerRotationalForce = value; }
    public static void SetPlayerNormalBending(float value) { GameManager.instance.gameVariables.playerNormalBending = value; }
    public static void SetPlayerDashEnabled(bool enabled) { GameManager.instance.gameVariables.dashEnabled = enabled; }
    public static void SetPlayerDashDuration(float value) { GameManager.instance.gameVariables.dashDuration = value; }
    public static void SetPlayerDashCooldown(float value) { GameManager.instance.gameVariables.dashCooldown = value; }
    public static void SetPlayerHitEnabled(bool enabled) { GameManager.instance.gameVariables.hitEnabled = enabled; }
    public static void SetPlayerHitDuration(float value) { GameManager.instance.gameVariables.hitDuration = value; }
    public static void SetPlayerHitCooldown(float value) { GameManager.instance.gameVariables.hitCooldown = value; }
    public static void SetPlayerHitStrength(float value) { GameManager.instance.gameVariables.hitStrength = value; }
    public static void SetPlayerGrabEnabled(bool enabled) { GameManager.instance.gameVariables.hitEnabled = enabled; }
    public static void SetPlayerGrabDuration(float value) { GameManager.instance.gameVariables.hitDuration = value; }
    public static void SetPlayerGrabCooldown(float value) { GameManager.instance.gameVariables.hitCooldown = value; }
    #endregion

    #region Ball
    [Header("Ball"), Min(1)] public int ballCount = 1;
    public float ballSpeed = 4.0f;
    public float ballSpeedDamp = 1.2f;
    public float ballSpeedPerHit = 0.0f;
    public float ballSize = 0.5f;
    [Range(0, 1)] public float shieldBounceTowardsCenterBias;

    public static void SetBallCount(int value) { GameManager.instance.gameVariables.ballCount = value; }
    public static void SetBallCount(float value) { GameManager.instance.gameVariables.ballCount = (int)value; }
    public static void SetBallSpeed(float value) { GameManager.instance.gameVariables.ballSpeed = value; }
    public static void SetBallSpeedDamp(float value) { GameManager.instance.gameVariables.ballSpeedDamp = value; }
    public static void SetBallSpeedPerHit(float value) { GameManager.instance.gameVariables.ballSpeedPerHit = value; }
    public static void SetBallSize(float value) { GameManager.instance.gameVariables.ballSize = value; }
    public static void SetShieldBounceBias(float value) { GameManager.instance.gameVariables.shieldBounceTowardsCenterBias = value; }
    #endregion

    #region Goal & Shield
    [Header("Goal & Shield"), Min(0)] public int shieldLives = 1;

    public static void SetShieldLives(int value) { GameManager.instance.gameVariables.shieldLives = value; }
    public static void SetShieldLives(float value) { GameManager.instance.gameVariables.shieldLives = (int)value; }
    #endregion

    #region Field
    [Range(0, 1)] public float transformerFrequency;
    public float transformerPower = 1.0f;
    public Transformer.TransformerTypes enabledTransformers;

    public static void SetTransformerFrequency(float value) { GameManager.instance.gameVariables.transformerFrequency = value; }
    public static void SetTransformerPower(float value) { GameManager.instance.gameVariables.transformerPower = value; }
    public void SetBits(Transformer.TransformerTypes mask, bool add) { enabledTransformers = add ? enabledTransformers | mask : enabledTransformers & ~mask; }

    public static void SetBallSpeedTransformer(bool enabled) { GameManager.instance.gameVariables.SetBits(Transformer.TransformerTypes.BALLSPEED, enabled); }
    public static void SetBallSizeTransformer(bool enabled) { GameManager.instance.gameVariables.SetBits(Transformer.TransformerTypes.BALLSIZE, enabled); }
    public static void SetPlayerSpeedTransformer(bool enabled) { GameManager.instance.gameVariables.SetBits(Transformer.TransformerTypes.PLAYERSPEED, enabled); }
    public static void SetPlayerSizeTransformer(bool enabled) { GameManager.instance.gameVariables.SetBits(Transformer.TransformerTypes.PLAYERSIZE, enabled); }
    public static void SetDashCooldownTransformer(bool enabled) { GameManager.instance.gameVariables.SetBits(Transformer.TransformerTypes.DASHCOOLDOWN, enabled); }
    public static void SetShieldLivesTransformer(bool enabled) { GameManager.instance.gameVariables.SetBits(Transformer.TransformerTypes.SHIELDHEALTH, enabled); }
    public static void SetBallCountTransformer(bool enabled) { GameManager.instance.gameVariables.SetBits(Transformer.TransformerTypes.BALLCOUNT, enabled); }
    public static void SetBlackHoleTransformer(bool enabled) { GameManager.instance.gameVariables.SetBits(Transformer.TransformerTypes.BLACKHOLE, enabled); }
    #endregion

    #region Victory
    [Header("Victory")] public WinType winType = WinType.ELIMINATION;
    public bool useTimer = true;
    public float timeInSeconds = 20.0f;

    public static void SetWinType(WinType type) { GameManager.instance.gameVariables.winType = type; }
    public static void SetTimerEnabled(bool enabled) { GameManager.instance.gameVariables.useTimer = enabled; }
    public static void SetTimerSeconds(float value) { GameManager.instance.gameVariables.timeInSeconds = value; }
    #endregion
}