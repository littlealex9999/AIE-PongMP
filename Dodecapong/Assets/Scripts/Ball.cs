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
            BounceOnBounds();
        }
    }
}
