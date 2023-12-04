using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public GameObject pointer;

    public float gravityStrength;
    public float duration = 10.0f;

    public float buildupTime = 2.0f;

    public float destroyTime = 2.0f;
    public float ballLaunchMult = 4.0f;

    public bool pullEnabled = true;

    new PongCircleCollider collider;
    Ball ball;

    public Animator animator;

    private void Start()
    {
        collider = GetComponent<PongCircleCollider>();
        collider.OnTrigger += CheckCollisionBall;
    }

    private void FixedUpdate()
    {
        if (!ball) 
        {
            duration -= Time.fixedDeltaTime;
            if (duration <= 0)
            {
                StartCoroutine(DestroyHole());
            }

            if (pullEnabled && GameManager.instance.gameState == GameManager.GameState.GAMEPLAY && !GameManager.instance.holdGameplay) 
            {
                for (int i = 0; i < GameManager.instance.balls.Count; i++) 
                {
                    Vector2 deltaPos = transform.position - GameManager.instance.balls[i].transform.position;
                    Vector2 gravity = deltaPos.normalized * (6.67f * GameManager.instance.balls[i].collider.mass * collider.mass / deltaPos.sqrMagnitude);
                    GameManager.instance.balls[i].collider.velocity += gravity * gravityStrength;
                }
            }
        }
    }

    void CheckCollisionBall(PongCollider other, CollisionData data)
    {
        if (other.TryGetComponent(out ball))
        {
            pullEnabled = false;

            collider.enabled = false;

            other.gameObject.SetActive(false);
            other.transform.position = new Vector3(transform.position.x, transform.position.y, other.transform.position.z);
            other.velocity = Random.insideUnitCircle * ball.constantSpd * ballLaunchMult;

            GameManager.instance.CleanTransformers(false);
            EventManager.instance.ballHitBlackHoleEvent.Invoke();

            animator.SetTrigger("BallHit");

            StartCoroutine(BallHit());
        }
    }

    IEnumerator DestroyHole()
    {
        animator.SetTrigger("Shrink");
        yield return new WaitForSeconds(0.6f);
        GameManager.instance.blackHole = null;
        DestroyImmediate(gameObject);
    }

    IEnumerator BallHit()
    {
        yield return new WaitForSeconds(destroyTime);

        if (ball) ball.gameObject.SetActive(true);
        if (ball) ball.largeRing.Play();

        StartCoroutine(DestroyHole());
    }

    IEnumerator wwwDestroyHole(GameObject enableOnEnd)
    {
        float destroyTimer = 0.0f;
        Vector3 startScale = transform.localScale;

        while (destroyTimer < destroyTime) {
            destroyTimer += Time.deltaTime;
            float endPercentage = destroyTime / destroyTimer;

            //transform.localScale = Vector3.Lerp(startScale, Vector3.zero, endPercentage);

            if (pointer && ball) {
                pointer.SetActive(true);
                pointer.transform.rotation = Quaternion.Euler(0, 0, Player.Angle(ball.collider.velocity));
            }

            yield return new WaitForEndOfFrame();
        }

        if (enableOnEnd) yield return new WaitForSeconds(0.1f);

        if (enableOnEnd) enableOnEnd.SetActive(true);
        if (ball) ball.largeRing.Play();
        if (!enableOnEnd) animator.SetTrigger("Shrink");

        GameManager.instance.blackHole = null;
        DestroyImmediate(gameObject);
        yield break;
    }
}
