using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Map : MonoBehaviour
{
    public float mapRadius;

    public int lineStepCount;

    [Range(0, 360)] public float angleOffset;

    public LineRenderer lr;

    private void OnValidate()
    {
        if (!lr || lineStepCount < 1) return;

        // for some reason, line count borks everything

        lr.positionCount = lineStepCount;
        Vector3 startPos = GetTargetPointInCircleLocal(angleOffset);
        lr.SetPosition(0, startPos + transform.position);

        Quaternion targetRotation = Quaternion.identity;
        Quaternion rotationPerSegment = Quaternion.Euler(0, 0, 360 / lineStepCount);

        for (int i = 1; i < lineStepCount; i++) {
            targetRotation *= rotationPerSegment;
            lr.SetPosition(i, targetRotation * startPos + transform.position);
        }
    }

    public Vector3 GetTargetPointInCircle(float angle)
    {
        return transform.position + Quaternion.Euler(0, 0, angle) * transform.up * mapRadius;
    }

    public Vector3 GetTargetPointInCircleLocal(float angle)
    {
        return Quaternion.Euler(0, 0, angle) * transform.up * mapRadius;
    }
}
