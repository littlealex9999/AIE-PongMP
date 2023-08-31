using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualiser : MonoBehaviour
{
    public int spectrumDataSize = 128;
    public GameObject followObject;

    new public ParticleSystem particleSystem;
    public ParticleSystem.Particle[] particles;
    public int circleResolution = 360;
    public float circleRadius = 1.0f;
    public float circleWidthStep = 1.0f;
    public float alphaMultiplier = 1.0f;

    float[] spectrumData;

    private void Start()
    {
        spectrumData = new float[spectrumDataSize];
        particles = new ParticleSystem.Particle[spectrumData.Length * circleResolution];
        particleSystem.maxParticles = particles.Length;
        particleSystem.SetParticles(particles);
    }

    private void Update()
    {
        AudioListener.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);

        if (particleSystem) {
            if (particleSystem.maxParticles != spectrumData.Length * circleResolution) {
                particleSystem.maxParticles = spectrumData.Length * circleResolution;
            }

            Vector3 offsetPos;
            if (followObject) offsetPos = followObject.transform.position;
            else offsetPos = Vector3.zero;

            for (int i = 0; i < circleResolution; i++) {
                Vector3 normalDir = Quaternion.Euler(0, 0, 360.0f / circleResolution * i) * Vector3.up;

                particleSystem.GetParticles(particles);
                for (int j = 0; j < spectrumData.Length; j++) {
                    particles[i * spectrumData.Length + j].position = normalDir * circleRadius + normalDir * circleWidthStep * j + offsetPos;
                    particles[i * spectrumData.Length + j].startColor = new Color(1, 1, 1, spectrumData[j] * alphaMultiplier);
                }
                particleSystem.SetParticles(particles, particles.Length);
            }
        }
    }
}
