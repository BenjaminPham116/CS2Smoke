using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RotateWithMouse : MonoBehaviour
{
    private float horizontalSpeed = .4f;
    private float verticalSpeed = .4f;
    private PlayerController player;
    public GameObject model;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerController>();
        verticalSpeed = player.verticalSpeed;
        horizontalSpeed = player.horizontalSpeed;
    }

    Vector3 WorldToLocal(Vector3 world)
    {
        return new Vector3(Vector3.Dot(world, transform.right), Vector3.Dot(world, transform.up), Vector3.Dot(world, transform.forward));
    }

    // Update is called once per frame
    void Update()
    {
        float h = horizontalSpeed * Input.GetAxis("Mouse X");
        float v = verticalSpeed * Input.GetAxis("Mouse Y");

        if (Input.GetKey(KeyCode.Mouse1))
        {
            h = 0;
            v = 0;
        }
        
        transform.RotateAround(Vector3.up, h);
        transform.RotateAround(transform.right, -v);

        //transform.eulerAngles = new Vector3(Mathf.Clamp(transform.eulerAngles.x,-90,90),transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
