using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Map : MonoBehaviour
{
    public float mapRadius;

    [Min(3)] public int lineStepCount;

    [Range(0, 360)] public float angleOffset;

    public LineRenderer lr;

    public List<int> shieldLevels = new List<int>();

    private void OnValidate()
    {
        CalculateCircle();
    }

    public void CalculateCircle()
    {
        if (!lr || lineStepCount < 1) return;

        lr.positionCount = lineStepCount;
        Vector3 targetPos = GetTargetPointInCircleLocal(angleOffset);
        lr.SetPosition(0, targetPos + transform.position);

        Quaternion rotationPerSegment = Quaternion.Euler(0, 0, 360.0f / lineStepCount);

        for (int i = 1; i < lineStepCount; i++) {
            targetPos = rotationPerSegment * targetPos;
            lr.SetPosition(i, targetPos + transform.position);
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
