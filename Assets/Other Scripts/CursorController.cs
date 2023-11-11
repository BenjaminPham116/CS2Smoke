using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void LateUpdate()
    {

        Vector3 mouse = Input.mousePosition;

        //enable mouse
        if (Input.GetKey(KeyCode.Mouse1)) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else {
            //lr.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }
}
