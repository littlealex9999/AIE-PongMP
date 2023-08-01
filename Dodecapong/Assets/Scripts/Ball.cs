using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody2D rb;

    public Map map;
    
    public float constantVel;
    public float ballRadius;
    public float dampStrength;

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
        rb.velocity = Random.insideUnitCircle.normalized * constantVel;
    }

    private void BounceOnBounds()
    {
        if ((transform.position - map.transform.position).sqrMagnitude > map.mapRadius * map.mapRadius) 
        {
            transform.position = (transform.position - map.transform.position).normalized * map.mapRadius;
        }

        Vector3 forward = rb.velocity.normalized;
        Vector3 normal = (Vector3.zero - transform.position).normalized;
        rb.velocity = Vector3.Reflect(forward, normal) * constantVel;
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
            Vector2 targetVec = transform.position.normalized;
            float angle = Angle(targetVec);// Mathf.Atan2(targetVec.x, targetVec.y);
            int alivePlayerCount = GameManager.instance.alivePlayerCount;

            float playerSector = 360.0f / alivePlayerCount;

            for (int i = 0; i < alivePlayerCount; i++)
            {
                if (angle > playerSector * i && angle < playerSector * i + playerSector)
                {
                    if (map.ShieldHit(i))
                    {
                        BounceOnBounds();
                    }
                    else
                    {
                        ResetBall();
                    }
                }
            }
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
