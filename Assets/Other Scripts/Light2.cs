using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light2 : MonoBehaviour
{
    public float fireRate = .1f;
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
        if (fireBuffer < 0)
        {
            fireBuffer = fireRate;
        }

        mat.SetVector("_Intensity2", light.color * light.intensity / 2);
        mat.SetVector("_LightPos2", transform.position);
    }
}
