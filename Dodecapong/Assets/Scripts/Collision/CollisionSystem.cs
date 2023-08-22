using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
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
            if (colliders[i].isActiveAndEnabled) {
                colliders[i].CollisionUpdate(Time.fixedDeltaTime);
            }
        }

        for (int i = 0; i < colliders.Count; i++) {
            if (colliders[i].isActiveAndEnabled) {
                for (int j = i + 1; j < colliders.Count; j++) {
                    if (colliders[j].isActiveAndEnabled) {
                        CollisionData data = CheckCollision(colliders[i], colliders[j]);
                        if (data != null && data.isColliding) {
                            data.ResolveCollision();
                        }
                    }
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
                    case ColliderTypes.CONVEXHULL:
                        return CircleConvexHullCollision((PongCircleCollider)colliderA, (PongConvexHullCollider)colliderB);
                }
                break;

            case ColliderTypes.RECTANGLE:
                switch (colliderB.Type()) {
                    case ColliderTypes.CIRCLE:
                        return CircleRectangleCollision((PongCircleCollider)colliderB, (PongRectangleCollider)colliderA); // swapped for less implementation work
                    case ColliderTypes.RECTANGLE:
                        return null; // RECTANGLE RECTANGLE COLLISIONS CURRENTLY DO NOT NEED IMPLEMENTATION
                    case ColliderTypes.CONVEXHULL:
                        return null;
                }
                break;

            case ColliderTypes.CONVEXHULL:
                switch (colliderB.Type()) {
                    case ColliderTypes.CIRCLE:
                        return CircleConvexHullCollision((PongCircleCollider)colliderB, (PongConvexHullCollider)colliderA);
                    case ColliderTypes.RECTANGLE:
                        return null;
                    case ColliderTypes.CONVEXHULL:
                        return null;
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
        Quaternion rotationOffset = convexB.transform.rotation * Quaternion.Euler(convexB.GetRotationOffset());

        float depth = float.MaxValue;
        Vector2 normal = rotationOffset * -convexB.normals[0];
        Vector2 collisionPos = (Vector2)circleA.transform.position + normal * circleA.radius;

        Vector4[] midpoints = new Vector4[convexB.points.Length];
        for (int i = 0; i < midpoints.Length; i++) {
            midpoints[i] = rotationOffset * convexB.GetFaceMidpoint(i) + convexB.transform.position;
        }

        for (int i = 0; i < convexB.points.Length; i++) {
            Vector2 testingNormal = rotationOffset * convexB.normals[i];
            Vector2 testingPoint = (Vector2)circleA.transform.position - testingNormal * circleA.radius;
            float leastDepth = float.MaxValue;

            for (int j = 0; j < convexB.points.Length; j++) {
                float testingDepth = Vector2.Dot((Vector2)midpoints[i] - testingPoint, testingNormal);
                if (testingDepth < leastDepth) leastDepth = testingDepth;
            }

            if (leastDepth < depth) {
                depth = leastDepth;
                normal = -testingNormal;
            }
        }

        Vector2 vel = new Vector2(convexB.velocity.x, convexB.velocity.y);
        Vector2 rotatedVelocity = convexB.transform.rotation * Quaternion.Euler(convexB.GetRotationOffset()) * vel;
        normal = (normal - rotatedVelocity).normalized;

        return new CollisionData(circleA, convexB, depth, normal, collisionPos);
    }
    #endregion
}
