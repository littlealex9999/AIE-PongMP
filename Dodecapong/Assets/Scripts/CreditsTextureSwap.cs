using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsTextureSwap : MonoBehaviour
{
    public Material pinArtMaterial;
    public float duration = 2.0f;
    float elapsed = 0.0f;
    float completion;

    private void Update()
    {
        if (elapsed < duration) {
            elapsed += Time.deltaTime;

        }
    }

    public void ApplyNewTexture(Texture texture)
    {
        // apply old second texture to first texture
        // reset lerp to 0

        pinArtMaterial.SetTexture("_Render_Texture", texture);

        elapsed = 0.0f;
    }
}
