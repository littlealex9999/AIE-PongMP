using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Map : MonoBehaviour
{
    public float mapRadius;

    public int lineStepCount;

    public LineRenderer lr;

    private void OnValidate()
    {
        if (!lr) return;

        lr.positionCount = lineStepCount;

        for (int currentStep = 0; currentStep < lineStepCount; currentStep++)
        {
            float progress = (float)currentStep / lineStepCount;

            float currentRadian = progress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * mapRadius;
            float y = yScaled * mapRadius;

            Vector3 currentPosition = new Vector3(x, y, 0);

            lr.SetPosition(currentStep, currentPosition);
        }
    }
}
