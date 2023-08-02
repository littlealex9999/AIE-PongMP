using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

//[RequireComponent(typeof(LineRenderer))]
public class Map : MonoBehaviour
{
    public float mapRadius;

    [Min(3)] public int lineStepCount;

    public GameObject ringMesh;
    public void GenerateMap()
    {
        if (lineStepCount < 1) return;

        RegenerateLineRenderers();
    }

    public List<GameObject> ringMeshes;
    void RegenerateLineRenderers()
    {
        if (ringMeshes.Count == 0)
        {
            int alivePlayerCount = GameManager.instance.alivePlayerCount;
            for (int currentPlayer = 0; currentPlayer < alivePlayerCount; currentPlayer++)
            {
                // setup values
                int pointCount = lineStepCount / alivePlayerCount;
                Quaternion rotationPerSegment = Quaternion.Euler(0, 0, 360.0f / lineStepCount);
                float angle = GameManager.instance.mapRotationOffset + 360 / alivePlayerCount * currentPlayer;
                Vector3 targetPos = GetTargetPointInCircleLocal(angle);

                // ring segment of players colour
                for (int currentPoint = 0; currentPoint < pointCount; currentPoint++)
                {
                    targetPos = rotationPerSegment * targetPos;
                    GameObject obj = Instantiate(ringMesh, targetPos, Quaternion.identity, transform);
                    obj.GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", GameManager.instance.players[currentPlayer].color);
                    ringMeshes.Add(obj);
                }
            }
        }
        else
        {
            foreach (GameObject obj in ringMeshes)
            {
                Destroy(obj);
            }
            ringMeshes.Clear();
            RegenerateLineRenderers();
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

    public List<TextMeshProUGUI> list = new List<TextMeshProUGUI>();
}
