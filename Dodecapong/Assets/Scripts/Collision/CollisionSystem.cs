using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem : MonoBehaviour
{
    public enum ColliderTypes
    {
        CIRCLE,
        RECTANGLE,
        CONVEXHULL,
    }

    static List<PongCollider> colliders = new List<PongCollider>();

    public static void AddCollider(PongCollider collider) => colliders.Add(collider);
    public static void RemoveCollider(PongCollider collider) => colliders.Remove(collider);

    private void FixedUpdate()
    {
        for (int i = 0; i < colliders.Count; i++) {
            colliders[i].CollisionUpdate(Time.fixedDeltaTime);
        }

        for (int i = 0; i < colliders.Count; i++) {
            for (int j = i + 1; j < colliders.Count; j++) {
                CollisionData data = CheckCollision(colliders[i], colliders[j]);
                if (data != null && data.isColliding) {
                    data.ResolveCollision();
                }
            }
        }
    }

    #region Collisions
    public static CollisionData CheckCollision(PongCollider colliderA, PongCollider colliderB)
    {
        switch (colliderA.Type()) {
            case ColliderTypes.CIRCLE:
                switch (colliderB.Type()) {
                    case ColliderTypes.CIRCLE:
                        return CircleCircleCollision((PongCircleCollider)colliderA, (PongCircleCollider)colliderB);
                    case ColliderTypes.RECTANGLE:
                        return CircleRectangleCollision((PongCircleCollider)colliderA, (PongRectangleCollider)colliderB);
                }
                break;

            case ColliderTypes.RECTANGLE:
                switch (colliderB.Type()) {
                    case ColliderTypes.CIRCLE:
                        return CircleRectangleCollision((PongCircleCollider)colliderB, (PongRectangleCollider)colliderA); // swapped for less implementation work
                    case ColliderTypes.RECTANGLE:
                        return null; // RECTANGLE RECTANGLE COLLISIONS CURRENTLY DO NOT NEED IMPLEMENTATION
                }
                break;
        }

        return null;
    }

    static CollisionData CircleCircleCollision(PongCircleCollider circleA, PongCircleCollider circleB)
    {
        Vector2 aToB = circleB.position - circleA.position;

        float distance = aToB.magnitude;
        aToB /= distance;

        float depth = circleA.radius + circleB.radius - distance;
        Vector2 collisionPos = (Vector2)circleA.position + aToB * (circleA.radius - depth / 2);

        Vector2 normal;
        if (distance == 0.0f) normal = new Vector2(0, 1);
        else normal = aToB;

        return new CollisionData(circleA, circleB, depth, normal, collisionPos);
    }

    static CollisionData CircleRectangleCollision(PongCircleCollider circleA, PongRectangleCollider rectB)
    {
        throw new System.NotImplementedException();
    }

    static CollisionData CircleConvexHullCollision(PongCircleCollider circleA, PongConvexHullCollider convexB)
    {
        Vector2 AToB = convexB.position - circleA.position;
        return null;
    }
    #endregion
}
