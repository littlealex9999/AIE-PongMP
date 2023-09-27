using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    new Camera camera;
    Vector3 cameraOriginalPos;
    public float time;
    public float strength = 1.0f;

    void Start()
    {
        camera = Camera.main;
        cameraOriginalPos = camera.transform.position;
    }

    public void SetShakeTime(float duration = 1.0f)
    {
        time = duration;
    }

    public void SetShakeStrength(float str = 0.2f)
    {
        strength = str;
    }

    public void SetShakeData(CameraShakeData data)
    {
        time = data.time;
        strength = data.strength;
    }

    private void Update()
    {
        if (GameManager.instance.enableScreenShake && time > 0) {
            time -= Time.deltaTime;
            if (time < 0) time = 0;

            camera.transform.position = cameraOriginalPos + (Vector3)(Random.insideUnitCircle * strength);
        } else {
            camera.transform.position = cameraOriginalPos;
        }
    }
}

[CreateAssetMenu(fileName = "Camera Shake Data")]
public class CameraShakeData : ScriptableObject
{
    public float time;
    public float strength;
}