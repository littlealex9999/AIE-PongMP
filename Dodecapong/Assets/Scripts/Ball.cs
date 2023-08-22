using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;

    public Map map;
    
    public float constantVel;
    public float ballRadius;
    public float dampStrength;
    [Range(0f, 1f), Tooltip("a value of 0 will have no effect. a value of 1 will make the ball go through the center every bounce")]
    public float shieldBounceTowardsCenterBias;
    [Range(0f, 1f), Tooltip("a value of 0 will have no effect. a value of 1 will make the ball go through the center every bounce")]
    public float paddleBounceTowardsCenterBias;

    float distFromCenter
    {
        get
        { 
            return Vector2.Distance(rb.position, Vector2.zero);
        }
    }

    private void OnValidate()
    {
        transform.localScale = new Vector3(ballRadius, ballRadius, ballRadius);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameManager.instance.gameStateChanged.AddListener(OnGameStateChanged);
    }

    private void OnGameStateChanged()
    {
        if (GameManager.instance.gameState == GameManager.GameState.MAINMENU)
        {
            rb.velocity = rb.transform.forward * constantVel;
        }
    }
    private void Bounce(float centerBias, Vector2 bounceNormal, float velocity)
    {
        Vector2 forward = rb.velocity.normalized;
        Vector2 bounceDir = Vector2.Reflect(forward, bounceNormal).normalized;
        Vector2 finalBounceDir = Vector2.Lerp(bounceDir, bounceNormal, centerBias).normalized;
        rb.velocity = finalBounceDir * velocity;
        rb.position += bounceNormal * 0.1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Paddle paddle))
        {
            if (paddle.hitting)
            {
                Bounce(paddleBounceTowardsCenterBias, paddle.BounceNormal(), rb.velocity.magnitude + paddle.hitStrength);
            }
            else
            {
                Bounce(paddleBounceTowardsCenterBias, paddle.BounceNormal(), rb.velocity.magnitude);
            }
        }
    }

    private void BounceOnBounds()
    {
        Vector2 shieldNormal = (Vector3.zero - transform.position).normalized;
        Bounce(shieldBounceTowardsCenterBias, shieldNormal, rb.velocity.magnitude);
    }

    public void ResetBall()
    {
        transform.position = Vector2.zero;

        int player = Random.Range(0, GameManager.instance.alivePlayers.Count);

        Vector2 dir = (GameManager.instance.alivePlayers[player].paddle.transform.position - transform.position).normalized;

        rb.velocity = dir * constantVel;
    }

    public void AddVelocity(Vector2 velocity)
    {
        rb.velocity += velocity;
    }

    private void FixedUpdate()
    {
        Debug.Log(rb.velocity.magnitude);

        if (GameManager.instance.gameState != GameManager.GameState.GAMEPLAY || GameManager.instance.holdGameplay) {
            rb.velocity = Vector2.zero;
            return;
        }

        if (rb.velocity.magnitude > constantVel)
        {
            rb.velocity -= rb.velocity.normalized * dampStrength * Time.fixedDeltaTime;
        }
        else if (rb.velocity.magnitude < constantVel)
        {
            rb.velocity = rb.velocity.normalized * constantVel;
        }
        if (distFromCenter + ballRadius > map.mapRadius)
        {
            float angle = Angle(transform.position.normalized);

            int alivePlayerID = (int)(angle / 360.0f * GameManager.instance.alivePlayers.Count);
            
            if (!GameManager.instance.OnSheildHit(alivePlayerID)) ResetBall();
        }
    }
    public static float Angle(Vector2 vector2)
    {
        float ret;

        if (vector2.x < 0)
        {
            ret = 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * -1 );
        }
        else
        {
            ret = Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
        }

        return 360 - ret;
    }

    // pee
}
