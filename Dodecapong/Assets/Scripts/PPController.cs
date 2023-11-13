using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
using System.Collections;

public class PPController : MonoBehaviour
{
    public Volume volume;

    [Serializable] public struct ValueAnimator
    {
        [Range(0, 1)][SerializeField] float Default;
        [Range(0, 1)][SerializeField] float Target;

        [SerializeField] float duration;
        [SerializeField] AnimationCurve curve;

        public Action<float> OnValueChanged;
        float value;
        public float Value
        {
            get { return value; }
            private set
            {
                this.value = value;
                OnValueChanged.Invoke(value);
            } 
        }

        public IEnumerator Play()
        {
            float timeElapsed = 0;

            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                Debug.Log(timeElapsed);
                Value = Mathf.Lerp(Default, Target, curve.Evaluate(timeElapsed / duration));
               

                yield return new WaitForEndOfFrame();
            }
            Value = Default;
        }
    }

    public Vignette mVignette;
    public ChromaticAberration mChromaticAberration;
    public Bloom mBloom;

    public ValueAnimator vignetteIntensity;
    public ValueAnimator chromaticAberrationIntensity;
    public ValueAnimator bloomIntensity;

    void OnVingetteValChanged(float value)
    {
        ClampedFloatParameter clampedfloat;
        clampedfloat = mVignette.intensity;
        clampedfloat.value = value;
    }

    void OnChromaticAberrationValChanged(float value)
    {
        ClampedFloatParameter clampedfloat;
        clampedfloat = mChromaticAberration.intensity;
        clampedfloat.value = value;
    }

    void OnBloomValChanged(float value)
    {
        ClampedFloatParameter clampedfloat;
        clampedfloat = mBloom.intensity;
        clampedfloat.value = value;
    }

    public void StartVignette() => StartCoroutine(vignetteIntensity.Play());
    public void StartChromaticAberration() => StartCoroutine(chromaticAberrationIntensity.Play());
    public void StartBloom() => StartCoroutine(bloomIntensity.Play());

    public void GetProfileComponents()
    {
        VolumeProfile volumeProfile = volume.sharedProfile;

        for (int i = 0; i < volumeProfile.components.Count; i++)
        {
            if (volumeProfile.components[i].name == "Vignette")
            {
                mVignette = (Vignette)volumeProfile.components[i];
                vignetteIntensity.OnValueChanged += OnVingetteValChanged;
            }
            if (volumeProfile.components[i].name == "ChromaticAberration")
            {
                mChromaticAberration = (ChromaticAberration)volumeProfile.components[i];
                chromaticAberrationIntensity.OnValueChanged += OnChromaticAberrationValChanged;
            }
            if (volumeProfile.components[i].name == "Bloom")
            {
                mBloom = (Bloom)volumeProfile.components[i];
                bloomIntensity.OnValueChanged += OnBloomValChanged;
            }
        }
    }
}
