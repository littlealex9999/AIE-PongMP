using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

//[RequireComponent(typeof(LineRenderer))]
public class Map : MonoBehaviour
{
    public float mapRadius;

    [Min(3)] public int lineStepCount;

    public List<int> shieldLevels = new List<int>();
    public List<Paddle> players = new List<Paddle>();

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
            int alivePlayerCount = GetLivingPlayerCount();

            for (int currentPlayer = 0; currentPlayer < alivePlayerCount; currentPlayer++)
            {
                // setup values
                int pointCount = lineStepCount / alivePlayerCount;
                Color playerColor = GameManager.instance.GetPlayerColor(GetTargetLivingPlayerID(currentPlayer));

                Quaternion rotationPerSegment = Quaternion.Euler(0, 0, 360.0f / lineStepCount);
                float angle = GameManager.instance.mapRotationOffset + 360 / alivePlayerCount * currentPlayer;
                Vector3 targetPos = GetTargetPointInCircleLocal(angle);

                for (int currentPoint = 0; currentPoint < pointCount; currentPoint++)
                {
                    targetPos = rotationPerSegment * targetPos;
                    GameObject obj = Instantiate(ringMesh, targetPos, Quaternion.identity, transform);
                    obj.GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", GameManager.instance.GetPlayerColor(currentPlayer));
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

    public bool ShieldHit(int playerID)
    {
        if (playerID < 0 || playerID >= shieldLevels.Count) return false;

        --shieldLevels[playerID];
        //if (shieldLevels[playerID] == 0)
        //{
        //    RegenerateLineRenderers();
        //    RemovePlayer(playerID);
        //}

        return true;
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
