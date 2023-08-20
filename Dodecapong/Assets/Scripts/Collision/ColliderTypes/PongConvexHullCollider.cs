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

    public Vector2[] normals;
    public Vector2 center;

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
}
