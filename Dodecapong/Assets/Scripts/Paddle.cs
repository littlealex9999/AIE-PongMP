using System;
using UnityEngine;
using static GameManager;

public class Paddle : MonoBehaviour
{
    float playerMidPoint;
    float angleDeviance; // the max amount you can move from your starting rotation

    [Tooltip("In degrees per second")] public float moveSpeed = 90;

    public float pushDistance = 0.1f;
    public float pushStrength = 3.0f;

    [HideInInspector] public Vector3 facingDirection = Vector3.right;

    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    public void CalculateLimits(int alivePlayerId, int alivePlayerCount, float mapRotationOffset)
    {
        // the "starting position" is as follows, with 2 players as an example:
        // 360 / player count to get the base angle (360 / 2 = 180)
        // ... * i + 1 to get a multiple of the base angle based on the player (180 * (0 + 1) = 180)
        // ... + mapRotationOffset to ensure the paddles spawn relative to the way the map is rotated (+ 0 in example, so ignored)
        // 360 / (playerCount * 2) to get the offset of the middle of each player area (360 / (2 * 2) = 90)
        // (player position - segment offset) to get the correct position to place the player (180 - 90 = 90)
        float segmentOffset = 180.0f / alivePlayerCount;

        playerMidPoint = 360.0f / alivePlayerCount * (alivePlayerId + 1) + mapRotationOffset - segmentOffset;
        angleDeviance = segmentOffset;

        // get the direction this paddle is facing, set its position, and have its rotation match
        facingDirection = Quaternion.Euler(0, 0, playerMidPoint) * -transform.up;
        SetPosition(playerMidPoint);
    }

    /// <summary>
    /// Use a Vector3 input from a controller and dot it with the direction this paddle is facing 
    /// to get a move input based on the direction that was input and the direction you can move
    /// </summary>
    /// <param name="input"></param>
    /// <param name="clampSpeed"></param>
    public void Move(Vector2 input, bool clampSpeed = true)
    {
        if (GameManager.gameManagerInstance.holdGameplay) return;

        float moveTarget = Vector2.Dot(input, Quaternion.Euler(0, 0, 90) * facingDirection) * input.magnitude * moveSpeed;
        if (clampSpeed) moveTarget = Mathf.Clamp(moveTarget, -moveSpeed, moveSpeed);

        transform.RotateAround(Vector3.zero, Vector3.back, moveTarget * Time.fixedDeltaTime);

        float maxDev = playerMidPoint + angleDeviance;
        float minDev = playerMidPoint - angleDeviance;
        float angle = Angle(transform.position);

        if (angle > maxDev || angle < minDev)
        {
            float lowComparison = Mathf.Abs(360 - angle);
            float lowExtraComparison = Mathf.Abs(minDev - angle);
            if (lowExtraComparison < lowComparison) lowComparison = lowExtraComparison;

            if (maxDev >= 360) maxDev -= 360;
            float highComparison = Mathf.Abs(maxDev - angle);

            if (lowComparison < highComparison)
            {
                SetPosition(minDev);
            }
            else
            {
                SetPosition(maxDev);
            }
        }
    }

    public void SetPosition(float angle)
    {
        transform.position = gameManagerInstance.map.GetTargetPointInCircleLocal(angle).normalized * gameManagerInstance.playerDistance;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }


    public static float Angle(Vector2 vector2)
    {
        float ret;

        if (vector2.x < 0)
        {
            ret = 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            ret = Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
        }

        return 360 - ret;
    }

    public void SetColor(Color col)
    {
        GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", col);
    }

    public Vector2 BounceNormal()
    {
        return (transform.position - Vector3.zero).normalized;
    }

    internal void Dash()
    {
        throw new NotImplementedException();
    }
}
