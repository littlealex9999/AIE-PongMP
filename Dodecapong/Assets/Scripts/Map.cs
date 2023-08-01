using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//[RequireComponent(typeof(LineRenderer))]
public class Map : MonoBehaviour
{
    public float mapRadius;

    [Min(3)] public int lineStepCount;

    public List<int> shieldLevels = new List<int>();
    public List<Paddle> players = new List<Paddle>();

    public Material lrDefault;

    public float lineWidth;

    public GameObject ringMesh;
    public void GenerateMap()
    {
        if (lineStepCount < 1) return;

        RegenerateLineRenderers();
    }

    public List<GameObject> lineRenderers;
    void RegenerateLineRenderers()
    {
        if (lineRenderers.Count == 0)
        {
            int alivePlayerCount = GetLivingPlayerCount();

            for (int currentPlayer = 0; currentPlayer < alivePlayerCount; currentPlayer++)
            {
                GameObject obj = new GameObject();
                obj.transform.parent = transform;
                obj.name = "Line " + currentPlayer;
                LineRenderer lr = obj.AddComponent<LineRenderer>();
                lineRenderers.Add(obj);

                // setup values
                int pointCount = lineStepCount / alivePlayerCount;
                Color playerColor = GameManager.instance.GetPlayerColor(GetTargetLivingPlayerID(currentPlayer));

                lr.material = lrDefault;
                lr.material.SetColor("_EmissiveColor", playerColor);
                lr.positionCount = pointCount + 1;
                lr.startWidth = lr.endWidth = lineWidth;

                Quaternion rotationPerSegment = Quaternion.Euler(0, 0, 360.0f / lineStepCount);

                // first point
                Vector3 targetPos = GetTargetPointInCircleLocal(GameManager.instance.mapRotationOffset + 360 / alivePlayerCount * currentPlayer);
                lr.SetPosition(0, targetPos + transform.position);

                // middle points
                for (int currentPoint = 1; currentPoint < pointCount; currentPoint++)
                {
                    targetPos = rotationPerSegment * targetPos;
                    lr.SetPosition(currentPoint, targetPos + transform.position);
                }

                // last connecting point
                targetPos = GetTargetPointInCircleLocal(GameManager.instance.mapRotationOffset + 360 / alivePlayerCount * (currentPlayer + 1));
                lr.SetPosition(pointCount, targetPos + transform.position);
            }
        }
        else
        {
            foreach (GameObject line in lineRenderers)
            {
                Destroy(line);
            }
            lineRenderers.Clear();
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

    public void ShieldHit(int playerID)
    {
        if (playerID < 0 || playerID >= shieldLevels.Count) return;

        --shieldLevels[playerID];
        if (shieldLevels[playerID] == 0)
        {
            RegenerateLineRenderers();
            RemovePlayer(playerID);
        }
    }

    public void RemovePlayer(int playerID)
    {
        players[playerID].gameObject.SetActive(false);
    }

    public int GetLivingPlayerCount()
    {
        int ret = 0;
        for (int i = 0; i < shieldLevels.Count; i++) {
            if (shieldLevels[i] > 0) ++ret;
        }
        return ret;
    }

    public int GetTargetLivingPlayerID(int index)
    {
        int hits = 0;
        for (int i = 0; i < shieldLevels.Count; i++) {
            if (shieldLevels[i] > 0) ++hits;
            if (hits == index + 1) return i;
        }
        return -1;
    }
}
