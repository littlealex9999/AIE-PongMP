using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsTextureSwap : MonoBehaviour
{
    public Material pinArtMaterial;

    public void ApplyNewTexture(Texture texture)
    {
        pinArtMaterial.SetTexture("_Render_Texture", texture);
    }
}
