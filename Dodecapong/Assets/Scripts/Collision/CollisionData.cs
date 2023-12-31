using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionData
{
    public readonly float depth = -1.0f;
    public Vector2 normal { get; private set; } // from a to b
    public Vector2 forceResolutionNormal { get; private set; }
    public readonly Vector2 collisionPos;

    public readonly PongCollider colliderA;
    public readonly PongCollider colliderB;

    #region Constructors
    public CollisionData(PongCollider colliderA, PongCollider colliderB, float depth, Vector2 normal, Vector2 collisionPos)
    {
        this.colliderA = colliderA;
        this.colliderB = colliderB;

        this.depth = depth;
        this.normal = normal;
        forceResolutionNormal = normal;
        this.collisionPos = collisionPos;
    }

    public CollisionData(PongCollider colliderA, PongCollider colliderB, float depth, Vector2 normal, Vector2 forceResolutionNormal, Vector2 collisionPos)
    {
        this.colliderA = colliderA;
        this.colliderB = colliderB;

        this.depth = depth;
        this.normal = normal;
        this.forceResolutionNormal = forceResolutionNormal;
        this.collisionPos = collisionPos;
    }
    #endregion

    public bool isColliding { get { return depth > 0.0f; } }

    public void ResolveCollision()
    {
        // SOLVE DEPENETRATION
        float movementOnA;
        float movementOnB;
        float totalIMass = colliderA.inverseMass + colliderB.inverseMass;

        if (colliderA.immovable) {
            if (colliderB.immovable) return; // both objects are immovable

            // only A is immovable
            movementOnA = 0.0f;
            movementOnB = 1.0f;
        } else if (colliderB.immovable) {
            // only B is immovable
            movementOnB = 0.0f;
            movementOnA = 1.0f;
        } else {
            // neither is immovable, solve using relative mass
            movementOnA = colliderA.inverseMass / totalIMass;
            movementOnB = colliderB.inverseMass / totalIMass;
        }

        Vector2 movement = depth * normal;

        colliderA.position -= (Vector3)movement * movementOnA;
        colliderB.position += (Vector3)movement * movementOnB;


        // SOLVE VELOCITY
        float j = 2 * Vector2.Dot(colliderA.velocity - colliderB.velocity, forceResolutionNormal) / totalIMass;
        float startingMag;
        if (!colliderA.addForceWhileImmovable && colliderA.immovable) {
            startingMag = colliderB.velocity.magnitude;
        } else {
            startingMag = colliderA.velocity.magnitude;
        }

        // both colliders cannot be immovable as we return before this if that is the case
        if (colliderA.immovable) {
            if (j < 0) j *= -1;
            colliderB.ApplyImpulse(2 * j * forceResolutionNormal);
            if (!colliderA.addForceWhileImmovable) {
                colliderB.velocity = colliderB.velocity.normalized * startingMag;
            }
        } else if (colliderB.immovable) {
            if (j < 0) j *= -1;
            colliderA.ApplyImpulse(2 * j * -forceResolutionNormal);
            if (!colliderB.addForceWhileImmovable) {
                colliderA.velocity = colliderA.velocity.normalized * startingMag;
            }
        } else {
            colliderA.ApplyImpulse(j * -forceResolutionNormal);
            colliderB.ApplyImpulse(j * forceResolutionNormal);
        }
    }

    public void SetForceNormal(Vector2 norm)
    {
        forceResolutionNormal = norm;
    }
}
