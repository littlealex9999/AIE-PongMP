using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    new public PongCircleCollider collider;

    public float constantSpd;
    public float minimumSpd = 0.5f;
    float targetSpd;
    public float dampStrength;
    [Range(0f, 1f), Tooltip("a value of 0 will have no effect. a value of 1 will make the ball go through the center every bounce")]
    public float shieldBounceTowardsCenterBias;

    public GameObject bouncePillar;
    public GameObject bounceShield;
    public GameObject bouncePaddle;
    public GameObject hitPaddle;
    public GameObject playerDies;

    float distFromCenter { get { return Vector2.Distance(transform.position, Vector2.zero); } }
    public float radius {
        get {
            return collider.radius;
        } set {
            collider.radius = value;
            transform.localScale = new Vector3(value * 2, value * 2, value * 2);
        }
    }

    bool held;

    public ParticleSystem smallRing; 
    public ParticleSystem mediumRing; 
    public ParticleSystem largeRing; 

    void Awake()
    {
        collider = GetComponent<PongCircleCollider>();
        collider.OnPaddleCollisionEnter += OnPaddleCollisionEnter;
        GameManager.instance.OnGameStateChange += OnGameStateChanged;
    }

    private void OnPaddleCollisionEnter(PongCollider other, CollisionData data)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            if (player.grabbing)
            {
                transform.SetParent(player.transform);
                StartCoroutine(player.GrabRoutine());
                player.heldBall = this;
                held = true;
            }
            else if (player.hitting)
            {
                PlayVFX(hitPaddle, data.collisionPos, player.color);
                EventManager.instance.ballHitEvent.Invoke();
                mediumRing.Play();
            }
            else
            {
                PlayVFX(bouncePaddle, data.collisionPos, player.color);
                EventManager.instance.ballBounceEvent.Invoke();
                smallRing.Play();
            }
        }
        else if (other.gameObject.CompareTag("Pillar"))
        {
            PlayVFX(bouncePillar,data.collisionPos, Color.white);
            EventManager.instance.ballHitPillarEvent.Invoke();
        }
    }

    private void OnGameStateChanged()
    {
        
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.gameState != GameManager.GameState.GAMEPLAY || GameManager.instance.holdGameplay || held) {
            collider.immovable = true;
            return;
        } else if (collider.immovable) {
            collider.immovable = false;
        }

        if (constantSpd < minimumSpd) targetSpd = minimumSpd;
        else targetSpd = constantSpd;

        DampVelocity();
        CheckIfHitBounds();

        transform.rotation = Quaternion.Euler(0, 0, Angle(collider.velocity));
    }

    private void DampVelocity()
    {
        if (collider.velocity.sqrMagnitude > constantSpd * constantSpd)
        {
            collider.velocity -= collider.velocity.normalized * dampStrength * Time.fixedDeltaTime;
        }
        else if (collider.velocity.sqrMagnitude < constantSpd * constantSpd)
        {
            collider.velocity = collider.velocity.normalized * targetSpd;
        }
    }

    void PlayVFX(GameObject particle, Vector3 pos)
    {
        Instantiate(particle, pos, Quaternion.Euler(Vector3.back));
    }
    void PlayVFX(GameObject particle, Vector3 pos, Color color)
    {
        GameObject obj = Instantiate(particle, pos, Quaternion.Euler(Vector3.back));
        VFXColorSetter vfxColorSetter = obj.GetComponent<VFXColorSetter>();

        ParticleSystem.MinMaxGradient startColor = new()
        {
            mode = ParticleSystemGradientMode.Color,
            color = color
        };
        vfxColorSetter.SetStartColor(startColor);
    }

    private void CheckIfHitBounds()
    {
        if (distFromCenter + radius > GameManager.instance.mapRadius)
        {
            float angle = Angle(transform.position.normalized);

            int alivePlayerID = (int)(angle / 360.0f * GameManager.instance.alivePlayers.Count);

            if (!GameManager.instance.OnShieldHit(alivePlayerID))
            {
                PlayVFX(bounceShield, transform.position, GameManager.instance.alivePlayers[alivePlayerID].color);
                mediumRing.Play();

                Vector2 shieldNormal = (Vector3.zero - transform.position).normalized;
                Vector2 forward = collider.velocity.normalized;
                Vector2 bounceDir = Vector2.Reflect(forward, shieldNormal).normalized;
                Vector2 finalBounceDir = Vector2.Lerp(bounceDir, shieldNormal, shieldBounceTowardsCenterBias).normalized;
                collider.velocity = finalBounceDir * targetSpd;
                transform.position = (Vector3.zero - (Vector3)shieldNormal) * (GameManager.instance.mapRadius - radius);
                transform.position = transform.position.normalized * (GameManager.instance.mapRadius - radius);
               
            }
            else // if player dies
            {
                PlayVFX(playerDies, transform.position, GameManager.instance.alivePlayers[alivePlayerID].color);
                largeRing.Play();
                return;
            }
            mediumRing.Play();
        }
    }

    public void Release()
    {
        if (!held) return;
        held = false;
        transform.parent = null;
        Vector2 dir = (Vector3.zero - transform.position).normalized;
        collider.velocity = dir * constantSpd;
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
}
