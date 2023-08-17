using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PongCollider), true)]
public class PongColliderEditor : Editor
{
    static readonly string[] excludeVars = {
        "m_Script",
    };

    public override void OnInspectorGUI()
    {
        // Mass
        serializedObject.FindProperty("inverseMass").floatValue = 1 / EditorGUILayout.FloatField("Mass", 1 / serializedObject.FindProperty("inverseMass").floatValue);
        DrawPropertiesExcluding(serializedObject, excludeVars);

        serializedObject.ApplyModifiedProperties();
    }
}
