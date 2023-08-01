using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

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

    Collider2D collider;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        Vector3 input = Vector3.zero;

        // we could potentially concatenate a string to search for "HorizontalPlayer" + playerID
        // and use that to reduce the complexity of this script per player
        switch (playerID) {
            default:
            case 0:
                if (Input.GetKey(KeyCode.D)) input.x = 1;
                else if (Input.GetKey(KeyCode.A)) input.x = -1;

                if (Input.GetKey(KeyCode.W)) input.y = 1;
                else if (Input.GetKey(KeyCode.S)) input.y = -1;
                break;
            case 1:
                if (Input.GetKey(KeyCode.RightArrow)) input.x = 1;
                else if (Input.GetKey(KeyCode.LeftArrow)) input.x = -1;

                if (Input.GetKey(KeyCode.UpArrow)) input.y = 1;
                else if (Input.GetKey(KeyCode.DownArrow)) input.y = -1;
                break;
        }

        Move(input);
    }

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
    public void Move(Vector3 input, bool clampSpeed = true)
    {
        float maxDev = startingRotation + angleDeviance;
        float minDev = startingRotation - angleDeviance;

        float angle = Angle(transform.position);

        float moveTarget = Vector3.Dot(input, Quaternion.Euler(0, 0, 90) * facingDirection) * input.magnitude * moveSpeed;
        if (clampSpeed) moveTarget = Mathf.Clamp(moveTarget, -moveSpeed, moveSpeed);

        if (angle > minDev && moveTarget < 0 )
        {
            transform.RotateAround(Vector3.zero, Vector3.back, moveTarget * Time.deltaTime);
        }


        //if (angle > maxDev || angle < minDev)
        //{
        //    float lowComparison = 360 - angle;
        //    if (angle < lowComparison) lowComparison = angle;

        //    float highComparison = angle - maxDev;

        //    if (lowComparison < highComparison)
        //    {
        //        SetPosition(minDev);
        //    }
        //    else
        //    {
        //        SetPosition(maxDev);
        //    }
        //}
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
}
