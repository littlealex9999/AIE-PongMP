using UnityEngine;
using static GameManager;

public class Player : MonoBehaviour
{
    public Paddle paddle;
    [HideInInspector] public int ID { get { return gameManagerInstance.players.IndexOf(this); } private set { } }

    [HideInInspector] public Vector2 movementInput;

    [HideInInspector] public int shieldHealth;

    [HideInInspector] public Color color;

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
