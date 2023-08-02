using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ArcTanShaderHelper : MonoBehaviour
{
    public Material mat;
    Texture2DArray texArray;
    Texture2D tex;

    public Color[] colors = new Color[8];

    void Update()
    {
        
    }

    [ContextMenu("Calculate Tex Array")]
    void CalculateTextureArray()
    {
        if (!mat) mat = GetComponent<MeshRenderer>().material;

        tex = new Texture2D(colors.Length, 1, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;

        for (int i = 0; i < colors.Length; i++) {
            tex.SetPixel(i, 1, colors[i]);
        }
        tex.Apply();
        mat.SetTexture("_mainTex", tex);
        mat.SetFloat("_PlayerCount", colors.Length);
    }

    private void OnValidate()
    {
        CalculateTextureArray();
    }
}
