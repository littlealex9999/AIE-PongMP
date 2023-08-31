using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PongRectangleCollider))]
public class PongRectangleColliderEditor : PongColliderEditor
{
    private void OnSceneGUI()
    {
        PongRectangleCollider collider = (PongRectangleCollider)target;
        Handles.color = new Color(0.3f, 1.0f, 0.3f);

        Vector3 min = collider.transform.position - (Vector3)collider.size / 2;
        Vector3 max = collider.transform.position + (Vector3)collider.size / 2;
        Vector3[] points = new Vector3[4] {
            min,
            new Vector3(min.x, max.y),
            max,
            new Vector3(max.x, min.y)
        };

        int[] indices = new int[] {
            0, 1,
            1, 2,
            2, 3,
            3, 0,
        };

        Handles.DrawLines(points, indices);
    }
}
