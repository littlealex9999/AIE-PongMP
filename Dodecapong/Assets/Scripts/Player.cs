using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEditor;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Windows;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    #region Variables
    [HideInInspector] public int ID { get { return GameManager.instance.players.IndexOf(this); } private set { } }
    [HideInInspector] public int LivingID { get { return GameManager.instance.alivePlayers.IndexOf(this); } private set { } }
    [HideInInspector] public bool isAI { get; private set; }

    [HideInInspector] public Vector2 movementInput;

    [HideInInspector] public int shieldHealth;

    public bool dead;

    [HideInInspector] public Color color { get; private set; }
    [HideInInspector] public ParticleSystem.MinMaxGradient particleColor { get; private set; }
    [HideInInspector] public int colorIndex;

    [HideInInspector] public float dashDistance;
    [HideInInspector] public float dashDuration;
    [HideInInspector] public float dashCooldown;
    public bool dashEnabled;
    bool readyToDash = true;

    [HideInInspector] public float hitDuration;
    [HideInInspector] public float hitCooldown;
    bool readyToHit = true;

    public bool grabAttraction;
    public float grabAttractionForce;
    [HideInInspector] public float grabDuration;
    [HideInInspector] public float grabCooldown;
    [HideInInspector] public bool readyToGrab = true;

    new public PongConvexHullCollider collider;

    float playerMidPoint;
    float angleDeviance; // the max amount you can move from your starting rotation
    float angleDevianceCollider; // the amount that the collider affects the outcome of angleDeviance

    public float playerSectionMiddle { get { return playerMidPoint; } }
    public float playerAngleDeviance { get { return angleDeviance; } }


    [Tooltip("In degrees per second")] public float moveSpeed = 90;
    [Tooltip("In degrees per second")] public float dashMoveSpeed = 360;

    public float rotationalForce = 1.0f;
    public float pushDistance = 0.1f;
    [Range(0, 1)] public float deadzone = 0.01f;

    [HideInInspector] public Vector3 facingDirection = Vector3.right;

    public ParticleSystem grabParticles;
    public GameObject dashTrailObj;
    private TrailRenderer dashTrail;

    public AnimationCurve dashAnimationCurve;
    [HideInInspector] public bool dashing = false;

    public AnimationCurve hitAnimationCurve;
    [HideInInspector] public bool hitting = false;
    public float hitStrength;

    public Transform paddleFace;
    [HideInInspector] public Ball heldBall;
    [HideInInspector] public bool grabbing = false;
    [HideInInspector] public bool hitstunned = false;

    public ControllerInputHandler controllerHandler;

    [HideInInspector] public List<GameObject> healthBlips = new List<GameObject>();

    [SerializeField] private Animator animator;

    public MeshRenderer meshRenderer;

    public Transform inputGhost;
    Material ghostMat;
    float startTime;
    public float fadeDuration;
    public AnimationCurve fadeCurve;
    
    public bool pillairMarkers;
    public GameObject leftPilar;
    public GameObject rightPilar;

    public float ShieldHitImortalityDuration;
    #endregion

    #region Unity
    private void Awake()
    {
        dashTrail = dashTrailObj.GetComponentInChildren<TrailRenderer>();
        if (!dashTrail) Debug.LogError("dashTrailObj must have a TrailRenderer on a child object.");

        collider.OnPaddleCollisionEnter += OnCollisionEnterBall;

        ghostMat = inputGhost.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial;
    }

    public void SetupPlayer(Color playerColor, ParticleSystem.MinMaxGradient particleColor)
    {
        color = playerColor; ;
        this.particleColor = particleColor;
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (dead) return;

        if (pillairMarkers)
        {
            leftPilar.transform.position = GameManager.instance.pillars[LivingID].transform.position;

            if (GameManager.instance.pillars.Count - 1 == LivingID)
            {
                rightPilar.transform.position = GameManager.instance.pillars[0].transform.position;
            }
            else
            {
                rightPilar.transform.position = GameManager.instance.pillars[LivingID + 1].transform.position;
            }
        }

        float timeElapsed = Time.time - startTime;

        float alpha = Mathf.Lerp(0, 1, fadeCurve.Evaluate(timeElapsed / fadeDuration));

        ghostMat.color = new Color(ghostMat.color.r, ghostMat.color.g, ghostMat.color.b, alpha);

        if (isAI) {
           // CalculateAIInput();
        }

        Move();

        if (!grabAttraction) return;

        if (grabbing && heldBall == null && GameManager.instance.gameState == GameManager.GameState.GAMEPLAY && !GameManager.instance.holdGameplay) {
            for (int i = 0; i < GameManager.instance.balls.Count; i++) {
                Ball ball = GameManager.instance.balls[i];

                float distance = Vector3.Distance(ball.transform.position, transform.position);

                if (distance > 2.0f) continue;

                Vector2 deltaPos = transform.position - ball.transform.position;
                Vector2 gravity = deltaPos.normalized * (6.67f * ball.collider.mass * collider.mass / deltaPos.sqrMagnitude);
                ball.collider.velocity += gravity * grabAttractionForce;
            }
        }
    }
    #endregion

    #region Functions
    public void ResetStartValues()
    {
        startTime = Time.time;
    }

    public void Dash()
    {
        if (!isActiveAndEnabled || dead || !dashEnabled) return;
        StartCoroutine(DashRoutine());
    }

    public void Hit()
    {
        //StartCoroutine(HitRoutine());
    }

    public void Grab(InputAction.CallbackContext context)
    {
        if (dead) return;
        if (context.started) {
            grabParticles.gameObject.GetComponent<VFXColorSetter>().SetStartColor(color);
            grabParticles.gameObject.SetActive(true);
            grabbing = true;
        } else if (context.canceled) {
            grabParticles.gameObject.SetActive(false);
            //Release();
            grabbing = false;
        }
    }

    float CalculateMoveTarget()
    {
        float moveTarget;
        float movementInputAngle = Angle(movementInput);
        if (movementInputAngle < playerMidPoint - angleDeviance || movementInputAngle > playerMidPoint + angleDeviance) {
            // based on player midsection. move perpendicular to area
            moveTarget = Vector2.Dot(movementInput, Quaternion.Euler(0, 0, 90) * facingDirection);
        } else {
            // based on player position. move player to where joystick is pointing
            moveTarget = Vector2.Dot(movementInput, Quaternion.Euler(0, 0, 270) * transform.position.normalized);
        }

        if (moveTarget < deadzone && moveTarget > -deadzone) {
            collider.velocity = Vector2.zero;
            return 0;
        } else if (moveTarget > deadzone) {
            moveTarget = 1.0f;
        } else {
            moveTarget = -1.0f;
        }

        return moveTarget;
    }

    /// <summary>
    /// Use a Vector3 input from a controller and dot it with the direction this paddle is facing 
    /// to get a move input based on the direction that was input and the direction you can move
    /// </summary>
    /// <param name="clampSpeed"></param>
    public void Move(bool clampSpeed = true)
    {
        if (movementInput == Vector2.zero) {
            inputGhost.gameObject.SetActive(false);
            collider.velocity = Vector2.zero;
            return;
        } else if (GameManager.instance.holdGameplay && !(GameManager.instance.countdownTimer > 0) || hitstunned) {
            return;
        } else if (!inputGhost.gameObject.activeSelf) {
            inputGhost.gameObject.SetActive(true);
        }

        float moveTarget = CalculateMoveTarget();

        float speed;
        if (dashing) speed = dashMoveSpeed;
        else speed = moveSpeed;

        moveTarget *= movementInput.magnitude * speed;

        if (clampSpeed) moveTarget = Mathf.Clamp(moveTarget, -speed, speed);

        Vector3 startPos = transform.position;

        float ghostAngle = Angle(movementInput);

        transform.RotateAround(Vector3.zero, Vector3.back, moveTarget * Time.fixedDeltaTime);
        Vector3 targetPos = transform.position;

        float targetAngle = Angle(transform.position);
        float maxDev = playerMidPoint + angleDeviance - angleDevianceCollider;
        float minDev = playerMidPoint - angleDeviance + angleDevianceCollider;

        #region angle clamp
        if (targetAngle > maxDev || targetAngle < minDev) {
            if (playerMidPoint >= 180.0f) {
                float oppositePoint = playerMidPoint - 180.0f;
                if (targetAngle < oppositePoint || targetAngle > maxDev) {
                    // player is closer to max
                    SetPosition(maxDev);
                } else {
                    // player is closer to min
                    SetPosition(minDev);
                }
            } else {
                float oppositePoint = playerMidPoint + 180.0f;
                if (targetAngle < oppositePoint && targetAngle > maxDev) {
                    // player is closer to max
                    SetPosition(maxDev);
                } else {
                    // player is closer to min
                    SetPosition(minDev);
                }
            }
        }

        if (ghostAngle > maxDev || ghostAngle < minDev) {
            if (playerMidPoint >= 180.0f) {
                float oppositePoint = playerMidPoint - 180.0f;
                if (ghostAngle < oppositePoint || ghostAngle > maxDev) {
                    // ghost is closer to max
                    ghostAngle = maxDev;
                } else {
                    // ghost is closer to min
                    ghostAngle = minDev;
                }
            } else {
                float oppositePoint = playerMidPoint + 180.0f;
                if (ghostAngle < oppositePoint && ghostAngle > maxDev) {
                    // ghost is closer to max
                    ghostAngle = maxDev;
                } else {
                    // ghost is closer to min
                    ghostAngle = minDev;
                }
            }
        }
        #endregion

        inputGhost.position = GetPositionFromAngle(ghostAngle);
        inputGhost.rotation = Quaternion.Euler(0, 0, ghostAngle);
        Vector3 clampedPos = transform.position;

        // ensure we don't accidentally reverse the direction
        Vector3 deltaTarget = targetPos - startPos;
        Vector3 deltaPos = clampedPos - startPos;
        if (deltaPos.x < 0) deltaPos.x *= -1;
        if (deltaPos.y < 0) deltaPos.y *= -1;
        if (moveTarget < 0) moveTarget *= -1;

        // avoid divide by 0
        if (deltaTarget.x == 0 || deltaPos.x == 0) deltaPos.x = 0;
        else deltaPos.x = deltaTarget.x / deltaPos.x;
        if (deltaTarget.y == 0 || deltaPos.y == 0) deltaPos.y = 0;
        else deltaPos.y = deltaTarget.y / deltaPos.y;

        collider.velocity = deltaTarget * (deltaPos.magnitude / 1.4f) * (moveTarget / speed) * rotationalForce;

        if (hitting) {
            Vector2 hitVel = (Vector2)(Quaternion.Euler(0, 0, -targetAngle) * new Vector2(0, hitStrength));
            hitVel.y *= -1;
            collider.velocity += hitVel;
        }
    }

    public void CalculateLimits()
    {
        int alivePlayerCount = GameManager.instance.alivePlayers.Count;

        float segmentOffset = 180.0f / alivePlayerCount;

        Vector2 devianceMax = Vector2.zero;
        Vector2 devianceMin = Vector2.zero;
        for (int i = 0; i < collider.points.Length; i++) {
            if (collider.scaledPoints[i].x > devianceMax.x) devianceMax.x = collider.scaledPoints[i].x;
            if (collider.scaledPoints[i].y > devianceMax.y) devianceMax.y = collider.scaledPoints[i].y;
            if (collider.scaledPoints[i].x < devianceMin.x) devianceMin.x = collider.scaledPoints[i].x;
            if (collider.scaledPoints[i].y < devianceMin.y) devianceMin.y = collider.scaledPoints[i].y;
        }

        devianceMin.x += GameManager.instance.mapRadius;
        devianceMax.x += GameManager.instance.mapRadius;
        float angleColliderTotal = Angle(devianceMax) - Angle(devianceMin);

        playerMidPoint = 360.0f / alivePlayerCount * (LivingID + 1) + GameManager.instance.mapRotationOffset - segmentOffset;
        angleDeviance = segmentOffset;
        angleDevianceCollider = angleColliderTotal;

        // get the direction this paddle is facing, set its position, and have its rotation match
        facingDirection = Quaternion.Euler(0, 0, playerMidPoint) * -Vector3.up;
    }

    public void Resize(Vector3 size)
    {
        transform.localScale = size;
        collider.scale = new Vector2(size.y, size.x);
        collider.RecalculateScale();
        CalculateLimits();
    }

    void CalculateAIInput()
    {
        Vector2 targetBallPos = GameManager.instance.balls[0].collider.position;
        Vector2 targetBallVel = GameManager.instance.balls[0].collider.velocity;

        for (int i = 0; i < GameManager.instance.balls.Count; i++) {
            // select target ball based on heuristic
        }

        Vector2 intersectionPoint = GameManager.instance.GetCircleIntersection(targetBallPos, targetBallVel, GameManager.instance.mapRadius);
        float targetAngle = Angle(intersectionPoint);

        if (targetAngle < playerMidPoint - angleDeviance + angleDevianceCollider || targetAngle > playerMidPoint + angleDeviance - angleDevianceCollider) {
            movementInput = Vector2.zero;
        } else {
            float currentAngle = Angle(transform.position);

            if (targetAngle < currentAngle) {
                movementInput = Quaternion.Euler(0, 0, 90) * facingDirection;
            } else {
                movementInput = Quaternion.Euler(0, 0, -90) * facingDirection;
            }

            if (currentAngle > playerMidPoint + angleDeviance - angleDevianceCollider) {
                movementInput *= -1;
            }
        }
    }

    void OnCollisionEnterBall(PongCollider other, CollisionData data)
    {
        if (other.tag == "Ball") {
            if (controllerHandler != null) {
                controllerHandler.SetHaptics(GameManager.instance.paddleBounceHaptics);
            }

            Vector2 nextBallCollision = GameManager.instance.GetCircleIntersection(other.position, other.velocity, GameManager.instance.mapRadius);
            float nextBallCollisionAngle = Angle(nextBallCollision);
            if (nextBallCollisionAngle > playerMidPoint - angleDeviance && nextBallCollisionAngle < playerMidPoint + angleDeviance) {
                if (nextBallCollisionAngle < Angle(collider.position)) {
                    other.velocity = (GameManager.instance.GetTargetPointInCircle(playerMidPoint - angleDeviance) - other.position).normalized * other.velocity.magnitude;
                } else {
                    other.velocity = (GameManager.instance.GetTargetPointInCircle(playerMidPoint + angleDeviance) - other.position).normalized * other.velocity.magnitude;
                }
            }
        }
    }
    #endregion

    #region HelperFunctions
    public void SetPosition(float angle)
    {
        transform.position = GameManager.instance.GetTargetPointInCircle(angle).normalized * GameManager.instance.playerDistance;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }

    public Vector3 GetPositionFromAngle(float angle)
    {
        return GameManager.instance.GetTargetPointInCircle(angle).normalized * GameManager.instance.playerDistance;
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

    public void Release(Vector2 releaseVel)
    {
        if (heldBall) {
            if (heldBall.holdingPlayer != this) return;
            heldBall.HitVFX();
            heldBall.holdingPlayer = null;
            heldBall.transform.parent = null;
            heldBall.collider.velocity = releaseVel;

            heldBall = null;
            grabbing = false;
            readyToHit = true;
            animator.SetTrigger("Play Hit");

            //Hit();
        } else {
            grabbing = false;
        }
    }

    public void SetAI()
    {
        isAI = true;
    }
    #endregion

    #region Coroutines
    IEnumerator DashRoutine()
    {
        if (!readyToDash || dashing || movementInput == Vector2.zero) yield break;

        EventManager.instance.dashEvent.Invoke();

        readyToDash = false;
        dashing = true;

        dashTrail.enabled = false;
        dashTrailObj.transform.parent = transform;
        dashTrailObj.transform.localPosition = Vector3.zero;
        dashTrail.enabled = true;

        yield return new WaitForSeconds(dashDuration);

        dashing = false;

        dashTrailObj.transform.parent = null;

        yield return new WaitForSeconds(dashCooldown);

        readyToDash = true;

        /*
        float value;
        float timeElapsed = 0;

        float dashAngle = 360f / GameManager.instance.alivePlayers.Count * dashDistance;
        float startingAngle = Angle(transform.position);

        float dir = -CalculateMoveTarget();
        float targetAngle = startingAngle + dashAngle * dir;

        Angle(movementInput);

        float maxDev = playerMidPoint + angleDeviance - angleDevianceCollider;
        float minDev = playerMidPoint - angleDeviance + angleDevianceCollider;

        while (timeElapsed < dashDuration) {
            float currentAngle = Mathf.Lerp(targetAngle, startingAngle, dashAnimationCurve.Evaluate(timeElapsed / dashDuration));

            if (currentAngle > maxDev) SetPosition(maxDev);
            else if (currentAngle < minDev) SetPosition(minDev);
            else SetPosition(currentAngle);

            timeElapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();

            if (currentAngle > maxDev || currentAngle < minDev)
            {
                if (playerMidPoint >= 180.0f)
                {
                    float oppositePoint = playerMidPoint - 180.0f;
                    if (currentAngle < oppositePoint || currentAngle > maxDev)
                    {
                        // player is closer to max
                        SetPosition(maxDev);
                    }
                    else
                    {
                        // player is closer to min
                        SetPosition(minDev);
                    }
                }
                else
                {
                    float oppositePoint = playerMidPoint + 180.0f;
                    if (currentAngle < oppositePoint && currentAngle > maxDev)
                    {
                        // player is closer to max
                        SetPosition(maxDev);
                    }
                    else
                    {
                        // player is closer to min
                        SetPosition(minDev);
                    }
                }
            }
            else
            {
                SetPosition(currentAngle);
            }
            timeElapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();

            value = Mathf.Lerp(2, 1, dashAnimationCurve.Evaluate(timeElapsed / dashDuration));
            timeElapsed += Time.fixedDeltaTime;

            Move(dashInput * value, false);

            yield return new WaitForFixedUpdate();
        }

        dashing = false;

        dashTrailObj.transform.parent = null;

        yield return new WaitForSeconds(dashCooldown);

        readyToDash = true;
        */
    }

    IEnumerator HitRoutine()
    {
        if (!readyToHit || hitting || grabbing) yield break;

        readyToHit = false;
        hitting = true;
        collider.addForceWhileImmovable = true;

        animator.SetTrigger("Play Hit");

        yield return new WaitForSeconds(hitDuration);

        collider.addForceWhileImmovable = false;
        hitting = false;

        yield return new WaitForSeconds(hitCooldown);

        readyToHit = true;
    }

    public void Grab(CollisionData data) => StartCoroutine(CR_Grab(data));

    IEnumerator CR_Grab(CollisionData data)
    {
        if (!readyToGrab) yield break;
        readyToGrab = false;

        EventManager.instance.ballGrabEvent.Invoke();

        float timeElapsed = 0;

        while (timeElapsed < grabDuration) {
            timeElapsed += Time.deltaTime;
            if (!grabbing) break;

            yield return new WaitForEndOfFrame();
        }

        Vector2 hitVel = heldBall.collider.velocity + -(Vector2)transform.position.normalized * hitStrength * heldBall.collider.velocity.magnitude;
        Vector2 lobVel = -(Vector2)transform.position.normalized * heldBall.constantSpd;
        Release(Vector2.Lerp(hitVel, lobVel, timeElapsed / grabDuration));

        if (grabParticles.isPlaying) grabParticles.gameObject.SetActive(false);
        EventManager.instance.ballHitEvent.Invoke();

        yield return new WaitForSeconds(grabCooldown);

        readyToGrab = true;
    }

    public void UnHittable() => StartCoroutine(CR_UnHittable());
    private IEnumerator CR_UnHittable()
    {
        if (collider.enabled == false) yield break;

        collider.enabled = false;

        yield return new WaitForSeconds(ShieldHitImortalityDuration);

        collider.enabled = true;
    }
    #endregion
}
