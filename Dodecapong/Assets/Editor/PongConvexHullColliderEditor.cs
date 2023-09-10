using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PongConvexHullCollider))]
public class PongConvexHullColliderEditor : PongColliderEditor
{
    private void OnSceneGUI()
    {
        PongConvexHullCollider collider = (PongConvexHullCollider)target;
        Handles.color = new Color(0.3f, 1.0f, 0.3f);
        Vector3[] points = new Vector3[collider.points.Length];
        for (int i = 0; i < points.Length; i++) {
            points[i] = collider.transform.rotation * Quaternion.Euler(collider.GetRotationOffset()) * (Vector3)collider.points[i] + collider.transform.position;
        }

        collider.RegenrateResolutionBools();
        int activeFaces = 0;
        for (int i = 0; i < points.Length; i++) {
            if (collider.doResolutionOnFace[i]) ++activeFaces;
        }

        List<int> l_collisionActiveIndices = new List<int>();
        List<int> l_collisionInactiveIndices = new List<int>();
        for (int i = 0; i < points.Length; i++) {
            if (collider.doResolutionOnFace[i]) {
                l_collisionActiveIndices.Add(i);
                l_collisionActiveIndices.Add((i + 1) % points.Length);
            } else {
                l_collisionInactiveIndices.Add(i);
                l_collisionInactiveIndices.Add((i + 1) % points.Length);
            }
        }

        int[] collisionActiveIndices = l_collisionActiveIndices.ToArray();
        int[] collisionInactiveIndices = l_collisionInactiveIndices.ToArray();

        Handles.DrawLines(points, collisionActiveIndices);

        for (int i = 0; i < points.Length; i++) {
            Vector3 targetPoint = Handles.FreeMoveHandle(points[i], Quaternion.identity, 0.05f, new Vector3(0.1f, 0.1f, 0.0f), Handles.DotHandleCap);
            
            if (targetPoint != points[i]) {
                collider.points[i] += (Vector2)(targetPoint - points[i]);
                collider.RecalculateNormals();
            }
        }

        Handles.color = Color.red;
        Handles.FreeMoveHandle((Vector3)(collider.transform.rotation * Quaternion.Euler(collider.GetRotationOffset()) * collider.center) + collider.transform.position, Quaternion.identity, 0.05f, new Vector3(0.1f, 0.1f, 0.0f), Handles.DotHandleCap);

        for (int i = 0; i < points.Length; i++) {
            points[i] = collider.transform.rotation * Quaternion.Euler(collider.GetRotationOffset()) * (Vector3)collider.scaledPoints[i] + collider.transform.position;
        }
        Handles.DrawLines(points, collisionActiveIndices);

        for (int i = 0; i < points.Length; i++) {
            Vector3 midPoint = (points[i] + points[(i + 1) % points.Length]) / 2;
            Handles.DrawLine(midPoint, midPoint + collider.transform.rotation * Quaternion.Euler(collider.GetRotationOffset()) * (Vector3)collider.normals[i]);
        }
    }

    public override void OnInspectorGUI()
    {
        PongConvexHullCollider collider = (PongConvexHullCollider)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Recalculate Normals")) {
            collider.RecalculateNormals();
        }

        //if (collider.doResolutionOnFace.Length != collider.points.Length) {
        //    collider.RegenrateResolutionBools();
        //}
    }
}
