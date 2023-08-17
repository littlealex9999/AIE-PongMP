using UnityEngine;

public class Player : MonoBehaviour
{
    public Paddle paddle;
    [HideInInspector] public int ID { get { return GameManager.instance.players.IndexOf(this); } private set { } }

    [HideInInspector] public Vector2 movementInput;

    [HideInInspector] public int shieldHealth;

    [HideInInspector] public Color color;

    [HideInInspector] public float dashDuration;
    [HideInInspector] public float dashCooldown;

    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    public void Dash()
    {
        if (readyToDash)
        {
            readyToDash = false;
            progress = 0;
            StartCoroutine(paddle.Dash(movementInput, dashDuration));
        }
    }

    float progress;
    bool readyToDash = true;

    void FixedUpdate()
    {
        // animation value: progress / (dashCooldown + dashDuration)

        if (progress < dashCooldown + dashDuration)
        {
            progress += Time.fixedDeltaTime;
        }
        else
        {
            readyToDash = true;
        }

        if (!paddle.dashing && !GameManager.instance.holdGameplay) paddle.Move(movementInput);
    }
}
