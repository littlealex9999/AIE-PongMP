using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CreditsTextureSwap : MonoBehaviour
{
    public Material pinArtMaterial;
    public float duration = 2.0f;
    float elapsed = 0.0f;

    private void Update()
    {
        if (elapsed < duration) {
            elapsed += Time.deltaTime;
            if (elapsed > duration) elapsed = duration;

            SetLerp(elapsed / duration);
        }
    }

    public void ApplyNewTexture(Texture texture)
    {
        pinArtMaterial.SetTexture("_Render_Texture", pinArtMaterial.GetTexture("_Face"));
        pinArtMaterial.SetTexture("_Face", texture);
        pinArtMaterial.SetFloat("_Face_On_Off_Lerp", 0.0f);

        elapsed = 0.0f;
    }

    void SetLerp(float value)
    {
        pinArtMaterial.SetFloat("_Face_On_Off_Lerp", value);
    }
}
