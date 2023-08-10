using UnityEngine;
using static GameManager;

public class Player : MonoBehaviour
{
    public Paddle paddle;
    [HideInInspector] public int ID { get { return instance.players.IndexOf(this); } private set { } }

    [HideInInspector] public Vector2 movementInput;

    [HideInInspector] public int shieldHealth;

    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    public void Dash()
    {
        paddle.Dash();
    }

    void FixedUpdate()
    {
        paddle.Move(movementInput);
    }
        
}
