using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VFXColorSetter : MonoBehaviour
{
    private ParticleSystem.MinMaxGradient startColor;
   
    private ParticleSystem.MinMaxGradient colorOverLifetime;

    ParticleSystem[] particleSystems;
    private void OnValidate()
    {
        
        ApplyValues();
    }

    public void SetStartColor(ParticleSystem.MinMaxGradient startColor)
    {
        this.startColor = startColor;
        ApplyValues();
    }

    public void SetStartAndLifetimeColor(ParticleSystem.MinMaxGradient startColor, ParticleSystem.MinMaxGradient colorOverLifetime)
    {
        this.startColor = startColor;
        this.colorOverLifetime = colorOverLifetime;
        ApplyValues();
    }

    void ApplyValues()
    {
        particleSystems ??= GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem p in particleSystems)
        {
            ParticleSystem.MainModule main = p.main;
            ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = p.colorOverLifetime;
            main.startColor = startColor;
            colorOverLifetimeModule.color = colorOverLifetime;
        }
    }
}
