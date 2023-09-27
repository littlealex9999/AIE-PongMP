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
        CLIPPEDPLANE,
    }

    static List<PongCollider> colliders = new List<PongCollider>();
    static List<PongCollider> paddleColliders = new List<PongCollider>();

    public static void AddCollider(PongCollider collider) => colliders.Add(collider);
    public static void RemoveCollider(PongCollider collider) => colliders.Remove(collider);
    public static void AddPaddleCollider(PongCollider collider) => paddleColliders.Add(collider);
    public static void RemovePaddleCollider(PongCollider collider) => paddleColliders.Remove(collider);

    private void FixedUpdate()
    {
        for (int i = 0; i < colliders.Count; i++) {
            if (colliders[i].isActiveAndEnabled) {
                colliders[i].CollisionUpdate(Time.fixedDeltaTime);
            }
        }

        for (int i = 0; i < colliders.Count; i++) {
            if (colliders[i].isActiveAndEnabled) {
                #region regular collisions
                // checks all regular colliders against eachother
                for (int j = i + 1; j < colliders.Count; j++) {
                    if (colliders[j].isActiveAndEnabled) {
                        CollisionData data = CheckCollision(colliders[i], colliders[j]);
                        bool alreadyColliding = colliders[i].collidingWith.Contains(colliders[j]) && colliders[j].collidingWith.Contains(colliders[i]);
                        if (data != null && data.isColliding) {
                            // ensure collision enter only triggers once, regardless of being a trigger or not
                            if (!alreadyColliding) {
                                colliders[i].collidingWith.Add(colliders[j]);
                                colliders[j].collidingWith.Add(colliders[i]);
                            }

                            // check if either collider is a trigger. use trigger checks if true
                            if (colliders[i].trigger || colliders[j].trigger) {
                                DoCollisionEvents(colliders[i], colliders[j], colliders[i].OnTrigger, colliders[j].OnTrigger, data);
                                if (!alreadyColliding) {
                                    DoCollisionEvents(colliders[i], colliders[j], colliders[i].OnTriggerEnter, colliders[j].OnTriggerEnter, data);
                                }
                            } else {
                                data.ResolveCollision();
                                DoCollisionEvents(colliders[i], colliders[j], colliders[i].OnCollision, colliders[j].OnCollision, data);
                                if (!alreadyColliding) {
                                    DoCollisionEvents(colliders[i], colliders[j], colliders[i].OnCollisionEnter, colliders[j].OnCollisionEnter, data);
                                }
                            }
                        } else {
                            // ensure collision exit only triggers once, then call the appropriate collision events
                            if (alreadyColliding) {
                                colliders[i].collidingWith.Remove(colliders[j]);
                                colliders[j].collidingWith.Remove(colliders[i]);
                                if (colliders[i].trigger || colliders[j].trigger) {
                                    DoCollisionEvents(colliders[i], colliders[j], colliders[i].OnTriggerExit, colliders[j].OnTriggerExit, data);
                                } else {
                                    DoCollisionEvents(colliders[i], colliders[j], colliders[i].OnCollisionExit, colliders[j].OnCollisionExit, data);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region paddle
                // checks everything against the paddles
                for (int j = 0; j < paddleColliders.Count; j++) {
                    if (paddleColliders[j].isActiveAndEnabled) {
                        CollisionData data = CheckCollision(colliders[i], paddleColliders[j]);
                        bool alreadyColliding = colliders[i].collidingWith.Contains(paddleColliders[j]) && paddleColliders[j].collidingWith.Contains(colliders[i]);
                        if (data != null && data.isColliding) {
                            // ensure collision enter only triggers once, regardless of being a trigger or not
                            if (!alreadyColliding) {
                                colliders[i].collidingWith.Add(paddleColliders[j]);
                                paddleColliders[j].collidingWith.Add(colliders[i]);
                            }

                            // check if either collider is a trigger. use trigger checks if true
                            if (colliders[i].trigger || paddleColliders[j].trigger) {
                                DoCollisionEvents(colliders[i], paddleColliders[j], colliders[i].OnPaddleTrigger, paddleColliders[j].OnPaddleTrigger, data);
                                if (!alreadyColliding) {
                                    DoCollisionEvents(colliders[i], paddleColliders[j], colliders[i].OnPaddleTriggerEnter, paddleColliders[j].OnPaddleTriggerEnter, data);
                                }
                            } else {
                                data.ResolveCollision();
                                DoCollisionEvents(colliders[i], paddleColliders[j], colliders[i].OnPaddleCollision, paddleColliders[j].OnPaddleCollision, data);
                                if (!alreadyColliding) {
                                    DoCollisionEvents(colliders[i], paddleColliders[j], colliders[i].OnPaddleCollisionEnter, paddleColliders[j].OnPaddleCollisionEnter, data);
                                }
                            }
                        } else {
                            // ensure collision exit only triggers once, then call the appropriate collision events
                            if (alreadyColliding) {
                                colliders[i].collidingWith.Remove(paddleColliders[j]);
                                paddleColliders[j].collidingWith.Remove(colliders[i]);
                                if (colliders[i].trigger || paddleColliders[j].trigger) {
                                    DoCollisionEvents(colliders[i], paddleColliders[j], colliders[i].OnPaddleTriggerExit, paddleColliders[j].OnPaddleTriggerExit, data);
                                } else {
                                    DoCollisionEvents(colliders[i], paddleColliders[j], colliders[i].OnPaddleCollisionExit, paddleColliders[j].OnPaddleCollisionExit, data);
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }
    }

    void DoCollisionEvents(PongCollider colliderA, PongCollider colliderB, PongCollider.CollisionEvents eventA, PongCollider.CollisionEvents eventB, CollisionData data)
    {
        if (eventA != null)
            eventA.Invoke(colliderB, data);
        if (eventB != null)
            eventB.Invoke(colliderA, data);
    }

    #region Collisions
    static CollisionData CheckCollision(PongCollider colliderA, PongCollider colliderB)
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
                        //case ColliderTypes.CLIPPEDPLANE:
                        //return CircleClippedPlaneCollision((PongCircleCollider)colliderA, (PongClippedPlaneCollider)colliderB);
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
        Vector2 forceNormal = rotationOffset * -convexB.forceNormals[0];

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

            if (leastDepth < 0 && leastDepth < depth || convexB.doResolutionOnFace[i] && leastDepth < depth) {
                depth = leastDepth;
                normal = -testingNormal;
                forceNormal = rotationOffset * -convexB.forceNormals[i];
            }
        }

        Vector2 vel = new Vector2(convexB.velocity.x, convexB.velocity.y);
        forceNormal = (forceNormal - convexB.velocity * convexB.normalBending).normalized;
        Vector2 collisionPos = (Vector2)circleA.transform.position + normal * circleA.radius;

        return new CollisionData(circleA, convexB, depth, normal, forceNormal, collisionPos);
    }

    //static CollisionData CircleClippedPlaneCollision(PongCircleCollider circleA, PongClippedPlaneCollider planeB)
    //{

    //}
    #endregion
}
