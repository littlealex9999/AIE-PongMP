using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualiser : MonoBehaviour
{
    public LineRenderer lr;
    public AudioSource source;
    public AudioClip clip;
    
    float[] spectrumData = new float[256];

    private void Update()
    {
        if (lr.positionCount != spectrumData.Length) lr.positionCount = spectrumData.Length;

        AudioListener.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);

        for (int i = 0; i < spectrumData.Length; i++) {
            lr.SetPosition(i, new Vector3(i / 10.0f, spectrumData[i], 0));
        }
    }
}
