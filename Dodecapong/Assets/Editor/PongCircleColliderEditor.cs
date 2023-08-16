using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PongCircleCollider))]
public class PongCircleColliderEditor : Editor
{
    private void OnSceneGUI()
    {
        PongCircleCollider collider = (PongCircleCollider)target;
        Handles.color = new Color(0.3f, 1.0f, 0.3f);
        Handles.DrawWireDisc(collider.transform.position, Vector3.up, collider.radius);
        Handles.DrawWireDisc(collider.transform.position, Vector3.forward, collider.radius);
        Handles.DrawWireDisc(collider.transform.position, Vector3.right, collider.radius);
    }
}
