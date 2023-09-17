using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transformer : MonoBehaviour
{
    public bool limitedTime = true;
    public float duration = 20.0f;

    [System.Flags]
    public enum TransformerTypes
    {
        BALLSPEED       = 1 << 0,
        PLAYERSPEED     = 1 << 1,
        BALLSIZE        = 1 << 2,
        PLAYERSIZE      = 1 << 3,
        SHIELDHEALTH    = 1 << 4,
        BLACKHOLE       = 1 << 5,
        DASHCOOLDOWN    = 1 << 6,
        BALLCOUNT       = 1 << 7,
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
