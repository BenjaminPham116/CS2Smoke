using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public Transform transformTo;
    public float turnSpeed = .2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, transformTo.rotation, turnSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}
