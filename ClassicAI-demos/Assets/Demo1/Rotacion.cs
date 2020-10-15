using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotacion : MonoBehaviour
{

    private float rotZ;
    public float RotationSpeed;

    // Update is called once per frame
    void Update()
    {
        rotZ += Time.deltaTime * RotationSpeed;

        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
