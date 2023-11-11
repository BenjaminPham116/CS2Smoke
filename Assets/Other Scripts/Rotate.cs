using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.I)){
            transform.RotateAround(Vector3.right, 1f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.K))
        {
            transform.RotateAround(Vector3.right, -1f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.J))
        {
            transform.RotateAround(Vector3.up, 1f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.L))
        {
            transform.RotateAround(Vector3.up, -1f * Time.deltaTime);
        }
    }
}
