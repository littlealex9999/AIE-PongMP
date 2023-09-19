using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VFXColorSetter : MonoBehaviour
{
    public ParticleSystem.MinMaxGradient startColor;
    public ParticleSystem.MinMaxGradient colorOverLifetime;

    ParticleSystem[] particleSystems;
    private void OnValidate()
    {
        particleSystems ??= GetComponentsInChildren<ParticleSystem>();
        ApplyValues();
    }

    void ApplyValues()
    {
        foreach (ParticleSystem p in particleSystems)
        {
            ParticleSystem.MainModule main = p.main;
            ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = p.colorOverLifetime;
            main.startColor = startColor;
            colorOverLifetimeModule.color = colorOverLifetime;
        }
    }
}
