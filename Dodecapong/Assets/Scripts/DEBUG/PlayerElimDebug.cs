using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerElimDebug : MonoBehaviour
{
    public List<GameObject> pillars = new List<GameObject>();
    public ArcTanShaderHelper shaderHelper;

    public int players = 3;
    public int elimIndex = 0;
    [Range(0, 1)] public float elimAmount;

    public float mapRadius;

    void Update()
    {
        float[] startAngles = new float[players];
        float[] targetAngles = new float[players];

        for (int i = 0; i < players; i++) {
            startAngles[i] = 360.0f / players * i;
            targetAngles[i] = 360.0f / (players - 1);

            int targetPlayer = i;
            if (i > elimIndex) --targetPlayer;
            targetAngles[i] *= targetPlayer;

            float lerpedAngle = Mathf.Lerp(startAngles[i], targetAngles[i], elimAmount);

            pillars[i].transform.position = GetTargetPointInCircle(lerpedAngle);
            pillars[i].transform.rotation = Quaternion.Euler(0, 0, lerpedAngle);
        }

        shaderHelper.SetTargetPlayer(elimIndex);

        float sin = Mathf.Sqrt(Mathf.Sin(elimAmount * Mathf.PI) / (3 * 3));
        float sin2 = Mathf.Sqrt(Mathf.Sin(elimAmount / 2 * Mathf.PI));
        float arcTanShrink = elimAmount + (sin2 - elimAmount) * sin;
        shaderHelper.SetShrink(arcTanShrink);
    }

    public Vector3 GetTargetPointInCircle(float angle)
    {
        return Quaternion.Euler(0, 0, angle) * Vector3.up * mapRadius;
    }
}
