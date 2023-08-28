using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
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
        float pseudoPlayerCount = players - elimAmount;
        for (int i = 0; i < players; i++) {
            float targetAngle = 360.0f / pseudoPlayerCount * i;
            if (i > elimIndex) {
                int countAfter = players - i;
                targetAngle = 360.0f / players * i - 360.0f / players / pseudoPlayerCount * elimAmount * countAfter;
            }

            pillars[i].transform.position = GetTargetPointInCircle(targetAngle);
            pillars[i].transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }

        shaderHelper.SetTargetPlayer(elimIndex);
        shaderHelper.SetShrink(elimAmount);
    }

    public Vector3 GetTargetPointInCircle(float angle)
    {
        return Quaternion.Euler(0, 0, angle) * Vector3.up * mapRadius;
    }
}
