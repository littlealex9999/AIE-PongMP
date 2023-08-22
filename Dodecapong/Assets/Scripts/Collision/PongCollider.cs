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

    public bool immovable = false;

    private void Start()
    {
        CollisionSystem.AddCollider(this);
    }

    private void OnDestroy()
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
