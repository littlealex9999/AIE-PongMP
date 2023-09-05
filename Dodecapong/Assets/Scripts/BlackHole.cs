using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float gravityStrength;
    public float duration = 10.0f;

    public float destroyTime = 2.0f;

    public bool pullEnabled = true;

    new PongCircleCollider collider;

    private void Start()
    {
        collider = GetComponent<PongCircleCollider>();
        collider.OnTrigger += CheckCollisionBall;
        EventManager.instance.blackHoleEvent.Invoke();
    }

    private void FixedUpdate()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            StartCoroutine(DestroyHole(null));
        }

        if (pullEnabled && GameManager.instance.gameState == GameManager.GameState.GAMEPLAY && !GameManager.instance.holdGameplay) {
            for (int i = 0; i < GameManager.instance.balls.Count; i++) {
                Vector2 deltaPos = transform.position - GameManager.instance.balls[i].transform.position;
                Vector2 gravity = deltaPos.normalized * (6.67f * GameManager.instance.balls[i].collider.mass * collider.mass / deltaPos.sqrMagnitude);
                GameManager.instance.balls[i].collider.velocity += gravity * gravityStrength;
            }
        }
    }

    void CheckCollisionBall(PongCollider other, CollisionData data)
    {
        if (other.GetComponent<Ball>()) {
            pullEnabled = false;

            other.gameObject.SetActive(false);
            other.transform.position = new Vector3(transform.position.x, transform.position.y, other.transform.position.z);
            other.velocity = Random.insideUnitCircle * GameManager.instance.ballPrefab.constantVel;

            StartCoroutine(DestroyHole(other.gameObject));
        }
    }

    IEnumerator DestroyHole(GameObject enableOnEnd)
    {
        float destroyTimer = 0.0f;
        Vector3 startScale = transform.localScale;
        while (destroyTimer < destroyTime) {
            destroyTimer += Time.deltaTime;
            float endPercentage = destroyTime / destroyTimer;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, endPercentage);

            yield return new WaitForEndOfFrame();
        }

        if (enableOnEnd) enableOnEnd.SetActive(true);

        // we make the assumption that this is true, as only one black hole should be able to spawn at a time
        GameManager.instance.blackHole = this;
        Destroy(gameObject);
        yield break;
    }
}
