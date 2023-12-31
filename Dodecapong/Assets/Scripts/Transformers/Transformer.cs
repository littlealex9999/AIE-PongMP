using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transformer : MonoBehaviour
{
    public bool limitedTime = true;
    public float despawnTime = 15.0f;
    public float duration = 20.0f;

    [System.Flags]
    public enum TransformerTypes
    {
        BALLSPEED       = 1 << 0,
        BALLSIZEUP      = 1 << 1,
        BALLSIZEDOWN    = 1 << 2,
        BLACKHOLE       = 1 << 3,
    }

    public abstract TransformerTypes GetTransformerType();
    public abstract void ApplyModifier();
    protected abstract void RemoveModifier();

    private void Start()
    {
        GetComponent<PongCollider>().OnTrigger += CheckIfCollidingBall;
    }

    void CheckIfCollidingBall(PongCollider other, CollisionData data)
    {
        if (other.TryGetComponent(out Ball ball)) 
        {
            ball.smallRing.Play();

            GameManager.instance.spawnedTransformers.Remove(this);
            GameManager.instance.activeTransformers.Add(this);
            ApplyModifier();

            gameObject.SetActive(false);
        }
    }

    public void EndModifier()
    {
        GameManager.instance.activeTransformers.Remove(this);
        RemoveModifier();

        Destroy(gameObject);
    }
}
