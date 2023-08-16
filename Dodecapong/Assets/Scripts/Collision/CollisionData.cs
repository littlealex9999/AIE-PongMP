using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionData
{
    public readonly float depth = -1.0f;
    public Vector2 normal { get; private set; } // from a to b
    public readonly Vector2 collisionPos;

    public readonly PongCollider colliderA;
    public readonly PongCollider colliderB;

    public CollisionData(PongCollider colliderA, PongCollider colliderB, float depth, Vector2 normal, Vector2 collisionPos)
    {
        this.colliderA = colliderA;
        this.colliderB = colliderB;

        this.depth = depth;
        this.normal = normal;
        this.collisionPos = collisionPos;
    }

    public bool isColliding { get { return depth > 0.0f; } }

    public void ResolveCollision()
    {
        PongCollider primaryCollider;
        PongCollider secondaryCollider;

        if (colliderB.immovable) {
            if (colliderA.immovable) return; // both colliders are immovable, and cannot be resovled

            primaryCollider = colliderB;
            secondaryCollider = colliderA;
            normal *= -1;
        } else {
            primaryCollider = colliderA;
            secondaryCollider = colliderB;
        }

        secondaryCollider.position += depth * (Vector3)normal;
    }
}
