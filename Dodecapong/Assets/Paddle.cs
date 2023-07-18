using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float startingRotation;
    public float rotationalDegreesPerSecond = 90;
    public int playerID;
    public Ball ball;

    public float pushStrength;

    private void Awake()
    {
        transform.RotateAround(Vector3.zero, Vector3.back, startingRotation);
    }

    void Update()
    {
       
        if (playerID == 0)
        {
            float xInput = 0;
            if (Input.GetKey(KeyCode.D)) xInput = 1;
            if (Input.GetKey(KeyCode.A)) xInput = -1;

            
            transform.RotateAround(Vector3.zero, Vector3.back, rotationalDegreesPerSecond * -xInput * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Vector3.Distance(ball.transform.position, transform.position) < 1)
                {
                    ball.AddVelocity(Vector2.one * pushStrength);
                }
            }
        }
            
        if (playerID == 1)
        {
            float xInput = 0;
            if (Input.GetKey(KeyCode.RightArrow)) xInput = 1;
            if (Input.GetKey(KeyCode.LeftArrow)) xInput = -1;
            transform.RotateAround(Vector3.zero, Vector3.back, rotationalDegreesPerSecond * -xInput * Time.deltaTime);
        };
    }
}
