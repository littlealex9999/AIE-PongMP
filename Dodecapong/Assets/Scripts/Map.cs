using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public float mapRadius;

    [Min(3)] public int lineStepCount;

    public GameObject ringMesh;

    public Material arcTangentShader;
    public float removeSpeed = 1;

    public void SetupMap(List<Player> alivePlayers)
    {
        GenerateMap();
        arcTangentShader.SetFloat("_Shrink", 0);
        GameManager.instance.arcTanShader.colors = new Color[alivePlayers.Count];
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            GameManager.instance.arcTanShader.colors[i] = alivePlayers[i].color;
        }
        GameManager.instance.arcTanShader.CalculateTextureArray();
    }

    public void RemoveSegment(int index, List<Player> alivePlayers)
    {
        StartCoroutine(RemovalCoroutine(index, alivePlayers));
    }

    IEnumerator RemovalCoroutine(int index, List<Player> alivePlayers)
    {
        arcTangentShader.SetFloat("_TargetPlayer", index);

        float value = 0;
        float timeElapsed = 0;
        float duration = 1;
        while (timeElapsed < duration)
        { 
            value = Mathf.Lerp(0, 1, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            arcTangentShader.SetFloat("_Shrink", value);

            yield return new WaitForEndOfFrame();
        }

        SetupMap(alivePlayers);

        yield break;
    }


    void GenerateMap()
    {
        if (lineStepCount < 1) return;

        RegenerateLines();
    }

    public List<GameObject> ringMeshes;
    void RegenerateLines()
    {
        if (ringMeshes.Count == 0)
        {
            for (int i = 0; i < lineStepCount; i++)
            {
                Vector3 targetPos = GetTargetPointInCircleLocal(i);
                GameObject obj = Instantiate(ringMesh, targetPos, Quaternion.identity, transform);
                ringMeshes.Add(obj);
            }
        }
        else
        {
            foreach (GameObject obj in ringMeshes)
            {
                Destroy(obj);
            }
            ringMeshes.Clear();
            RegenerateLines();
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
