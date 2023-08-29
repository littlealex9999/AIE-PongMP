using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualiser : MonoBehaviour
{
    public LineRenderer lr;
    public AudioSource source;
    public Material mat;

    public ParticleSystem particleSystem;
    public ParticleSystem.Particle[] particles;
    public int circleResolution = 360;
    public float circleRadius = 1.0f;
    public float circleWidthStep = 1.0f;
    
    float[] spectrumData = new float[128];

    private void Start()
    {
        particles = new ParticleSystem.Particle[spectrumData.Length * circleResolution];
        particleSystem.maxParticles = particles.Length;
        particleSystem.SetParticles(particles);
    }

    private void Update()
    {
        if (lr.positionCount != spectrumData.Length) lr.positionCount = spectrumData.Length;

        AudioListener.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);

        for (int i = 0; i < spectrumData.Length; i++) {
            lr.SetPosition(i, new Vector3(i / 10.0f, spectrumData[i], 0));
        }

        if (mat) mat.SetFloatArray("_testArray", spectrumData);

        if (particleSystem) {
            for (int i = 0; i < circleResolution; i++) {
                Vector3 normalDir = Quaternion.Euler(0, 0, 360.0f / circleResolution * i) * Vector3.up;

                particleSystem.GetParticles(particles);
                for (int j = 0; j < spectrumData.Length; j++) {
                    particles[i * spectrumData.Length + j].position = normalDir * circleRadius + normalDir * circleWidthStep * j;
                    particles[i * spectrumData.Length + j].startColor = new Color(1, 1, 1, spectrumData[j]);
                }
                particleSystem.SetParticles(particles, particles.Length);
            }
        }
    }
}
