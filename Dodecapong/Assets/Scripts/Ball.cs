using UnityEngine;
using static GameManager;

public class Ball : MonoBehaviour
{
    Rigidbody2D rb;

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
        instance.gameStateChanged.AddListener(OnGameStateChanged);
    }

    private void OnGameStateChanged()
    {
        if (instance.gameState == GameState.MAINMENU)
        {
            rb.velocity = rb.transform.forward * constantVel;
        }
    }
    private void Bounce(float centerBias, Vector2 bounceNormal)
    {
        Vector2 forward = rb.velocity.normalized;
        Vector2 bounceDir = Vector2.Reflect(forward, bounceNormal).normalized;
        Vector2 finalBounceDir = Vector2.Lerp(bounceDir, bounceNormal, centerBias).normalized;
        rb.velocity = finalBounceDir * constantVel;
        rb.position += bounceNormal * 0.1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Paddle paddle;
        if (collision.gameObject.TryGetComponent(out paddle))
        {
            Bounce(paddleBounceTowardsCenterBias, paddle.BounceNormal());
        }
    }

    private void BounceOnBounds()
    {
        Vector2 shieldNormal = (Vector3.zero - transform.position).normalized;
        Bounce(shieldBounceTowardsCenterBias, shieldNormal);
    }
    private void ResetBall()
    {
        rb.position = Vector2.zero;
        rb.velocity = Random.insideUnitCircle.normalized * constantVel;
    }

    public void AddVelocity(Vector2 velocity)
    {
        rb.velocity += velocity;
    }

    private void FixedUpdate()
    {
        if (instance.gameState != GameState.GAMEPLAY || instance.holdGameplay) {
            rb.velocity = Vector2.zero;
            return;
        }

        if (rb.velocity.magnitude > constantVel)
        {
            rb.velocity -= Vector2.one * dampStrength * Time.fixedDeltaTime;
        }
        else if (rb.velocity.magnitude < constantVel)
        {
            rb.velocity = Vector2.one * constantVel;
        }
        if (distFromCenter + ballRadius > map.mapRadius)
        {
            float angle = Angle(transform.position.normalized);

            int alivePlayerID = (int)(angle / 360.0f * instance.alivePlayers.Count);
            
            if (instance.OnSheildHit(alivePlayerID)) ResetBall();
            else BounceOnBounds();
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
