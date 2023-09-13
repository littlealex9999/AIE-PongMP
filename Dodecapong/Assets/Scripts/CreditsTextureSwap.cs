using System.Collections;
using System.Collections.Generic;
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
        // apply old second texture to first texture
        // reset lerp to 0
        //pinArtMaterial.SetTexture(, texture);
        //pinArtMaterial.SetFloat(, value);

        pinArtMaterial.SetTexture("_Render_Texture", texture);

        elapsed = 0.0f;
    }

    void SetLerp(float value)
    {
        //pinArtMaterial.SetFloat(, value);
    }
}
