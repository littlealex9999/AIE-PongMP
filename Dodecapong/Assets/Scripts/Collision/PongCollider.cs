using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PongCollider : MonoBehaviour
{
    public abstract CollisionSystem.ColliderTypes Type();

    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    
    [HideInInspector] public float inverseMass = 1;
    public float mass { get { return 1 / inverseMass; } set { inverseMass = 1 / value; } }

    public Vector2 velocity;
    public Vector2 acceleration;
    public float normalBending = 1.0f;

    public bool trigger = false;
    public bool immovable = false;
    public bool addForceWhileImmovable = true;

    public delegate void CollisionEvents(PongCollider other, CollisionData data);
    public CollisionEvents OnCollision;
    public CollisionEvents OnPaddleCollision;
    public CollisionEvents OnTrigger;
    public CollisionEvents OnPaddleTrigger;

    private void Start()
    {
        StartEvents();
    }

    private void OnDestroy()
    {
        DestroyEvents();
    }

    protected virtual void StartEvents()
    {
        CollisionSystem.AddCollider(this);
    }

    protected virtual void DestroyEvents()
    {
        CollisionSystem.RemoveCollider(this);
    }

    public void ApplyImpulse(Vector2 force) => velocity += inverseMass * force;

    public void CollisionUpdate(float delta)
    {
        velocity += acceleration * delta;

        if (!immovable) {
            position += (Vector3)(velocity * delta);
        }
    }
}
