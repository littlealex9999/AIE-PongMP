using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PongConvexHullCollider : PongCollider
{
    public override CollisionSystem.ColliderTypes Type() => CollisionSystem.ColliderTypes.CONVEXHULL;

    // preset with values for a square
    public Vector2[] points = new Vector2[4] {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0)
    };

    [HideInInspector]
    public Vector2[] scaledPoints = new Vector2[4] {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0)
    };

    [HideInInspector]
    public Vector2[] normals = new Vector2[4] {
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
    };

    public Vector2[] forceNormals = new Vector2[4] {
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
    };

    public bool[] doResolutionOnFace;

    public Vector2 center = new Vector2(0.5f, 0.5f);
    public Vector3 rotationOffset;

    public Vector2 scale = new Vector2(1, 1);

    [Range(0, 180)]
    public float forceNormalDirectionClamp = 180.0f;

    public void RecalculateNormals()
    {
        RecalculateScale();
        if (doResolutionOnFace.Length != points.Length) RegenrateResolutionBools();

        center = Vector2.zero;
        for (int i = 0; i < points.Length; i++) {
            center += scaledPoints[i];
        }
        center /= scaledPoints.Length;

        normals = new Vector2[points.Length];
        for (int i = 0; i < normals.Length; i++) {
            normals[i] = scaledPoints[i] - scaledPoints[(i + 1) % points.Length];
            normals[i] = new Vector2(normals[i].y, -normals[i].x).normalized;
        }

        if (forceNormals.Length > normals.Length) {
            Vector2[] temp = new Vector2[normals.Length];
            for (int i = 0; i < normals.Length; i++) {
                temp[i] = normals[i];
            }

            forceNormals = temp;
        } else if (forceNormals.Length < normals.Length) {
            Vector2[] temp = new Vector2[normals.Length];
            for (int i = 0; i < forceNormals.Length; i++) {
                temp[i] = forceNormals[i];
            }

            for (int i = forceNormals.Length; i < normals.Length; i++) {
                temp[i] = normals[i];
            }

            forceNormals = temp;
        }
    }

    public void RecalculateScale()
    {
        scaledPoints = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++) {
            scaledPoints[i] = points[i] * scale;
        }
    }

    public void RegenrateResolutionBools()
    {
        if (doResolutionOnFace.Length > points.Length) {
            bool[] temp = new bool[points.Length];
            for (int i = 0; i < points.Length; i++) {
                temp[i] = doResolutionOnFace[i];
            }

            doResolutionOnFace = temp;
        } else if (doResolutionOnFace.Length < points.Length) {
            bool[] temp = new bool[points.Length];
            for (int i = 0; i < doResolutionOnFace.Length; i++) {
                temp[i] = doResolutionOnFace[i];
            }

            for (int i = doResolutionOnFace.Length; i < points.Length; i++) {
                temp[i] = true;
            }

            doResolutionOnFace = temp;
        }
    }

    public Vector2 GetFaceMidpoint(int index)
    {
        return (scaledPoints[index] + scaledPoints[(index + 1) % points.Length]) / 2;
    }

    public Vector3 GetRotationOffset()
    {
        return rotationOffset;
    }

    public void NormalizeNormals()
    {
        for (int i = 0; i < normals.Length; i++) {
            normals[i] = normals[i].normalized;
            forceNormals[i] = forceNormals[i].normalized;
        }
    }

    protected override void StartEvents()
    {
        CollisionSystem.AddPaddleCollider(this);
    }

    protected override void DestroyEvents()
    {
        CollisionSystem.RemovePaddleCollider(this);
    }
}
