using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongConvexHullCollider : PongCollider
{
    public override CollisionSystem.ColliderTypes Type() => CollisionSystem.ColliderTypes.CONVEXHULL;

    public Vector2[] points = new Vector2[4] {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0)
    };

    public Vector2[] normals = new Vector2[4] {
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
    };

    public Vector2 center = new Vector2(0.5f, 0.5f);
    public Vector3 rotationOffset;

    public void RecalculateNormals()
    {
        center = Vector2.zero;
        for (int i = 0; i < points.Length; i++) {
            center += points[i];
        }
        center /= points.Length;

        normals = new Vector2[points.Length];
        for (int i = 0; i < normals.Length; i++) {
            normals[i] = points[i] - points[(i + 1) % points.Length];
            normals[i] = new Vector2(normals[i].y, -normals[i].x).normalized;
        }
    }

    public Vector2 GetFaceMidpoint(int index)
    {
        return (points[index] + points[(index + 1) % points.Length]) / 2;
    }

    public Vector3 GetRotationOffset()
    {
        return rotationOffset;

        Vector3 rotationEulers = transform.rotation.eulerAngles;
        return new Vector3(-rotationEulers.x, -rotationEulers.y, rotationEulers.z);
    }
}
