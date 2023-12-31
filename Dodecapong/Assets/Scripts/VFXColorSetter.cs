using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VFXColorSetter : MonoBehaviour
{
    [SerializeField] private ParticleSystem.MinMaxGradient startColor;

    [SerializeField] private bool enableColorOverLifeTime;
    [SerializeField] private ParticleSystem.MinMaxGradient colorOverLifetime;

    ParticleSystem[] particleSystems;
    [SerializeField] private ParticleSystem[] ignoredSystems;

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

    public void SetLifetimeColor(ParticleSystem.MinMaxGradient colorOverLifetime)
    {
        this.colorOverLifetime = colorOverLifetime;

        ApplyValues();
    }

    void ApplyValues()
    {
        particleSystems ??= GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem p in particleSystems)
        {
            if (ignoredSystems.Contains(p)) continue;
            ParticleSystem.MainModule main = p.main;
            main.startColor = startColor;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = p.colorOverLifetime;
            colorOverLifetimeModule.enabled = enableColorOverLifeTime;
            colorOverLifetimeModule.color = colorOverLifetime;
        }
    }
}
