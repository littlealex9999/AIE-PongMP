using System.Linq;
using UnityEngine;

public class VFXColorSetter : MonoBehaviour
{
    public ParticleSystem.MinMaxGradient startColor;
    public ParticleSystem.MinMaxGradient colorOverLifetime;

    ParticleSystem[] particleSystems;

    private void OnValidate()
    {
        if (particleSystems.Count() == 0) particleSystems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem p in particleSystems)
        {
            ParticleSystem.MainModule main = p.main;
            ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = p.colorOverLifetime;
            main.startColor = startColor;
            colorOverLifetimeModule.color = colorOverLifetime;
        }
    }
}
