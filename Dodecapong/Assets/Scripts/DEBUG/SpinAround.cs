using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAround : MonoBehaviour
{
    public float rotationSpeed;

    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.forward, rotationSpeed);
    }
}
