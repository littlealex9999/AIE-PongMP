using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ball : MonoBehaviour
{
    Rigidbody2D rb;

    public Map map;
    
    public float constantVel;
    public float ballRadius;
    public float dampStrength;
    [Range(0f, 1f)]
    public float bounceTowardsCenterBias;

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

    private void BounceOnBounds()
    {
        Vector2 forward = rb.velocity.normalized;
        Vector2 normalToCenter = (Vector3.zero - transform.position).normalized;
        Vector2 bounceDir = Vector2.Reflect(forward, normalToCenter).normalized;
        Vector2 finalBounceDir = Vector2.Lerp(bounceDir, normalToCenter, bounceTowardsCenterBias).normalized;
        rb.velocity = finalBounceDir * constantVel;
        rb.position += normalToCenter * 0.1f;
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
        if (GameManager.instance.gameState != GameManager.GameState.GAMEPLAY) return;

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

            int alivePlayerID = (int)(angle / 360.0f * GameManager.instance.alivePlayerCount);
            
            if (GameManager.instance.OnSheildHit(alivePlayerID)) ResetBall();
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
}
