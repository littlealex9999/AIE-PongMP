using UnityEngine;
using static GameManager;

public class Paddle : MonoBehaviour
{
    public float startingRotation;
    float distance;

    [Tooltip("In degrees per second")] public float moveSpeed = 90;
    public int playerID;

    public float pushDistance = 0.1f;
    public float pushStrength = 3.0f;

    Vector3 facingDirection = Vector3.right;

    // the max amount you can move from your starting rotation
    float angleDeviance;

    public void Initialise(int playerID, float startingDistance, float startingAngle, float maxAngleDeviance)
    {
        this.playerID = playerID;
        startingRotation = startingAngle;
        angleDeviance = maxAngleDeviance;
        distance = startingDistance;

        // get the direction this paddle is facing, set its position, and have its rotation match
        facingDirection = Quaternion.Euler(0, 0, startingAngle) * -transform.up;
        SetPosition(startingAngle);
    }

    /// <summary>
    /// Use a Vector3 input from a controller and dot it with the direction this paddle is facing 
    /// to get a move input based on the direction that was input and the direction you can move
    /// </summary>
    /// <param name="input"></param>
    /// <param name="clampSpeed"></param>
    public void Move(Vector2 input, bool clampSpeed = true)
    {
        float moveTarget = Vector2.Dot(input, Quaternion.Euler(0, 0, 90) * facingDirection) * input.magnitude * moveSpeed;
        if (clampSpeed) moveTarget = Mathf.Clamp(moveTarget, -moveSpeed, moveSpeed);

        transform.RotateAround(Vector3.zero, Vector3.back, moveTarget * Time.deltaTime);

        float maxDev = startingRotation + angleDeviance;
        float minDev = startingRotation - angleDeviance;
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
        transform.position = GameManager.instance.map.GetTargetPointInCircleLocal(angle).normalized * distance;
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
}
