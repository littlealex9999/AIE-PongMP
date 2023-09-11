using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    #region Variables
    [HideInInspector] public int ID { get { return GameManager.instance.players.IndexOf(this); } private set { } }
    [HideInInspector] public int LivingID { get { return GameManager.instance.alivePlayers.IndexOf(this); } private set { } }

    [HideInInspector] public Vector2 movementInput;

    [HideInInspector] public int shieldHealth;

    [HideInInspector] public Color color;

    [HideInInspector] public float dashDuration;
    [HideInInspector] public float dashCooldown;
    bool readyToDash = true;

    [HideInInspector] public float hitDuration;
    [HideInInspector] public float hitCooldown;
    bool readyToHit = true;

    [HideInInspector] public float grabDuration;
    [HideInInspector] public float grabCooldown;
    bool readyToGrab = true;

    new public PongConvexHullCollider collider;

    float playerMidPoint;
    float angleDeviance; // the max amount you can move from your starting rotation

    public float playerSectionMiddle { get { return playerMidPoint; } }

    [Tooltip("In degrees per second")] public float moveSpeed = 90;

    public float rotationalForce = 1.0f;
    public float pushDistance = 0.1f;
    public float pushStrength = 3.0f;

    [HideInInspector] public Vector3 facingDirection = Vector3.right;

    public GameObject dashTrailObj;
    private TrailRenderer dashTrail;
    public AnimationCurve dashAnimationCurve;
    [HideInInspector] public bool dashing = false;

    public AnimationCurve hitAnimationCurve;
    [HideInInspector] public bool hitting = false;
    [HideInInspector] public float hitStrength;

    [HideInInspector] public Ball heldBall;
    [HideInInspector] public bool grabbing = false;
    #endregion

    #region UnityMessages
    private void Awake()
    {
        dashTrail = dashTrailObj.GetComponentInChildren<TrailRenderer>();
        if (!dashTrail) Debug.LogError("dashTrailObj must have a TrailRenderer on a child object."); 
    }
    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        Move(movementInput);
    }
    #endregion

    #region Functions
    public void Dash()
    {
        StartCoroutine(DashRoutine());
    }

    public void Hit()
    {
        StartCoroutine(HitRoutine());
    }

    public void Grab(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            grabbing = true;
        }
        else if (context.canceled)
        {
            Release();
        }
    }

    /// <summary>
    /// Use a Vector3 input from a controller and dot it with the direction this paddle is facing 
    /// to get a move input based on the direction that was input and the direction you can move
    /// </summary>
    /// <param name="input"></param>
    /// <param name="clampSpeed"></param>
    public void Move(Vector2 input, bool clampSpeed = true)
    {
        if (input == Vector2.zero) {
            collider.velocity = Vector2.zero;
            return;
        } else if (GameManager.instance.holdGameplay) {
            return;
        }

        float moveTarget = Vector2.Dot(input, Quaternion.Euler(0, 0, 90) * facingDirection) * input.magnitude * moveSpeed;
        if (clampSpeed) moveTarget = Mathf.Clamp(moveTarget, -moveSpeed, moveSpeed);

        Vector3 startPos = transform.position;

        transform.RotateAround(Vector3.zero, Vector3.back, moveTarget * Time.fixedDeltaTime);
        Vector3 targetPos = transform.position;

        float limit = 0;

        float maxDev = playerMidPoint + angleDeviance - limit;
        float minDev = playerMidPoint - angleDeviance + limit;
        float angle = Angle(transform.position);

        if (angle > maxDev || angle < minDev)
        {
            float lowComparison = Mathf.Abs(360 - angle);
            float lowExtraComparison = Mathf.Abs(minDev - angle);
            if (lowExtraComparison < lowComparison) lowComparison = lowExtraComparison;

            if (maxDev >= 360) maxDev -= 360;
            float highComparison = Mathf.Abs(maxDev - angle);

            if (lowComparison < highComparison)
            {
                SetPosition(minDev);
            }
            else
            {
                SetPosition(maxDev);
            }
        }

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

        collider.velocity = deltaTarget * (deltaPos.magnitude / 1.4f) * (moveTarget / moveSpeed) * rotationalForce;

        if (hitting)
        {
            Vector2 hitVel = (Vector2)(Quaternion.Euler(0, 0, -angle) * new Vector2(0, hitStrength));
            hitVel.y *= -1;
            collider.velocity += hitVel;
        }
    }

    public void CalculateLimits()
    {
        // the "starting position" is as follows, with 2 players as an example:
        // 360 / player count to get the base angle (360 / 2 = 180)
        // ... * i + 1 to get a multiple of the base angle based on the player (180 * (0 + 1) = 180)
        // ... + mapRotationOffset to ensure the paddles spawn relative to the way the map is rotated (+ 0 in example, so ignored)
        // 360 / (playerCount * 2) to get the offset of the middle of each player area (360 / (2 * 2) = 90)
        // (player position - segment offset) to get the correct position to place the player (180 - 90 = 90)
        int alivePlayerCount = GameManager.instance.alivePlayers.Count;

        float segmentOffset = 180.0f / alivePlayerCount;

        playerMidPoint = 360.0f / alivePlayerCount * (LivingID + 1) + GameManager.instance.mapRotationOffset - segmentOffset;
        angleDeviance = segmentOffset;

        // get the direction this paddle is facing, set its position, and have its rotation match
        facingDirection = Quaternion.Euler(0, 0, playerMidPoint) * -Vector3.up;
    }
    #endregion

    #region HelperFunctions
    public void SetPosition(float angle)
    {
        transform.position = GameManager.instance.map.GetTargetPointInCircleLocal(angle).normalized * GameManager.instance.playerDistance;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }

    public static float Angle(Vector2 vector2)
    {
        float ret;

        if (vector2.x < 0)
        {
            ret = 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            ret = Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
        }

        return 360 - ret;
    }

    public void Release()
    {
        if (heldBall)
        {
            heldBall.Release();
            heldBall = null;
            grabbing = false;
            readyToHit = true;
            Hit();
        }
        else
        {
            grabbing = false;
        }
    }
    #endregion

    #region Coroutines
    IEnumerator DashRoutine()
    {
        if (!readyToDash || dashing || movementInput == Vector2.zero) yield break;

        Vector2 dashInput = movementInput;

        EventManager.instance.dashEvent.Invoke();

        readyToDash = false;
        dashing = true;

        dashTrail.enabled = false;
        dashTrailObj.transform.parent = transform;
        dashTrailObj.transform.localPosition = Vector3.zero;
        dashTrail.enabled = true;

        float value;
        float timeElapsed = 0;

        while (timeElapsed < dashDuration)
        {
            value = Mathf.Lerp(2, 1, dashAnimationCurve.Evaluate(timeElapsed / dashDuration));
            timeElapsed += Time.fixedDeltaTime;

            Move(dashInput * value, false);

            yield return new WaitForFixedUpdate();
        }

        dashing = false;

        dashTrailObj.transform.parent = null;

        yield return new WaitForSeconds(dashCooldown);

        readyToDash = true;
    }

    IEnumerator HitRoutine()
    {
        if (!readyToHit || hitting || grabbing) yield break;

        readyToHit = false;
        hitting = true;
        collider.addForceWhileImmovable = true;

        float value;
        float timeElapsed = 0;
        Vector3 startingScale = transform.localScale;
        Vector2 colliderStart = collider.scale;

        while (timeElapsed < hitDuration)
        {
            value = Mathf.Lerp(startingScale.x, startingScale.x * 2, hitAnimationCurve.Evaluate(timeElapsed / hitDuration));
            timeElapsed += Time.fixedDeltaTime;

            transform.localScale = new Vector3(value, startingScale.y, startingScale.z);
            collider.scale = new Vector2(transform.localScale.y, transform.localScale.x);
            collider.RecalculateScale();

            yield return new WaitForFixedUpdate();
        }

        transform.localScale = startingScale;
        collider.scale = colliderStart;
        collider.RecalculateScale();

        collider.addForceWhileImmovable = false;
        hitting = false;

        yield return new WaitForSeconds(hitCooldown);

        readyToHit = true;
    }

    public IEnumerator GrabRoutine()
    {
        if (!readyToGrab) yield break;

        EventManager.instance.ballGrabEvent.Invoke();

        readyToGrab = false;

        float timeElapsed = 0;

        yield return new WaitUntil(() => !grabbing || (timeElapsed += Time.fixedDeltaTime) >= grabDuration);

        Release();

        yield return new WaitForSeconds(grabDuration);

        readyToGrab = true;
    }
    #endregion
}
