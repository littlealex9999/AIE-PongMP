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

        int[] indices = new int[points.Length * 2];
        for (int i = 0; i < points.Length; i++) {
            indices[i * 2] = i;
            indices[i * 2 + 1] = (i + 1) % points.Length;
        }

        Handles.DrawLines(points, indices);

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
        Handles.DrawLines(points, indices);

        for (int i = 0; i < points.Length; i++) {
            Vector3 midPoint = (points[i] + points[(i + 1) % points.Length]) / 2;
            Handles.DrawLine(midPoint, midPoint + collider.transform.rotation * Quaternion.Euler(collider.GetRotationOffset()) * (Vector3)collider.normals[i]);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Recalculate Normals")) {
            ((PongConvexHullCollider)target).RecalculateNormals();
        }
    }
}
