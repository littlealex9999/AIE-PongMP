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

    [ContextMenu("Calculate Tex Array")]
    public void CalculateTextureArray()
    {
        if (!mat) mat = GetComponent<MeshRenderer>().sharedMaterial;

        tex = new Texture2D(colors.Length, 1, TextureFormat.ARGB32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
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

    public void SetTargetPlayer(int index) { mat.SetFloat("_TargetPlayer", index); }

    public void SetShrink(float value) { mat.SetFloat("_Shrink", value); }
}
