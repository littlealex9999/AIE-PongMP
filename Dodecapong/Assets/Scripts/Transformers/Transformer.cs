using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transformer : MonoBehaviour
{
    public bool limitedTime = true;
    public float duration = 20.0f;

    public abstract void ApplyModifier();
    protected abstract void RemoveModifier();

    private void Start()
    {
        GetComponent<PongCollider>().OnTrigger += CheckIfCollidingBall;
    }

    void CheckIfCollidingBall(PongCollider other)
    {
        if (other.GetComponent<Ball>()) {
            ApplyModifier();
            GameManager.instance.activeTransformers.Add(this);

            gameObject.SetActive(false);
        }
    }

    public void EndModifier()
    {
        RemoveModifier();
        GameManager.instance.activeTransformers.Remove(this);

        Destroy(gameObject);
    }
}
