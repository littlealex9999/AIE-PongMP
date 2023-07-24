using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class Paddle : MonoBehaviour
{
    public float startingRotation;
    public float moveSpeed = 90;
    public int playerID;
    public Ball ball;

    public float pushStrength;

    Vector3 facingDirection = Vector3.right;
    float angleDeviance;

    private void Awake()
    {
        
    }

    void Update()
    {
       
        //if (playerID == 0)
        //{
        //    transform.RotateAround(Vector3.zero, Vector3.back, rotationalDegreesPerSecond * -xInput * Time.deltaTime);

        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        if (Vector3.Distance(ball.transform.position, transform.position) < 1)
        //        {
        //            ball.AddVelocity(Vector2.one * pushStrength);
        //        }
        //    }
        //}
            
        //if (playerID == 1)
        //{
        //    float xInput = 0;
        //    if (Input.GetKey(KeyCode.RightArrow)) xInput = 1;
        //    if (Input.GetKey(KeyCode.LeftArrow)) xInput = -1;
        //    transform.RotateAround(Vector3.zero, Vector3.back, rotationalDegreesPerSecond * -xInput * Time.deltaTime);
        //};

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

        // get the direction this paddle is facing, set its position, and have its rotation match
        facingDirection = Quaternion.Euler(0, 0, startingAngle) * -transform.up;
        transform.position = -facingDirection * startingDistance;
        transform.rotation = Quaternion.Euler(0, 0, startingAngle + 90);
    }

    /// <summary>
    /// Use a Vector3 input from a controller and dot it with the direction this paddle is facing 
    /// to get a move input based on the direction that was input and the direction you can move
    /// </summary>
    /// <param name="input"></param>
    /// <param name="clampSpeed"></param>
    public void Move(Vector3 input, bool clampSpeed = true)
    {
        float moveTarget = Vector3.Dot(input, Quaternion.Euler(0, 0, 90) * facingDirection) * input.magnitude * moveSpeed;
        if (clampSpeed) moveTarget = Mathf.Clamp(moveTarget, -moveSpeed, moveSpeed);

        transform.RotateAround(Vector3.zero, Vector3.back, moveTarget * Time.deltaTime);
    }
}
