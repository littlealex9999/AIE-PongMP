using CodiceApp.EventTracking.Plastic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[CustomEditor(typeof(PPController))]
public class PPControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PPController controller = (PPController)target;


        if (GUILayout.Button("Get Components"))
        {
            controller.GetProfileComponents();
        }
        if (GUILayout.Button("Play Vignette"))
        {
            controller.StartVignette();
        }
        if (GUILayout.Button("Play Chromatic Aberration"))
        {
            controller.StartChromaticAberration();
        }
        if (GUILayout.Button("Play Bloom"))
        {
            controller.StartBloom();
        }




    }
}
