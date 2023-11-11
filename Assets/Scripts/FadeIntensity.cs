using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIntensity : MonoBehaviour
{
    public float fireRate = .05f;
    private float fireBuffer;
    private Light light;

    public Material mat;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        fireBuffer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        fireBuffer -= Time.deltaTime;
        light.intensity = fireBuffer / fireRate * 2;
        if (Input.GetKey(KeyCode.Mouse0) && fireBuffer < 0)
        {
            fireBuffer = fireRate;
        }

        mat.SetVector("_Intensity", light.color * light.intensity/2);
        mat.SetVector("_LightPos", transform.position);
    }
}
