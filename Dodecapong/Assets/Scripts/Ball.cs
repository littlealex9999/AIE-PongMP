using UnityEngine;

public class Ball : MonoBehaviour
{

    new PongCollider collider;

    public Map map;

    public float constantVel;
    public float ballRadius;
    public float dampStrength;
    [Range(0f, 1f), Tooltip("a value of 0 will have no effect. a value of 1 will make the ball go through the center every bounce")]
    public float shieldBounceTowardsCenterBias;
    [Range(0f, 1f), Tooltip("a value of 0 will have no effect. a value of 1 will make the ball go through the center every bounce")]
    public float paddleBounceTowardsCenterBias;

    float distFromCenter { get { return Vector2.Distance(transform.position, Vector2.zero); } }

    public float countdownTimer;
    float currentCountdownTime;
    bool reset;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<PongCollider>();
        collider.OnPaddleCollision += OnPaddleCollision;
        GameManager.instance.OnGameStateChange += OnGameStateChanged;
    }

    private void OnPaddleCollision(PongCollider other)
    {
        if (other.gameObject.TryGetComponent(out Paddle paddle))
        {
            if (paddle.hitting)
            {
                EventManager.instance.ballHitEvent.Invoke();
            }
            else
            {
                EventManager.instance.ballBounceEvent.Invoke();
            }
        }
    }

    private void OnGameStateChanged()
    {
        
    }

    private void Bounce(float centerBias, Vector2 bounceNormal)
    {
        Vector2 forward = collider.velocity.normalized;
        Vector2 bounceDir = Vector2.Reflect(forward, bounceNormal).normalized;
        Vector2 finalBounceDir = Vector2.Lerp(bounceDir, bounceNormal, centerBias).normalized;
        collider.velocity = finalBounceDir * constantVel;
        transform.position += (Vector3)(bounceNormal * 0.1f);
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.gameState != GameManager.GameState.GAMEPLAY || GameManager.instance.holdGameplay)
        {
            collider.velocity = Vector2.zero;
            return;
        }

        if (currentCountdownTime > 0)
        {
            currentCountdownTime -= Time.fixedDeltaTime;
            return;
        }
        else if (reset)
        {
            int player = Random.Range(0, GameManager.instance.alivePlayers.Count);

            Vector2 dir = (GameManager.instance.alivePlayers[player].paddle.transform.position - transform.position).normalized;

            collider.velocity = dir * constantVel;

            reset = false;
        }

        if (collider.velocity.sqrMagnitude > constantVel * constantVel)
        {
            collider.velocity -= collider.velocity.normalized * dampStrength * Time.fixedDeltaTime;
        } 
        else if (collider.velocity.sqrMagnitude < constantVel * constantVel)
        {
            collider.velocity = collider.velocity.normalized * constantVel;
        }

        if (distFromCenter + ballRadius > map.mapRadius) {
            float angle = Angle(transform.position.normalized);

            int alivePlayerID = (int)(angle / 360.0f * GameManager.instance.alivePlayers.Count);

            if (!GameManager.instance.OnSheildHit(alivePlayerID)) {
                BounceOnBounds();
                transform.position = transform.position.normalized * (map.mapRadius - ballRadius);
            }
        }

        transform.rotation = Quaternion.Euler(0, 0, Angle(collider.velocity));
    }

    private void BounceOnBounds()
    {
        Vector2 shieldNormal = (Vector3.zero - transform.position).normalized;
        Bounce(shieldBounceTowardsCenterBias, shieldNormal);
    }

    public void ResetBall()
    {
        EventManager.instance.ballCountdownEvent.Invoke();

        currentCountdownTime = countdownTimer;

        transform.position = Vector2.zero;

        reset = true;
    }

    public void AddVelocity(Vector2 velocity)
    {
        collider.velocity += velocity;
    }

    public static float Angle(Vector2 vector2)
    {
        float ret;

        if (vector2.x < 0) {
            ret = 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * -1);
        } else {
            ret = Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
        }

        return 360 - ret;
    }

    // pee
}
