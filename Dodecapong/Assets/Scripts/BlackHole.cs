using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float gravityStrength;
    float duration = 10.0f;

    public bool pullEnabled = true;

    new PongCircleCollider collider;

    private void Start()
    {
        collider = GetComponent<PongCircleCollider>();
        collider.OnTrigger += CheckCollisionBall;
    }

    private void FixedUpdate()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            // stop
        }

        if (pullEnabled && GameManager.instance.gameState == GameManager.GameState.GAMEPLAY && !GameManager.instance.holdGameplay) {
            Vector2 deltaPos = transform.position - GameManager.instance.ball.transform.position;
            Vector2 gravity = deltaPos.normalized * (6.67f * GameManager.instance.ball.collider.mass * collider.mass / deltaPos.sqrMagnitude);
            GameManager.instance.ball.collider.velocity += gravity * gravityStrength;
        }
    }

    void CheckCollisionBall(PongCollider other)
    {
        if (other.GetComponent<Ball>()) {
            pullEnabled = false;
            // shoot out ball
        }
    }
}
